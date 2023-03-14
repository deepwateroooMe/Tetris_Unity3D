namespace ET {
    // 把这个，加入标签系统
    public class SessionStreamDispatcherAttribute : BaseAttribute {
        public int Type;
        public SessionStreamDispatcherAttribute(int type) => this.Type = type;
    }
}