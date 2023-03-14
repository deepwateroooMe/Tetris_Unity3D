using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using static zFramework.Misc.Loom;
namespace ET {
    
// 封装Socket,将回调push到主线程处理 
    public sealed class TChannel : AChannel {
        private readonly TService Service;
        private Socket socket;

        private SocketAsyncEventArgs innArgs = new SocketAsyncEventArgs();
        private SocketAsyncEventArgs outArgs = new SocketAsyncEventArgs();
        private readonly CircularBuffer recvBuffer = new CircularBuffer();
        private readonly CircularBuffer sendBuffer = new CircularBuffer();

        private bool isSending;
        private bool isConnected;
        private readonly PacketParser parser;
        private readonly byte[] sendCache = new byte[Packet.OpcodeLength + Packet.ActorIdLength];

        private void OnComplete(object sender, SocketAsyncEventArgs e) {
            switch (e.LastOperation) { // 都精简：将异步回调执行的结果，同步到主线程 
                case SocketAsyncOperation.Connect:
                    Post(() => OnConnectComplete(e));
                    break;
                case SocketAsyncOperation.Receive:
                    Post(() => OnRecvComplete(e)); // 它说是，走到这里出错的，来自于Loom Update(), 出错的地方是在 ping 消息之后？回想一下昨天那个错误信息，把这块儿看具体点儿
                    break;
                case SocketAsyncOperation.Send:
                    Post(() => OnSendComplete(e));
                    break;
                case SocketAsyncOperation.Disconnect:
                    Post(() => OnDisconnectComplete(e));
                    break;
                default:
                    throw new Exception($"socket error: {e.LastOperation}");
            }
        }
#region 网络线程
        public TChannel(long id, IPEndPoint ipEndPoint, TService service) { // 两条专门信道，两种不同的构建 
            this.ChannelType = ChannelType.Connect;
            this.Id = id;
            this.Service = service;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.NoDelay = true;
            this.parser = new PacketParser(this.recvBuffer, this.Service);
            this.innArgs.Completed += this.OnComplete;
            this.outArgs.Completed += this.OnComplete;
            this.RemoteAddress = ipEndPoint;
            this.isConnected = false;
            this.isSending = false;
            PostNext(this.ConnectAsync);
        }
        public TChannel(long id, Socket socket, TService service) {
            this.ChannelType = ChannelType.Accept;
            this.Id = id;
            this.Service = service;
            this.socket = socket;
            this.socket.NoDelay = true;
            this.parser = new PacketParser(this.recvBuffer, this.Service);
            this.innArgs.Completed += this.OnComplete;
            this.outArgs.Completed += this.OnComplete;
            this.RemoteAddress = (IPEndPoint)socket.RemoteEndPoint;
            this.isConnected = true;
            this.isSending = false;
            // 下一帧再开始读写：这里会存在慢一桢的问题吗？应该是感觉不到的，1 秒 60 桢
            PostNext(() => {
                this.StartRecv();
                this.StartSend();
            });
        }
        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            Debug.Log($"channel dispose: {this.Id} {this.RemoteAddress}");
            long id = this.Id;
            this.Id = 0;
            this.Service.Remove(id);
            this.socket.Close();
            this.innArgs.Dispose();
            this.outArgs.Dispose();
            this.innArgs = null;
            this.outArgs = null;
            this.socket = null;
        }
        public void Send(long actorId, MemoryStream stream) {
            if (this.IsDisposed) {
                throw new Exception("TChannel已经被Dispose, 不能发送消息");
            }
            switch (this.Service.ServiceType) {
                case ServiceType.Inner: { // 内网消息 
                        int messageSize = (int)(stream.Length - stream.Position);
                        if (messageSize > ushort.MaxValue * 16) {
                            throw new Exception($"send packet too large: {stream.Length} {stream.Position}");
                        }
                        this.sendCache.WriteTo(0, messageSize);
                        this.sendBuffer.Write(this.sendCache, 0, PacketParser.InnerPacketSizeLength);
                        stream.GetBuffer().WriteTo(0, actorId); // actorId: 将这个 actorId 写入【实际上个更新】内存流，相应位置（头，都不需要序列化）
                        // 内网消息，上下文，这里所作的事情应该只是，读取消息头，知道它是内网消息，内存流消息内容不用反序列化再序列化，直接更新内存流的 actorId, 就从内存上转发出去
                        this.sendBuffer.Write(stream.GetBuffer(), (int)stream.Position, (int)(stream.Length - stream.Position));
                        break;
                    }
            case ServiceType.Outer: { // 应该是可以走到这里来： 
                    stream.Seek(Packet.ActorIdLength, SeekOrigin.Begin); // 外网不需要actorId, 所以快进，跳过 actorId 部分
                    ushort messageSize = (ushort)(stream.Length - stream.Position); // 读取消息长度：重要，是因为以大块为单位的流式读取，长短错了，就一定会读错消息 
                    this.sendCache.WriteTo(0, messageSize); // 本地发送缓存 
                    this.sendBuffer.Write(this.sendCache, 0, PacketParser.OuterPacketSizeLength); // 写入，信道的发送缓存区：先写入的是，消息的长度
                    this.sendBuffer.Write(stream.GetBuffer(), (int)stream.Position, (int)(stream.Length - stream.Position)); // 再写入，从内存流上读取的消息的内容，准备就绪，接下来就可以，从消息 accept 信道将消息发出去了
                    break;
                }
            }
            if (!this.isSending) { 
                // this.StartSend();
                this.Service.NeedStartSend.Add(this.Id); // 缓存好，准备下一桢Update() 的时候，发送出去：
            }
        }
        private void ConnectAsync() {
            this.outArgs.RemoteEndPoint = this.RemoteAddress;
            if (this.socket.ConnectAsync(this.outArgs)) {
                return;
            }
            OnConnectComplete(this.outArgs);
        }
        private void OnConnectComplete(object o) {
            if (this.socket == null) {
                return;
            }
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;
            if (e.SocketError != SocketError.Success) {
                this.OnError((int)e.SocketError);
                return;
            }
            e.RemoteEndPoint = null;
            this.isConnected = true;
            this.StartRecv();
            this.StartSend();
        }
        private void OnDisconnectComplete(object o) {
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;
            this.OnError((int)e.SocketError);
        }
        private void StartRecv() {
            while (true) {
                try {
                    if (this.socket == null) return;
                    int size = this.recvBuffer.ChunkSize - this.recvBuffer.LastIndex;
                    this.innArgs.SetBuffer(this.recvBuffer.Last, this.recvBuffer.LastIndex, size);
                }
                catch (Exception e) {
                    Debug.LogError($"tchannel error: {this.Id}\n{e}");
                    this.OnError(ErrorCore.ERR_TChannelRecvError);
                    return;
                }
                if (this.socket.ReceiveAsync(this.innArgs)) return;
                this.HandleRecv(this.innArgs);
            }
        }
        private void OnRecvComplete(object o) {
            this.HandleRecv(o); // <<<<<<<<<<<<<<<<<<<< 
            if (this.socket == null) {
                return;
            }
            this.StartRecv();
        }
        private void HandleRecv(object o) {
            if (this.socket == null) {
                return;
            }
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;
            if (e.SocketError != SocketError.Success) {
                this.OnError((int)e.SocketError);
                return;
            }
            if (e.BytesTransferred == 0) { // ERR_PeerDisconnect: 被抛出了这个异常：就是客户端发一个Ping 消息给服务器；服务器想写一个时间回来，可是这里去读的时候出错了
                this.OnError(ErrorCore.ERR_PeerDisconnect); // <<<<<<<<<<<<<<<<<<<< 感觉昨天晚上运行时抛出的错误有点儿底层。无论是概念上，还是底层原理上，都不太懂。
                return; // 要仔细熟悉这一块儿，今天晚上回家睡前再运行一遍，截个日志 
            }
            this.recvBuffer.LastIndex += e.BytesTransferred;
            if (this.recvBuffer.LastIndex == this.recvBuffer.ChunkSize) {
                this.recvBuffer.AddLast();
                this.recvBuffer.LastIndex = 0;
            }
            // 收到消息回调
            while (true) {
                // 这里循环解析消息执行，有可能，执行消息的过程中断开了session
                if (this.socket == null) {
                    return;
                }
                try {
                    bool ret = this.parser.Parse();
                    if (!ret) {
                        break;
                    }
                    this.OnRead(this.parser.MemoryStream);
                }
                catch (Exception ee) {
                    Debug.LogError($"ip: {this.RemoteAddress} {ee}");
                    this.OnError(ErrorCore.ERR_SocketError);
                    return;
                }
            }
        }
        public void Update() {
            this.StartSend();
        }
        private void StartSend() {
            if (!this.isConnected) {
                return;
            }
            if (this.isSending) {
                return;
            }
            while (true) { // <<<<<<<<<< 
                try {
                    if (this.socket == null) {
                        this.isSending = false;
                        return;
                    }
                    // 没有数据需要发送
                    if (this.sendBuffer.Length == 0) {
                        this.isSending = false;
                        return;
                    }
                    this.isSending = true;
                    int sendSize = this.sendBuffer.ChunkSize - this.sendBuffer.FirstIndex;
                    if (sendSize > this.sendBuffer.Length) {
                        sendSize = (int)this.sendBuffer.Length;
                    }
                    this.outArgs.SetBuffer(this.sendBuffer.First, this.sendBuffer.FirstIndex, sendSize);
                    if (this.socket.SendAsync(this.outArgs)) { // 为什么直接返回了？这里应该是信道发送缓存区的数据发完了
                        return;
                    }
                    HandleSend(this.outArgs); // <<<<<<<<<< 以大块为单位的发送，发完一块儿再发下一块儿，直到发完
                }
                catch (Exception e) {
                    throw new Exception($"socket set buffer error: {this.sendBuffer.First.Length}, {this.sendBuffer.FirstIndex}", e);
                }
            }
        }
        private void OnSendComplete(object o) {
            HandleSend(o);
            this.isSending = false;
            this.StartSend();
        }
        private void HandleSend(object o) {
            if (this.socket == null) {
                return;
            }
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;
            if (e.SocketError != SocketError.Success) {
                this.OnError((int)e.SocketError);
                return;
            }
            if (e.BytesTransferred == 0) {
                this.OnError(ErrorCore.ERR_PeerDisconnect);
                return;
            }
            this.sendBuffer.FirstIndex += e.BytesTransferred; // 以大块为单位的发送，发完一块儿，再发下一块儿？
            if (this.sendBuffer.FirstIndex == this.sendBuffer.ChunkSize) {
                this.sendBuffer.FirstIndex = 0;
                this.sendBuffer.RemoveFirst();
            }
        }
        private void OnRead(MemoryStream memoryStream) {
            try {
                long channelId = this.Id;
                this.Service.OnRead(channelId, memoryStream);
            }
            catch (Exception e) {
                Debug.LogError($"{this.RemoteAddress} {memoryStream.Length} {e}");
                // 出现任何消息解析异常都要断开Session，防止客户端伪造消息
                this.OnError(ErrorCore.ERR_PacketParserError);
            }
        }
        private void OnError(int error) {
            Debug.Log($"TChannel OnError: {error} {this.RemoteAddress}");
            long channelId = this.Id;
            this.Service.Remove(channelId);
            this.Service.OnError(channelId, error);
        }
#endregion
    }
}