﻿using System.IO;
using System.Text;
using Framework.MVVM;
using HotFix.Control;
using UnityEngine;

namespace HotFix.UI {

    public class MenuViewModel : ViewModelBase {
        private const string TAG = "MenuViewModel";

        // private bool _loadSavedGame;
        // private int _gameMode = 0; 
        // public BindableProperty<int> mgameMode = new BindableProperty<int>();
        // public BindableProperty<bool> loadGame = new BindableProperty<bool>();

        private int _gridSize; 
        // private bool _isChallengeMode = false;
        
        // public bool loadSavedGame {
        //     get {
        //         return _loadSavedGame;
        //     }
        //     set {
        //         _loadSavedGame = value;
        //         GloData.Instance.loadSavedGame = true;
        //     }
        // }
        // public bool isChallengeMode {
        //     get {
        //         return _isChallengeMode;
        //     }
        //     set {
        //         _isChallengeMode = value;
        //         GloData.Instance.isChallengeMode = true;
        //     }
        // }

        public int gridSize {
            get {
                return _gridSize;
            }
            set {
                _gridSize = value;
                // onGridWithSet(_gridSize);
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
                GloData.Instance.gridSize.Value = 3;
                GloData.Instance.gridXSize = 3;
                GloData.Instance.gridZSize = 3;
                break;
            case 4:
                GloData.Instance.gridSize.Value = 4;
                GloData.Instance.gridXSize = 4;
                GloData.Instance.gridZSize = 4;
                break;
            case 5:
                GloData.Instance.gridSize.Value = 5;
                GloData.Instance.gridXSize = 5;
                GloData.Instance.gridZSize = 5;
                break;
            }
        }
        
        void Initialization() {
            Debug.Log(TAG + " Initialization()");
            //_gameMode = -1;
            //_loadSavedGame = false;
            //_gridSize = -1;
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
