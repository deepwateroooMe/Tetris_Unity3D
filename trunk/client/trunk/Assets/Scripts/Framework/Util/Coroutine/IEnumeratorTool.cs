using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework.Util {

    public class IEnumeratorTool : MonoBehaviourInstance<IEnumeratorTool> {
        private const string TAG = "IEnumeratorTool"; 

        WaitForSeconds m_waitForOneSecond = new WaitForSeconds(1.0f);

        public WaitForSeconds waitForOneSecond {
            get {
                return m_waitForOneSecond;
            }
        }
    }
}
