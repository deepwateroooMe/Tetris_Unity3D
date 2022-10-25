using System;

namespace Framework.Core {

// 这个桥梁: 通过这个接口衔接了两个不同的程序域,接口适配
    public interface IHotFixMain {

        Type LoadType(string typeName);
        object CreateInstance(string typeName);
    }
}
