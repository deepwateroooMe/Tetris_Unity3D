using System;
using System.Collections.Generic;
namespace Framework.Util {

    public interface ILogFile {

        Action<string> OnWriteLine { get; set; }

        void CloseFile(bool zipOld=false);

        void Write(string FormatString, params object[] args);

        Queue<string> Cache {
            get;
            set;
        }
    }
}
