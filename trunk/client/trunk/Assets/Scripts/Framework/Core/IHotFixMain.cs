using System;

namespace Framework.Core {
    public interface IHotFixMain {
        Type LoadType(string typeName);
        object CreateInstance(string typeName);
    }
}
