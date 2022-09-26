using UnityEngine;

namespace tetris3d {

    public class GameMenuData : Singleton<GameMenuData> {

        private bool _loadSavedGame;
        private int _gameMode = 0;
        private string _saveGamePathFolderName;
        private int _gridSize; //= 5;

        void OnEnable() {
            _gameMode = 0;
            _loadSavedGame = false;
            _saveGamePathFolderName = "";
            _gridSize = -1;
        }

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
        public int gridSize {
            get {
                return _gridSize;
            }
            set {
                _gridSize = value;
            }
        }
    }
}