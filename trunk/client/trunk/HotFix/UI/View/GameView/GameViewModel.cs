using Framework.MVVM;
using UnityEngine;

namespace HotFix.UI {

    public class GameViewModel : ViewModelBase {

        protected override void OnInitialize() {
            base.OnInitialize();
            Initialization();
            DelegateSubscribe();
        }

        void Initialization() {
            this.ParentViewModel = ViewManager.EducaModesView.BindingContext;
        }

        void DelegateSubscribe() {
        }

        // 最开始的三种大方格的状态都应该是隐藏着的
        void InitializeGrid() {
        }

        public void PauseGame() {
            Time.timeScale = 0f;	    
            audioSource.Pause(); // ui
            isPaused = true;

            // Bug: disable all Hud canvas buttons: swap
            audioSource.Pause(); // ui
            pausePanel.SetActive(true); // ui

            // Bug cleaning: when paused game, if game has NOT started yet, disable Save Button
            if (!gameStarted) {
                
            }
            
        }
     }
}
