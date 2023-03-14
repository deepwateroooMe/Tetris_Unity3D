using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
namespace ET {

    [DefaultExecutionOrder(-97)] 
// 那么也就意味着：客户端与服务器的代码重复；这个类，或是模块，服务器端会有非 MonoBehaviour 的模块
// 上面的考虑是对的：这个客户端是可以直接与ET 框架通信的
    public class NetKcpComponent : MonoBehaviour { // 直接定义为组件：继承自 MonoBehaviour, 是客户端是这样的逻辑。
        public AService Service;
        int sessionStreamDispatcherType;
        private Dictionary<long, Session> sessions;
        
// 这个网络模块的组件：它也是需要有必要的初始化的。它的初始化，就是配置到能够上下文同步
        private void Awake() => Init(SessionStreamDispatcherType.SessionStreamDispatcherClientOuter); // 定义为，客户端
        private void Init(int sessionStreamDispatcherType) { // 组件的类型：脚本加载的时候，初始化用这个方法 
            this.sessionStreamDispatcherType = sessionStreamDispatcherType;
            Service = new TService(ServiceType.Outer); // 注意一下：这里说服务类型是，对外的
            Service.ErrorCallback += (channelId, error) => OnError(channelId, error);
            Service.ReadCallback += (channelId, Memory) => OnRead(channelId, Memory);
            NetServices.Add(Service);
            Debug.Log($"{nameof(NetKcpComponent)}:  {Service.GetType().Name}");
        }
        public void Init(IPEndPoint address, int sessionStreamDispatcherType) {
            this.sessionStreamDispatcherType = sessionStreamDispatcherType;
            Service = new TService(address, ServiceType.Outer);
            Service.ErrorCallback += (channelId, error) => OnError(channelId, error);
            Service.ReadCallback += (channelId, Memory) => OnRead(channelId, Memory);
            Service.AcceptCallback += (channelId, IPAddress) => OnAccept(channelId, IPAddress);
            NetServices.Add(Service);
        }
        public void OnDestroy() {
            NetServices.Remove(Service);
            Service.Destroy();
        }
        private void Update() => Service?.Update();
        public void OnRead(long channelId, MemoryStream memoryStream) {
            Session session = GetSession(channelId);
            if (session == null) {
                return;
            }
            session.LastRecvTime = TimeHelper.ClientNow();
            SessionStreamDispatcherManager.Dispatch(sessionStreamDispatcherType, session, memoryStream);
        }
        public void OnError(long channelId, int error) {
            Session session = GetSession(channelId);
            if (session == null) {
                return;
            }
            session.Error = error;
            session.Dispose();
        }
        // 这个channelId是由CreateAcceptChannelId生成的
        public void OnAccept(long channelId, IPEndPoint ipEndPoint) {
            Session session = AddChildWithId(channelId, Service);
            session.RemoteAddress = ipEndPoint;
            // 挂上这个组件，5秒就会删除session，所以客户端验证完成要删除这个组件。该组件的作用就是防止外挂一直连接不发消息也不进行权限验证
            // session.AddComponent<SessionAcceptTimeoutComponent>();
            // 客户端连接，2秒检查一次recv消息，10秒没有消息则断开
            // session.AddComponent<SessionIdleCheckerComponent, int>(NetThreadComponent.checkInteral);
        }
// 下面：原本封装在最底层的系统最基层方法，被抽出来放在每个模块下。为什么我还是觉得 ET 框架的封装更好呢？
        private Session AddChildWithId(long channelId, AService service) {
            sessions ??= new Dictionary<long, Session>();
            sessions.TryGetValue(channelId, out var session);
            if (session == null) {
                session = new Session(channelId, service);
                sessions.Add(channelId, session); // 自己，管理字典的维护
            }
            return session;
        }
        public Session Create(IPEndPoint realIPEndPoint) {
            long channelId = RandomHelper.RandInt64(); // 这里是，随时生成的
            Session session = AddChildWithId(channelId, Service); // 脚本初始化的时候，也会开启一个对外的服务
            session.RemoteAddress = realIPEndPoint; // 远程服务器的地址
            session.IdleChecker = NetServices.checkInteral;
            Service.GetOrCreate(session.Id, realIPEndPoint); // 去拿，或是创建一个，到这个远程服务器的会话框 
            return session;
        }
        public Session GetSession(long id) {
            sessions ??= new Dictionary<long, Session>();
            sessions.TryGetValue(id, out var session);
            return session;
        }
    }
}