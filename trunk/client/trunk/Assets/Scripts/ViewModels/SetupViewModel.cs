using System;
using Assets.Sources.Core.Message;
using Assets.Sources.Infrastructure;
using uMVVM.Sources.Infrastructure;
using uMVVM.Sources.Models;
using Debug = UnityEngine.Debug;

namespace uMVVM.Sources.ViewModels {
    
    public class SetupViewModel : ViewModelBase {
        private const string TAG = "SetupViewModel";

        public readonly BindableProperty<string> Educational = new BindableProperty<string>();
        
        public void onEducationalMode () {
            Debug.Log(TAG + ": onEducationalMode()"); 
            MessageAggregator<object>.Instance.Publish("Toggle", this, new MessageArgs<object>("Yellow"));
            Debug.Log(TAG + " ldfkjhsdklfj: " + Educational);
            
            // GloData.Instance.saveGamePathFolderName = "educational";
            // GloData.Instance.gameMode = 0;
            // easyModeGridSizePanel.SetActive(true);
        }

        // public void JoininCurrentTeam() {
        //     MessageAggregator<object>.Instance.Publish("Toggle", this,new MessageArgs<object>("Red"));
        //     Debug.Log(Name.Value + "加入当前Team，职业："+Job.Value+",攻击力："+ATK.Value+"成功率："+SuccessRate.Value);
        // }
        // public void JoininClan() {
        //     MessageAggregator<object>.Instance.Publish("Toggle", this, new MessageArgs<object>("Yellow"));
        //     Debug.Log(Name.Value + "加入当前Clan，职业：" + Job.Value + ",攻击力：" + ATK.Value + "成功率：" + SuccessRate.Value);
        // }

    }
}
