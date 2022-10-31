// 暂时添加了这个模块: 主要是为自己提供多一个视角来看待热更新过程中的点击事件回调等的命令式传递,传递方向?:UI向下向逻辑层控制层MVC MVP ?
namespace HotFix.Control.Edit {

    // 编辑命令接口
    public interface IEditCommand {

        // 执行
        bool Do();

        // 撤销
        void Undo();
    }
}
