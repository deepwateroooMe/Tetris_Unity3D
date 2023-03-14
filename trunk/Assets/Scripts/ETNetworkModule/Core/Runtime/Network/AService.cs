using System;
using System.IO;
using System.Net;
namespace ET {
    public abstract class AService : IDisposable { // 服务的抽象基类： 

        public ServiceType ServiceType { get; protected set; } // 内网消息，外网消息
// 这里，从最大，最小值，就是让他们尽量不要交叉         
        private long connectIdGenerater = int.MaxValue;
        public long CreateConnectChannelId(uint localConn) => (--this.connectIdGenerater << 32) | localConn; // localConn放在低32bit
        public uint CreateRandomLocalConn() => (1u << 30) | RandomHelper.RandUInt32();
        private long acceptIdGenerater = 1;
        public long CreateAcceptChannelId(uint localConn) => (++this.acceptIdGenerater << 32) | localConn; // localConn放在低32bit

        public abstract void Update();
        public abstract void Remove(long id);
        public abstract bool IsDispose();
        protected abstract void Get(long id, IPEndPoint address);
        public abstract void Dispose();
        protected abstract void Send(long channelId, long actorId, MemoryStream stream);

        protected void OnAccept(long channelId, IPEndPoint ipEndPoint) => this.AcceptCallback.Invoke(channelId, ipEndPoint);
        public void OnRead(long channelId, MemoryStream memoryStream) => this.ReadCallback.Invoke(channelId, memoryStream);
        public void OnError(long channelId, int e) {
            this.Remove(channelId);
            this.ErrorCallback?.Invoke(channelId, e);
        }
        public Action<long, IPEndPoint> AcceptCallback;
        public Action<long, int> ErrorCallback;
        public Action<long, MemoryStream> ReadCallback;

// 下面的四个，或是项目中某处某些处引用过这些方法，搭个桥，让他们连通，没实际意义 
        public void Destroy() => this.Dispose(); // 这个加得狠奇怪，可以去掉
        public void SendStream(long channelId, long actorId, MemoryStream stream) => this.Send(channelId, actorId, stream); // 这个方法：没有任何意义，只为适配？
        public void GetOrCreate(long id, IPEndPoint address) => this.Get(id, address);
        public void RemoveChannel(long channelId) => this.Remove(channelId);
    }
}