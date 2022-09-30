using System;
using System.Runtime;
using System.ComponentModel;

namespace tetris3d {

    // https://www.cnblogs.com/lemontea/archive/2011/11/26/2264168.html
    public class PropertyChangedEventArgs : EventArgs {
    
        // 摘要:初始化 System.ComponentModel.PropertyChangedEventArgs 类的新实例。
        // 参数:propertyName:已更改的属性名
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        // public PropertyChangedEventArgs(string propertyName) ;
        public PropertyChangedEventArgs(string propertyName) {
            this.PropertyName = propertyName;
        }

        // 摘要:获取已更改的属性名。
        // 返回结果:已更改的属性名。
        // public virtual string PropertyName { get; }        
        public string PropertyName { get; }        
    
    }
}