using Framework.Util;
using System.Collections.Generic;

namespace HotFix.Control.Edit {

    // 编辑管理器
    public class EditManager {

        private static EditManager _instance;
        public static EditManager Instance {
            get {
                if (_instance == null) {
                    _instance = new EditManager();
                }
                return _instance;
            }
        }

// 所以这里是命令式自上往下传递:自UI往下传递
#region Command

        // 因为这里赋予了用户无限撤销与重做机会,所以使用两个栈来管理最近的操作命令列表,方便随时撤销或重做,与自己写的第一个应用的这两个功能同样的原理
        Stack<IEditCommand> editCommandStack = new Stack<IEditCommand>();      // 当前命令栈: 执行过的命令存储栈
        Stack<IEditCommand> editCommandCacheStack = new Stack<IEditCommand>(); // 备用命令栈: 撤销过的命令存储栈

        // 执行编辑命令
        public void DoEditCommand(IEditCommand command) {
            bool result = command.Do();
            if (result) 
                editCommandStack.Push(command);
        }

        // 取消上一个编辑命令
        public void UndoLastEditCommand() {
            if (editCommandStack.Count > 0) { // 若有执行过的命令,撤销
                IEditCommand editCommand = editCommandStack.Pop();
                editCommand.Undo();
                editCommandCacheStack.Push(editCommand); // 撤销的最后一个命令加入撤销栈
            } else 
                DebugHelper.Log("editCommandStack is empty");
        }

        // 重新执行上一个命令
        public void RedoLastEditCommand() {
            if (editCommandCacheStack.Count > 0) {
                IEditCommand editCommand = editCommandCacheStack.Pop();
                bool result = editCommand.Do();
                if (result) {
                    editCommandStack.Push(editCommand);
                }
            } else {
                DebugHelper.Log("editCommandCacheStack is empty");
            }
        }

        // 清空编辑命令
        public void EditCommandClear() {
            editCommandStack.Clear();
            editCommandCacheStack.Clear();
        }
#endregion
    }
}
