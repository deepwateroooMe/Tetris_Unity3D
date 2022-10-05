using ILRuntime.CLR.TypeSystem;
using System;
using Framework.Util;

namespace Framework.Core {
    public class TransientObjectFactory : IObjectFactory {
        public object AcquireObject(string classFullName) {
            var instance = GameApplication.Instance.HotFix.CreateInstance(classFullName);
            return instance;
        }
        public void ReleaseObject(object obj) {
        }
    }
}
