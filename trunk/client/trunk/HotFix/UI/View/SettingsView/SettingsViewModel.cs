using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Framework.MVVM;
using HotFix.Control;
using UnityEngine;

namespace HotFix.UI {
    public class SettingsViewModel : ViewModelBase {
        private const string TAG = "SettingsViewModel";

        private int curVol;
        private int maxVol;
        
        protected override void OnInitialize() {
            base.OnInitialize();
            Initialization();
            DelegateSubscribe(); 
        }

        void Initialization() {
            curVol = 0;
            maxVol = 100;
            // if (VolumeManager.Instance.maxVol == null) {
            //     VolumeManager.Instance.maxVol = new BindableProperty<int>();
            //     VolumeManager.Instance.maxVol.Value = 0;
            // }
            // if (VolumeManager.Instance.curVol == null) {
            //     VolumeManager.Instance.curVol = new BindableProperty<int>();
            //     VolumeManager.Instance.curVol.Value = -1;
            // }
            // VolumeManager.Instance.maxVol.OnValueChanged += onMaxVolChanged;
            // VolumeManager.Instance.curVol.OnValueChanged += onCurVolChanged;
        }

        void DelegateSubscribe() { // <<<<<<<<<<<<<<<<<<<< 它的这个原本的设计我好像没有用上,需要再想一下它这么设计的原理
        }
        void onMaxVolChanged(int pre, int cur) {
            Debug.Log(TAG + " onMaxVolChanged() cur: " + cur);
            maxVol = cur;
        }
        void onCurVolChanged(int pre, int cur) {
            Debug.Log(TAG + " onCurVolChanged() cur: " + cur);
            curVol = cur;
        }
    }
}
// tac log1.log | awk '!flag; /09:37:00.00/{flag = 1};' | tac > cur.log
