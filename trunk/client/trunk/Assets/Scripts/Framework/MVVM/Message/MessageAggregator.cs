using System.Collections.Generic;
using Framework.Util;
using System;

namespace Framework.MVVM {

// 我现在把它理解为像安卓handler机制里的消息桶;但是这里更多的是对可观察可监听数据的订阅取消监听回调的这么一个全局唯一管理类
// 自己的小游戏中,事件代理等是否会太多,有没有串口的性能限制呢?这里还需要再多想一想
    public class MessageAggregator<T> : Singleton<MessageAggregator<T>> {
        
        private readonly Dictionary<string, Action<object, MessageArgs<T>>> messages
            = new Dictionary<string, Action<object, MessageArgs<T>>>();

        public void Subscribe(string name, Action<object, MessageArgs<T>> handler) {
            if (!messages.ContainsKey(name)) {
                messages.Add(name, handler);
            } else 
                messages[name] += handler;
        }

        public void Publish(string name, object sender, MessageArgs<T> args) {
            if (messages.ContainsKey(name) && messages[name] != null) 
                messages[name](sender, args);
        }
    }
}
