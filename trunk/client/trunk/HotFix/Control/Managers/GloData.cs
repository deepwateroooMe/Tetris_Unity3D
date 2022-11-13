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

        private int _gridSize = 5;
        private int _gridXSize = 9;
        private int _gridZSize = 9;

        private int _tetroCnter = 0;
        private int _challengeLevel = 0;
        private int _gameLevel = -1;

        public BindableProperty<bool> gameStarted = new BindableProperty<bool>();
        public int layerScore = 9170;
        public int challengeLayerScore = 16700;

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
                onGameModeSelected(_gameMode);
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
        
        private void onGameModeSelected(int gameMode) {
            _gameMode = gameMode;
            switch (gameMode) {
            case 0:
                _saveGamePathFolderName = "educational/grid";
                break;
            case 1: // 因为经典模式下也是有层级的
                _saveGamePathFolderName = "classic/level";
                break;
            case 2:
                isChallengeMode = true;
                _saveGamePathFolderName = "challenge/level";
                break;
            }
        }

        public string getFilePath() {
            StringBuilder path = new StringBuilder();
            if (gameMode > 0) 
                path.Append(Application.persistentDataPath + "/" + _saveGamePathFolderName
                            + _gameLevel + "/game.save"); 
            else 
                path.Append(Application.persistentDataPath + "/" + _saveGamePathFolderName 
                            + (isChallengeMode ? _gameLevel.ToString() : _gridSize.ToString())
                            + "/game.save");
            Debug.Log(TAG + " getFilePath() path.ToString(): " + path.ToString());
            return path.ToString();
        }
    }
}