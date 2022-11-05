using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotFix.Control {

    public class MainScene_ScoreManager : SingletonMono<MainScene_ScoreManager> { // listener for value changes
        private const string TAG = "MainScene_ScoreManager";

        private int scoreOneLine = 800;   
        private int scoreTwoLine = 2000;  
        private int scoreThreeLine = 3600;
        private int scoreFourLine = 5600; 

        public static int currentLevel;
        public static int numLinesCleared;
        // public static float fallSpeed;

        bool isNumberOfRowsThisTurnUpdated = false;

        private static int _currentScore = 0;
        public static int currentScore {
            get {
                return _currentScore;
            }
            set {
                if (_currentScore != value) {
                    _currentScore = value;
                    PropertyChangedEventInfo propertyChangedInfo = new PropertyChangedEventInfo();
                    propertyChangedInfo.propertyName = "currentScore";
                    EventManager.Instance.FireEvent(propertyChangedInfo);
                }
            }
        }
        
        void OnEnable() {
            Debug.Log(TAG + ": OnEnable()"); 
            ModelMono.updateScoreEvent += UpdateScore;
        }
        void OnDisable() {
            Debug.Log(TAG + ": OnDisable()"); 
            ModelMono.updateScoreEvent -= UpdateScore;
        }

        public void UpdateScore() {
            if (Model.numberOfRowsThisTurn > 0 && !isNumberOfRowsThisTurnUpdated) {
                if (Model.numberOfRowsThisTurn == 1) 
                    ClearedOneLine();
                else if (Model.numberOfRowsThisTurn == 2) 
                    ClearedTwoLine();
                else if (Model.numberOfRowsThisTurn == 3) 
                    ClearedThreeLine();
                else if (Model.numberOfRowsThisTurn == 4) 
                    ClearedFourLine();
                Model.numberOfRowsThisTurn = 0;
                isNumberOfRowsThisTurnUpdated = true;
            }
        }
        
        public void ClearedOneLine() {
            currentScore += scoreOneLine + (currentLevel + 20);
            numLinesCleared += 1;
        }
        public void ClearedTwoLine() {
            currentScore += scoreTwoLine + (currentLevel + 25);
            numLinesCleared += 2;
        }
        public void ClearedThreeLine() {
            currentScore += scoreThreeLine + (currentLevel + 30);
            numLinesCleared += 3;
        }
        public void ClearedFourLine() {
            currentScore += scoreFourLine + (currentLevel + 40);
            numLinesCleared += 4;
        }

        public void resetScore() {
            currentScore = 0;
            numLinesCleared = 0;
        }
    }
}
