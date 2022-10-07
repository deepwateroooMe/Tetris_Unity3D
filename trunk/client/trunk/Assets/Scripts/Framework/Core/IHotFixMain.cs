using System;

namespace Framework.Core {
// 这个只是在unity的程序域里运行
    public interface IHotFixMain {
        Type LoadType(string typeName);
        object CreateInstance(string typeName);
    }
}
