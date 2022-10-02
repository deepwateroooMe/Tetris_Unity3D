using UnityEngine;
using Framework.Util;

namespace tetris3d {

    public class GloData : Singleton<GloData> {
    // public class GameMenuData : ScriptableObject { // TODO: refactor to be GlobalData.cs static class ???
// https://unity3d.com/how-to/architect-with-scriptable-objects
        
        private const string TAG = "GloData";
        
        private bool _loadSavedGame = false;
        private bool _isChallengeMode = true;
        private int _gameMode = 0;
        private string _saveGamePathFolderName;
        private int _gridSize = 5;
        private int _gridXSize = 9;
        private int _gridZSize = 9;
        private int _tetrominoCnter = 0;
        private int _challengeLevel = 0;
        
        public int layerScore = 8000;
        public int challengeLayerScore = 16000;
        
        void OnEnable() {
            Debug.Log(TAG + ": OnEnable()");

            _gameMode = 0;
            _loadSavedGame = false;
            _isChallengeMode = false;
            _saveGamePathFolderName = "";
            _gridSize = 5; //-1
            _tetrominoCnter = 0;
        }

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
        public int tetrominoCnter {
            get {
                return _tetrominoCnter;
            }
            set {
                _tetrominoCnter = value;
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