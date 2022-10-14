using System.Collections.Generic;
using System;
using System.Reflection;
using Framework.Core;

namespace Framework.MVVM {

    // 特型:特指ViewModelBase视图模型的绑定
    public class PropertyBinder<ViewModelBase> {

        // 代理模式:
        private delegate void BindHandler(ViewModelBase viewModel);
        private delegate void UnBindHandler(ViewModelBase viewModel);

        // 管理者说: 对于某一个特定的属性TProperty, 可以有多个绑定者,也可以有多少取消了绑定的
        // 想一想:为什么取消了绑定的属性变化监听回调函数,这里还需要用一个链表把他们保存起来?有什么必要,作用是什么?
        private readonly List<BindHandler> binders = new List<BindHandler>();
        private readonly List<UnBindHandler> unBinders = new List<UnBindHandler>(); // <<<<<<<<<<<<<<<<<<<< ???

        public void Add<TProperty>(string name, string realTypeName,
                                   Action<TProperty, TProperty> valueChangedHandler) {


// ILRuntime有一套他自己独特的类型机制,是世外桃源,并不为Unity/C#系统通过反射来识别
    // 在热更DLL当中，直接调用Type.GetType(“TypeName”)或者typeof(TypeName)均可以得到有效System.Type类型实例
    // 在Unity主工程中，无法通过Type.GetType来取得热更DLL内部定义的类                
// ILRuntime额外实现了几个用于反射的辅助类：
    // ILRuntimeType，ILRuntimeMethodInfo，ILRuntimeFieldInfo等，来模拟系统的类型来提供部分反射功能
// 所以热更新程序集里(ILRuntime里)需要有套机制(一个字典?)来负责两个域里不同类型的来回切换           
            var fieldInfo = GameApplication.Instance.HotFix.LoadType(realTypeName)
                .GetField(name, BindingFlags.Instance | BindingFlags.Public);
            
            if (fieldInfo == null) 
                throw new Exception(string.Format("Unable to find bindableproperty field '{0}.{1}'", realTypeName, name));
            
            binders.Add(viewModel => {
                GetPropertyValue<TProperty>(name, viewModel, realTypeName, fieldInfo).OnValueChanged += valueChangedHandler;
            });
            unBinders.Add(viewModel => {
                GetPropertyValue<TProperty>(name, viewModel, realTypeName, fieldInfo).OnValueChanged -= valueChangedHandler;
            });
        }
        private BindableProperty<TProperty> GetPropertyValue<TProperty>(string name, ViewModelBase viewModel, string realTypeName, FieldInfo fieldInfo) {
            var value = fieldInfo.GetValue(viewModel);
            BindableProperty<TProperty> bindableProperty = value as BindableProperty<TProperty>;
            if (bindableProperty == null) 
                throw new Exception(string.Format("Illegal bindableproperty field '{0}.{1}' ", realTypeName, name));
            return bindableProperty;
        }
        
        public void Bind(ViewModelBase viewModel) {
            if (viewModel != null) // 广告天下: 对所有的订阅监听者发布通知说,现在绑定的是viewModel, 一一触发监听回调  
                for (int i = 0; i < binders.Count; i++) 
                    binders[i](viewModel);
        }
        public void UnBind(ViewModelBase viewModel) {
            if (viewModel != null) // 广告天下: 对所有取消的监听者发布通知说,要求解绑当前viewModel, 一一触发取消订阅回调  
                for (int i = 0; i < unBinders.Count; i++) 
                    unBinders[i](viewModel);
        }
    }
}
