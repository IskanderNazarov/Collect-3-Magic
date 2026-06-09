using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace _UI {
    public class UIManager : MonoBehaviour {
        [SerializeField] private List<BaseScreen> screens;

        private BaseScreen _currentScreen;

        [Inject]
        public void Construct() {
            foreach (var screen in screens) {
                screen.HideImmediate();
            }
        }

        // original signature for backwards compatibility
        public T ShowScreen<T>(Action onComplete = null) where T : BaseScreen {
            return ShowScreen<T>(true, onComplete);
        }

        // new signature allowing overlays
        public T ShowScreen<T>(bool hideCurrent, Action onComplete = null) where T : BaseScreen {
            var nextScreen = screens.FirstOrDefault(s => s is T);
            
            if (nextScreen == null) {
                Debug.LogError($"// screen {typeof(T)} not found in UIManager list!");
                return null;
            }

            // normal behavior: swap screens
            if (hideCurrent && _currentScreen != null && _currentScreen != nextScreen) {
                _currentScreen.Hide(() => {
                    _currentScreen = nextScreen;
                    nextScreen.Show(onComplete);
                });
            } else {
                // overlay behavior: just show next, keep current active underneath
                if (hideCurrent) {
                    _currentScreen = nextScreen;
                }
                nextScreen.Show(onComplete);
            }

            return nextScreen as T;
        }

        public T GetScreen<T>() where T : BaseScreen {
            return screens.FirstOrDefault(s => s is T) as T;
        }

        public void HideAllScreens() {
            foreach (var screen in screens) {
                screen.HideImmediate();
            }
            _currentScreen = null;
        }
    }
}