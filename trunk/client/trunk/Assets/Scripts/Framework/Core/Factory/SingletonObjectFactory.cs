using System.Collections.Generic;
using System;

namespace Framework.Core {

    public class SingletonObjectFactory : IObjectFactory {

        private static Dictionary<Type, object> cachedObjects = null;
        private static readonly object _lock = new object();

        private Dictionary<Type, object> CachedObjects {
            get {
                lock (_lock) {
                    if (cachedObjects == null) {
                        cachedObjects = new Dictionary<Type, object>();
                    }
                    return cachedObjects;
                }
            }
        }
        public object AcquireObject(string classFullName) {
            Type type = GameApplication.Instance.HotFix.LoadType(classFullName);
            if (CachedObjects.ContainsKey(type)) {
                return CachedObjects[type];
            }
            lock (_lock) {
                var instance = GameApplication.Instance.HotFix.CreateInstance(classFullName);
                CachedObjects.Add(type, instance);
                return instance;
            }
        }
        public void ReleaseObject(object obj) {
        }
    }
}
