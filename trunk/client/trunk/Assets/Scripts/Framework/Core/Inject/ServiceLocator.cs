using System.Collections.Generic;
using System;
namespace Framework.Core {
    public class ServiceLocatorContain {
        public string TypeName {
            get;
            private set;
        }
        public Func<object> Function {
            get;
            set;
        }
        public ServiceLocatorContain(string name, Func<object> func) {
            TypeName = name;
            Function = func;
        }
    }
    public class ServiceLocator {
        private SingletonObjectFactory _singletonObjectFactory = new SingletonObjectFactory();
        private TransientObjectFactory _transientObjectFactory = new TransientObjectFactory();
        private static readonly Dictionary<Type, ServiceLocatorContain> Container = new Dictionary<Type, ServiceLocatorContain>();
        public void RegisterSingleton(string interfaceName, string typeName) {
            ServiceLocatorContain contain = new ServiceLocatorContain(typeName, Lazy(FactoryType.Singleton, typeName));
            Type type = GameApplication.Instance.HotFix.LoadType(interfaceName);
            if (!Container.ContainsKey(type)) {
                Container.Add(type, contain);
            } else {
                throw new Exception("Container contains key: " + type);
            }
        }
        public void RegisterSingleton(string typeName) {
            ServiceLocatorContain contain = new ServiceLocatorContain(typeName, Lazy(FactoryType.Singleton, typeName));
            Type type = GameApplication.Instance.HotFix.LoadType(typeName);
            if (!Container.ContainsKey(type)) {
                Container.Add(type, contain);
            } else {
                throw new Exception("Container contains key: " + type);
            }
        }
        public void RegisterTransient(string interfaceName, string typeName) {
            ServiceLocatorContain contain = new ServiceLocatorContain(typeName, Lazy(FactoryType.Transient, typeName));
            Type type = GameApplication.Instance.HotFix.LoadType(interfaceName);
            if (!Container.ContainsKey(type)) {
                Container.Add(type, contain);
            } else {
                throw new Exception("Container contains key: " + type);
            }
        }
        public void RegisterTransient(string typeName) {
            ServiceLocatorContain contain = new ServiceLocatorContain(typeName, Lazy(FactoryType.Transient, typeName));
            Type type = GameApplication.Instance.HotFix.LoadType(typeName);
            if (!Container.ContainsKey(type)) {
                Container.Add(type, contain);
            } else {
                throw new Exception("Container contains key: " + type);
            }
        }
        public void Clear() {
            Container.Clear();
        }
        public TInterface Resolve<TInterface>(string keyName) where TInterface : class {
            return Resolve(GameApplication.Instance.HotFix.LoadType(keyName)) as TInterface;
        }
        private static object Resolve(Type type) {
            if (!Container.ContainsKey(type)) {
                return null;
            }
            return Container[type].Function();
        }
        private Func<object> Lazy(FactoryType factoryType, string typeFullName) {
            return () => {
                switch (factoryType) {
                case FactoryType.Singleton:
                    return _singletonObjectFactory.AcquireObject(typeFullName);
                default:
                    return _transientObjectFactory.AcquireObject(typeFullName);
                }
            };
        }
    }
}