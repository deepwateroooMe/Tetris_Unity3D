using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control {

// 添加这些类,以便更快地适配源代码完成项目.到游戏项目的后期,等对项目源码更为熟悉之后会对源码进行必要的整理和优化
    public class GloData : Singleton<GloData> {
        private const string TAG = "GloData";
        
        private bool _loadSavedGame = false;
        
        private bool _isChallengeMode = false;
        private int _gameMode = 0;
        private string _saveGamePathFolderName;
// 想写成 BindableProperty的形式,晚些时候再改
        private int _gridSize = 5;
        private int _gridXSize = 9;
        private int _gridZSize = 9;

        private int _tetroCnter = 0;
        private int _challengeLevel = 0;
        private int _gameLevel = -1;

        public BindableProperty<bool> gameStarted = new BindableProperty<bool>();
        public int layerScore = 9170;
        public int challengeLayerScore = 16700;
        
        // void OnEnable() { // <<<<<<<<<<<<<<<<<<<< 怎么会有这个方法呢,没有必要的呀
        //     Debug.Log(TAG + ": OnEnable()");
        //     _gameMode = 0;
        //     _loadSavedGame = false;
        //     _isChallengeMode = false;
        //     _saveGamePathFolderName = "";
        //     _gridSize = 5; //-1
        //     _tetroCnter = 0;
        // }

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
            }
        }
        public int gameLevel {
            get {
                return _gameLevel;
            }
            set {
                _gameLevel = value;
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
        public int gridSize {
            get {
                return _gridSize;
            }
            set {
                _gridSize = value;
            }
        }
        public int gridXSize {
            get {
                return _gridXSize;
            }
            set {
                _gridXSize = value;
            }
        }
        public int gridZSize {
            get {
                return _gridZSize;
            }
            set {
                _gridZSize = value;
            }
        }
        public int tetroCnter {
            get {
                return _tetroCnter;
            }
            set {
                _tetroCnter = value;
            }
        }
        public int challengeLevel {
            get {
                return _challengeLevel;
            }
            set {
                _challengeLevel = value;
            }
        }
    }
}