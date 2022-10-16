namespace deepwaterooo.tetris3d {

    // 这里需要理解的是: 当我把这些类型的脚本,或是接下来的Tetromino.cs GhostTetromino.cs等脚本直接加在预设中的时候
    // 其实也是一种认定: 这些脚本已经定型,不会参与(将来不会被)热更新(也不能这么说, 可以热更新资源包预设包,预设里的脚本便同样跟着改变了?)
    // 同样也还是可以热更新的,但可能更多的会是一种受到一定限制的热更新?
    // 实际上也是可以在热更新的程序包里实例化的时候来设定,或是序列化的时候,或是怎么配置一下,方便将来修改?
    public interface IType {
        string type { get; set; }
    }    
}
