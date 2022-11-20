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
        
        public int layerScore = 9170;
        public int challengeLayerScore = 16700;
        public int maxXWidth = 10;
        public int maxZWidth = 9;
        public int height = 12; 
        
        private bool _isChallengeMode = false;
        private bool _hasInitCubes = false;

        private string _saveGamePathFolderName;
        private int _gridXSize = 9;
        private int _gridZSize = 9;
        private int _tetroCnter = 0;

        private int _gameLevel = 1; // for educational and classic only
        public BindableProperty<int> challengeLevel = new BindableProperty<int>();
        public BindableProperty<int> gridSize = new BindableProperty<int>(); // <<<<<<<<<<<<<<<<<<<< 

        public BindableProperty<Vector3> camPos = new BindableProperty<Vector3>();
        public BindableProperty<Quaternion> camRot = new BindableProperty<Quaternion>();
        
        private bool _loadSavedGame = false;
        public BindableProperty<int> gameMode = new BindableProperty<int>();

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
                onChallengeMode(value);
            }
        }
        public bool hasInitCubes {
            get {
                return _hasInitCubes;
            }
            set {
                _hasInitCubes = value;
            }
        }
        public int gameLevel { // for educational and classic only
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

        private void onChallengeMode(bool challengeMode) {
            if (challengeMode)
                _saveGamePathFolderName = "challenge/level";
            else _saveGamePathFolderName = "";
        }
// 爱表哥,爱生活!!!
        public string getFilePath() {
            StringBuilder path = new StringBuilder();
            if (gameMode.Value > 0) 
                path.Append(Application.persistentDataPath + "/" + _saveGamePathFolderName
                            + _gameLevel + "/game.save"); 
            else 
                path.Append(Application.persistentDataPath + "/" + _saveGamePathFolderName 
                            + (isChallengeMode ? _gameLevel.ToString() : gridSize.Value.ToString())
                            + "/game.save");
            Debug.Log(TAG + " getFilePath() path.ToString(): " + path.ToString());
            return path.ToString();
        }
    }
}