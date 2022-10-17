using Framework.MVVM;

namespace HotFix.UI {

    public class MenuViewModel : ViewModelBase {

        // 这里想要把视图相关数据放入视图模型中来,其它视图类同
        // 可能还是需要把主游戏视图合并为一个相对更大的视图,方便绑定一个ViewModel;
        
        private bool _loadSavedGame;
        private int _gameMode = 0; // 因为其它视图也想要访问这些数据,考虑如何变为全局数据,供其它视图可读访问
        private string _saveGamePathFolderName;
        private int _gridWidth; //= 5;

        // 以前自己不怎么懂得使用设计模式,所以更的是用公用API提供给需要使用的调用者,但实际上就可以实现成观察者模式,数据变更自动通知
        // 改写为观察者模式
        public bool loadSavedGame {
            get {
                return _loadSavedGame;
            }
            set {
                _loadSavedGame = value;
            }
        }
        public int gameMode {
            get {
                return _gameMode;
            }
            set {
                _gameMode = value;
            }
        }
        public string saveGamePathFolderName {
            get {
                return _saveGamePathFolderName;
            }
            set {
                _saveGamePathFolderName = value;
            }
        }
        public int gridWidth {
            get {
                return _gridWidth;
            }
            set {
                _gridWidth = value;
            }
        }

        
        protected override void OnInitialize() {
            base.OnInitialize();
            Initialization();
            DelegateSubscribe();
        }

        void Initialization() {
            _gameMode = 0;
            _loadSavedGame = false;
            _saveGamePathFolderName = "";
            _gridWidth = -1;
        }

        void DelegateSubscribe() {
        }
    }
}
