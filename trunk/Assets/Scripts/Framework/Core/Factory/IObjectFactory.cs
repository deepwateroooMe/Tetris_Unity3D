using System;
namespace Framework.Core {

    public interface IObjectFactory {
        object AcquireObject(string classFullName);
        void ReleaseObject(object obj);
    }
}
