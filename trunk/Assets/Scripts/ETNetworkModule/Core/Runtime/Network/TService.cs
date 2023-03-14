using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using static zFramework.Misc.Loom;
namespace ET {
    // 再封装一层 AService 的原因或是目的是什么：它管理了多个不同的信道，一个服务类型可以开启多个不同的信道
    public sealed class TService : AService {
        private const string TAG = "TService";

        private readonly Dictionary<long, TChannel> idChannels = new Dictionary<long, TChannel>();
        private readonly SocketAsyncEventArgs innArgs = new SocketAsyncEventArgs();

        public HashSet<long> NeedStartSend = new HashSet<long>();
        private Socket acceptor;
        private bool m_DisposeCalled;

        public TService(ServiceType serviceType) {
            this.ServiceType = serviceType;
        }
        public TService(IPEndPoint ipEndPoint, ServiceType serviceType) {
            this.ServiceType = serviceType;
            this.acceptor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.acceptor.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this.innArgs.Completed += this.OnComplete;
            this.acceptor.Bind(ipEndPoint);
            this.acceptor.Listen(1000); // 是说，当前服务，最多一次连接 1000 个客户端
// 这里它用Loom, 可是感觉原理上是差不多呀？甚至ET 框架的包装更为精简，还是说ET NetService 里关于主线程＋异步线程的同步过程的逻辑我没有消化透彻，没有包括进来？
            PostNext(this.AcceptAsync); 
        }
        private void OnComplete(object sender, SocketAsyncEventArgs e) {
            switch (e.LastOperation) {
                case SocketAsyncOperation.Accept:
                    SocketError socketError = e.SocketError;
                    Socket acceptSocket = e.AcceptSocket;
                    Post(() => { this.OnAcceptComplete(socketError, acceptSocket); });
                    break;
                default:
                    throw new Exception($"socket error: {e.LastOperation}");
            }
        }
        
// 再想一下：网络线程使用的目的，是为了减少主线程的负担；但当它把活儿干完了，结果却一定是要让主线程知道的，所以一定把结果同步到主线程
#region 网络线程
        private void OnAcceptComplete(SocketError socketError, Socket acceptSocket) {
            if (this.acceptor == null) return;
            if (socketError != SocketError.Success) {
                Debug.LogError($"accept error {socketError}");
                return;
            }
            try {
                long id = this.CreateAcceptChannelId(0); // 这就是前面没仔细看明白的。连接是一个专用信道。当连接成功并结束了，开启一个专门的数据传输信道
                TChannel channel = new TChannel(id, acceptSocket, this);
                this.idChannels.Add(channel.Id, channel);
                long channelId = channel.Id;
                this.OnAccept(channelId, channel.RemoteAddress);
            }
            catch (Exception exception) {
                Debug.LogError(exception);
            }
            this.AcceptAsync();// 开始新的accept
        }
        private void AcceptAsync() {
            this.innArgs.AcceptSocket = null;
            if (this.acceptor.AcceptAsync(this.innArgs)) {
                return;
            }
            OnAcceptComplete(this.innArgs.SocketError, this.innArgs.AcceptSocket);
        }
        private TChannel Create(IPEndPoint ipEndPoint, long id) {
            TChannel channel = new TChannel(id, ipEndPoint, this);
            this.idChannels.Add(channel.Id, channel);
            return channel;
        }
        protected override void Get(long id, IPEndPoint address) {
            if (this.idChannels.TryGetValue(id, out TChannel _)) {
                return;
            }
            this.Create(address, id);
        }
        private TChannel Get(long id) {
            this.idChannels.TryGetValue(id, out var channel);
            return channel;
        }
        public override void Dispose() {
            m_DisposeCalled=true;
            this.acceptor?.Close();
            this.acceptor = null;
            this.innArgs.Dispose();
            foreach (long id in this.idChannels.Keys.ToArray()) {
                TChannel channel = this.idChannels[id];
                channel.Dispose();
            }
            this.idChannels.Clear();
        }
        public override void Remove(long id) {
            if (this.idChannels.TryGetValue(id, out TChannel channel)) {
                channel.Dispose();
            }
            this.idChannels.Remove(id);
        }
        protected override void Send(long channelId, long actorId, MemoryStream stream) { // 这里是，发送消息的地方
            try { 
                TChannel aChannel = this.Get(channelId); // 这里是对的，仍然能够拿到或是创建信道
                if (aChannel == null) {
                    this.OnError(channelId, ErrorCore.ERR_SendMessageNotFoundTChannel);
                    return;
                }
                aChannel.Send(actorId, stream); // 这里就是从信道上将消息发出去
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }
        public override void Update() {
            foreach (var channelId in NeedStartSend) {
                TChannel tChannel = this.Get(channelId);
                tChannel?.Update(); // 开始发送信道发送缓存区上的数据。这里的 tChannel 是非空的
            }
            this.NeedStartSend.Clear();
        }
        public override bool IsDispose() =>m_DisposeCalled;
#endregion
    }
}


