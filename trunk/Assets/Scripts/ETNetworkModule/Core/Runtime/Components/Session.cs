using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;
namespace ET {
    public readonly struct RpcInfo { // 这个封装：与以前一样吗？无所谓，就是既有发送端，也有回收的 IResponse, 包装在任务里
        public readonly IRequest Request;
        public readonly ETTask<IResponse> Tcs;
        public RpcInfo(IRequest request) {
            this.Request = request;
            this.Tcs = ETTask<IResponse>.Create(true);
        }
    }

    public sealed class Session {
        private const string TAG = "Session";

        public bool IsDisposed => this.Id == 0;

        public int AcceptTimeout = 5000; // 5 秒
        public int IdleChecker = 2000;   // 2 秒
        public AService AService;
        public Ping ping;

        public readonly Dictionary<int, RpcInfo> requestCallbacks = new Dictionary<int, RpcInfo>();
        public long Id { get; set; }
        public static int RpcId { get; set; }
        public long LastRecvTime { get; set; }
        public long LastSendTime { get; set; }
        public int Error { get; set; }
        public IPEndPoint RemoteAddress { get; set; }

        public Session(long id, AService aService) {
            this.Id = id;
            AService = aService;
            long timeNow = TimeHelper.ClientNow();
            LastRecvTime = timeNow;
            LastSendTime = timeNow;
            requestCallbacks.Clear();
            Debug.Log($"session create, id: {Id} {timeNow} ");
        }
        public void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            AService.RemoveChannel(Id);
            ping?.Dispose();
            ping = null;
            var temp = Id;
            Id = 0;
            foreach (RpcInfo responseCallback in requestCallbacks.Values.ToArray()) { // 所有还在等待的回调：抛错
                responseCallback.Tcs.SetException(new RpcException(Error, $"session dispose: {temp} - {RemoteAddress} -  rpcid={responseCallback.Request.RpcId}"));
            }
            Debug.Log($"session dispose: {RemoteAddress} id: {temp} ErrorCode: {Error}, please see ErrorCode.cs! {TimeHelper.ClientNow()}");
            requestCallbacks.Clear();
        }
        public void OnRead(ushort opcode, IResponse response) {
            OpcodeHelper.LogMsg(0, opcode, response);
            if (!requestCallbacks.TryGetValue(response.RpcId, out var action)) {
                return;
            }
            requestCallbacks.Remove(response.RpcId);
            if (ErrorCore.IsRpcNeedThrowException(response.Error)) {
                action.Tcs.SetException(new Exception($"Rpc error, request: {action.Request} response: {response}"));
                return;
            }
            action.Tcs.SetResult(response); // 就是把异步网络请求任务的结果写回去，返回回去
        }
        public async ETTask<IResponse> Call(IRequest request, ETCancellationToken cancellationToken) {
            int rpcId = ++RpcId;
            RpcInfo rpcInfo = new RpcInfo(request);
            requestCallbacks[rpcId] = rpcInfo;
            request.RpcId = rpcId;
            Send(request);
            void CancelAction() {
                if (!requestCallbacks.TryGetValue(rpcId, out RpcInfo action)) {
                    return;
                }
                requestCallbacks.Remove(rpcId);
                Type responseType = OpcodeManager.GetResponseType(action.Request.GetType());
                IResponse response = (IResponse)Activator.CreateInstance(responseType);
                response.Error = ErrorCore.ERR_Cancel;
                action.Tcs.SetResult(response);
            }
            IResponse ret;
            try {
                cancellationToken?.Add(CancelAction);
                ret = await rpcInfo.Tcs;
            }
            finally {
                cancellationToken?.Remove(CancelAction);
            }
            return ret;
        }
        public async ETTask<IResponse> Call(IRequest request) {
            int rpcId = ++RpcId;
            RpcInfo rpcInfo = new RpcInfo(request); // 这个封装，回答一个问题：网络异步调用中，消息从哪里来，消息到哪里去？把异步调用，真正组装成流式语法，呵呵
            requestCallbacks[rpcId] = rpcInfo; // 这里是，注册回调的意思
            request.RpcId = rpcId; // 标记的是：发消息的信使
            Send(request); // 这里是把消息发出去了，发给相应的服务器
            return await rpcInfo.Tcs; // 等待远程服务器处理，并返回异步结果，要拿结果 
        }
        public void Reply(IResponse message) => Send(0, message);
        public void Send(IMessage message) => Send(0, message);
        public void Send(long actorId, IMessage message) { // 确认了这个方法，ET 里也是这么发的，那么问题仍然是在服务器端
            (ushort opcode, MemoryStream stream) = MessageSerializeHelper.MessageToStream(message);
            OpcodeHelper.LogMsg(0, opcode, message); // 确认过：这里打出来没有问题
            Send(actorId, stream);
        }
        public void Send(long actorId, MemoryStream memoryStream) { // 消息序列化到内存流了，现在就是内存流上发消息了
            LastSendTime = TimeHelper.ClientNow();
            AService.SendStream(Id, actorId, memoryStream); // 从相对顶层的封装服务，向下走，走信道 channel， socket 等。理解一下，内存流上发消息，呵呵
// 把内存流上的消息内容【不曾反序列化，不曾再序列化】写入发送信道缓存区，就把消息转发出去了【整个过程，省下对消息内容的反序列化与再序列化过程】
            // 内网消息：走到信道，读个消息头，一看是内网消息，就【信道发送缓存区上 + 内存流上】更新一下 actorId 【内存流上更新 actorId, 我觉得是指针移动到消息内容头】,
            // 外网消息：不要 actorId, 只更新消息的长短
        }
    }
}