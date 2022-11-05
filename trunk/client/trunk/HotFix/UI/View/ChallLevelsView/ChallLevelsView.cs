using Framework.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UI {

    public class ChallLevelsView : UnityGuiView {
        private const string TAG = "ChallLevelsView"; 
        public override string BundleName { get { return "ui/view/challlevelsview"; } }
        public override string AssetName { get { return "ChallLevelsView"; } }
        public override string ViewName { get { return "ChallLevelsView"; } }
        public override string ViewModelTypeName { get { return typeof(ChallLevelsViewModel).FullName; } }
        public ChallLevelsViewModel ViewModel { get { return (ChallLevelsViewModel)BindingContext; } }

        GameObject basicPanel;
        Button rightBtn;
        Button oneBtn;
        Button twoBtn;
        Button thrBtn;
        Button forBtn;
        Button fivBtn;
        Button sixBtn;
        Button sevBtn;
        Button eitBtn;
        Button ninBtn;
        Button tenBtn;

        GameObject advancedPanel;
        Button leftBtn;
        Button eleBtn;
        Button twlBtn;
        Button thtBtn;
        Button fotBtn;
        Button fifBtn;
        Button sitBtn;
        Button svtBtn;
        Button attBtn;
        Button nitBtn;
        Button twtBtn;
        
        protected override void OnInitialize() {
            base.OnInitialize();

            basicPanel = GameObject.FindChildByName("basicPanel");
            rightBtn = GameObject.FindChildByName("right").GetComponent<Button>();
			rightBtn.onClick.AddListener(OnClickRightButton);

            oneBtn = GameObject.FindChildByName("oneBtn").GetComponent<Button>();
            oneBtn.onClick.AddListener(OnClickOneButton);
            twoBtn = GameObject.FindChildByName("twoBtn").GetComponent<Button>();
            twoBtn.onClick.AddListener(OnClickTwoButton);
            thrBtn = GameObject.FindChildByName("thrBtn").GetComponent<Button>();
            thrBtn.onClick.AddListener(OnClickThrButton);
            forBtn = GameObject.FindChildByName("forBtn").GetComponent<Button>();
            forBtn.onClick.AddListener(OnClickForButton);
            fivBtn = GameObject.FindChildByName("fivBtn").GetComponent<Button>();
            fivBtn.onClick.AddListener(OnClickFivButton);

            sixBtn = GameObject.FindChildByName("sixBtn").GetComponent<Button>();
            sixBtn.onClick.AddListener(OnClickSixButton);
            sevBtn = GameObject.FindChildByName("sevBtn").GetComponent<Button>();
            sevBtn.onClick.AddListener(OnClickSevButton);
            eitBtn = GameObject.FindChildByName("eitBtn").GetComponent<Button>();
            eitBtn.onClick.AddListener(OnClickEitButton);
            ninBtn = GameObject.FindChildByName("ninBtn").GetComponent<Button>();
            ninBtn.onClick.AddListener(OnClickNinButton);
            tenBtn = GameObject.FindChildByName("tenBtn").GetComponent<Button>();
            tenBtn.onClick.AddListener(OnClickTenButton);

            advancedPanel = GameObject.FindChildByName("advancedPanel");
            leftBtn = GameObject.FindChildByName("left").GetComponent<Button>();
            leftBtn.onClick.AddListener(OnClickLeftButton);

            eleBtn = GameObject.FindChildByName("eleBtn").GetComponent<Button>();
            eleBtn.onClick.AddListener(OnClickEleButton);
            twlBtn = GameObject.FindChildByName("twlBtn").GetComponent<Button>();
            // twlBtn.onClick.AddListener(OnClickTwlButton);
            thtBtn = GameObject.FindChildByName("thtBtn").GetComponent<Button>();
            // thtBtn.onClick.AddListener(OnClickThtButton);
            fotBtn = GameObject.FindChildByName("fotBtn").GetComponent<Button>();
            // fotBtn.onClick.AddListener(OnClickFotButton);
            fifBtn = GameObject.FindChildByName("fifBtn").GetComponent<Button>();
            // fifBtn.onClick.AddListener(OnClickFifButton);

            sitBtn = GameObject.FindChildByName("sitBtn").GetComponent<Button>();
            // sitBtn.onClick.AddListener(OnClickSitButton);
            svtBtn = GameObject.FindChildByName("svtBtn").GetComponent<Button>();
            // svtBtn.onClick.AddListener(OnClickSvtButton);
            attBtn = GameObject.FindChildByName("attBtn").GetComponent<Button>();
            // attBtn.onClick.AddListener(OnClickAttButton);
            nitBtn = GameObject.FindChildByName("nitBtn").GetComponent<Button>();
            // nitBtn.onClick.AddListener(OnClickNitButton);
            twtBtn = GameObject.FindChildByName("twtBtn").GetComponent<Button>();
            // twtBtn.onClick.AddListener(OnClickTwtButton);
        }

        void OnClickRightButton() {
            basicPanel.SetActive(false);
            advancedPanel.SetActive(true);
        }        
        void OnClickLeftButton() {
            advancedPanel.SetActive(false);
            basicPanel.SetActive(true);
        }        
        void OnClickOneButton() {
            ViewManager.GameView.Reveal();
            Hide();
        }
        void OnClickTwoButton() {
        }
        void OnClickThrButton() {
        }
        void OnClickForButton() {
        }
        void OnClickFivButton() {
        }
        void OnClickSixButton() {
        }
        void OnClickSevButton() {
        }
        void OnClickEitButton() {
        }
        void OnClickNinButton() {
        }
        void OnClickTenButton() {
        }
        void OnClickEleButton() {
        }
    }
}