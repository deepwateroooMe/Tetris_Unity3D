using System.IO;
using System.Text;
using Framework.MVVM;
using HotFix.Control;
using UnityEngine;

namespace HotFix.UI {

    public class MenuViewModel : ViewModelBase {
        private const string TAG = "MenuViewModel";

        private bool _loadSavedGame;
        private int _gameMode = 0; 
        public BindableProperty<int> mgameMode = new BindableProperty<int>();
        public BindableProperty<bool> loadGame = new BindableProperty<bool>();

        private int _gridSize; 
        private bool _isChallengeMode = false;
        
        public bool loadSavedGame {
            get {
                return _loadSavedGame;
            }
            set {
                _loadSavedGame = value;
            }
        }
        public bool isChallengeMode {
            get {
                return _isChallengeMode;
            }
            set {
                _isChallengeMode = value;
            }
        }
        public int gameMode {
            get {
                return _gameMode;
            }
            set {
                _gameMode = value;
                GloData.Instance.gameMode = _gameMode;
                // mgameMode.Value = value;
            }
        }
        public int gridWidth {
            get {
                return _gridSize;
            }
            set {
                _gridSize = value;
                onGridWithSet(_gridSize);
            }
        }

        
        protected override void OnInitialize() {
            base.OnInitialize();
            Initialization();
            DelegateSubscribe();
        }

        private void onGridWithSet(int width) {
            switch (width) {
            case 3:
                GloData.Instance.gridSize = 3;
                GloData.Instance.gridXSize = 3;
                GloData.Instance.gridZSize = 3;
                break;
            case 4:
                GloData.Instance.gridSize = 4;
                GloData.Instance.gridXSize = 4;
                GloData.Instance.gridZSize = 4;
                break;
            case 5:
                GloData.Instance.gridSize = 5;
                GloData.Instance.gridXSize = 5;
                GloData.Instance.gridZSize = 5;
                break;
            }
        }
        
        void Initialization() {
            _gameMode = 0;
            _loadSavedGame = false;
            _gridSize = -1;
            mgameMode.Value = -1;
        }
        void DelegateSubscribe() {
        }

        // public void onLogin() {
        //     LoadScene("Signin");
        // }

        public void onLogout() {
            
        }

        public void getCredit() {
            
        }
        
        public void rateTheGame() {
            
        }
        
        public void toggleSounds() {
            
        }
        
        public void getAdsFree() {
            
        }

        public void toggleSettings() {
            
        }
    }
}
