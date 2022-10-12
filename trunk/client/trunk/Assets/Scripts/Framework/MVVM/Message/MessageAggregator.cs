using System.Collections.Generic;
using Framework.Util;
using System;

namespace Framework.MVVM {

// 使用泛型: 对于某一个特定的属性T 来说,的 单例模式 管理器
// 感觉偈是自己先前写的EventManager.cs类:
// 负责某一单一可观察者属性:的不同方法(事件的)回调 ?
// 还没有找到项目里实际逻辑运行到它的地方,这里需要再深入理解一下
//     
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
