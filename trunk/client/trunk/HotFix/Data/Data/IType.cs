using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotFix.Data.Data {
 
    public interface IType { // 这个可能不用了? 直接Enum 不是也很方便吗?
        string type { get; set; } // string ==> int 换一下好处理 instatnceId later
    }     
}
