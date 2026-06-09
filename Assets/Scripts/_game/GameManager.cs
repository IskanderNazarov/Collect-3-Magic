using _Signals;
using Zenject;
using _Infrastructure._Analytics;
using core.ads;
using System;
using DG.Tweening;
using UnityEngine;

namespace _game {
    public enum GameState {
        None,
        Setup,
        Playing,
        Win,
        Lose,
        Paused
    }

    public class GameManager : IDisposable {
        private readonly SignalBus _signalBus;
        private readonly GameplayController _gameplayController;
        private readonly PlayerProgressService _playerProgress;
        private readonly IAdsService _adsService;
        private readonly IAnalyticsService _analyticsService;
        private readonly _UI.UIManager _uiManager;
        private readonly _Infrastructure._Boosters.BoosterUseService _boosterUseService;

        public GameState CurrentState { get; private set; }

        public GameManager(
            SignalBus signalBus, 
            GameplayController gameplayController, 
            PlayerProgressService playerProgress,
            IAdsService adsService, 
            IAnalyticsService analyticsService,
            _UI.UIManager uiManager,
            _Infrastructure._Boosters.BoosterUseService boosterUseService) {
            _signalBus = signalBus;
            _gameplayController = gameplayController;
            _playerProgress = playerProgress;
            _adsService = adsService;
            _analyticsService = analyticsService;
            _uiManager = uiManager;
            _boosterUseService = boosterUseService;
        }

        public void Initialize() {
            _signalBus.Subscribe<LevelCompletedSignal>(OnLevelCompleted);
            _signalBus.Subscribe<LevelFailedSignal>(OnLevelFailed);
            _signalBus.Subscribe<UseBoosterSignal>(OnUseBooster);
            _signalBus.Subscribe<ShowBuyBoosterSignal>(OnShowBuyBooster);

            StartGame();
        }

        public void Dispose() {
            _signalBus.Unsubscribe<LevelCompletedSignal>(OnLevelCompleted);
            _signalBus.Unsubscribe<LevelFailedSignal>(OnLevelFailed);
            _signalBus.Unsubscribe<UseBoosterSignal>(OnUseBooster);
            _signalBus.Unsubscribe<ShowBuyBoosterSignal>(OnShowBuyBooster);
        }

        private void OnUseBooster(UseBoosterSignal signal) {
            _boosterUseService.TryUseBooster(signal.BoosterId, signal.ButtonPosition, () => {
                Debug.Log($"[GameManager] Booster {signal.BoosterId} used successfully.");
            });
        }

        private void OnShowBuyBooster(ShowBuyBoosterSignal signal) {
            var dialog = _uiManager.ShowScreen<_UI.BuyBoosterDialog>();
            if (dialog != null) {
                dialog.Setup(signal.BoosterId);
            }
        }

        private void StartGame() {
            ChangeState(GameState.Setup);
            _gameplayController.Initialize();

            ChangeState(GameState.Playing);
            _signalBus.Fire<LevelStartedSignal>();
            _analyticsService.LogEvent("level_start", _playerProgress.CurrentLevelIndex.ToString());
        }

        private void OnLevelCompleted() {
            if (CurrentState == GameState.Win) return;
            
            ChangeState(GameState.Win);
            _analyticsService.LogEvent("level_finish", _playerProgress.CurrentLevelIndex.ToString());

            // Sequence: delay for animations -> darken (implicit via WinScreen or explicit) -> show WinScreen
            DOVirtual.DelayedCall(1.5f, () => {
                var winScreen = _uiManager.ShowScreen<_UI.WinScreen>();
                if (winScreen != null) {
                    winScreen.Setup(20, 5); // 20 coins, 5 stars (placeholder)
                    winScreen.OnClosed += () => {
                        // Level progression can happen here
                    };
                }
            });
        }

        private void OnLevelFailed() {
            if (CurrentState == GameState.Lose) return;
            
            ChangeState(GameState.Lose);
            _analyticsService.LogEvent("level_fail", _playerProgress.CurrentLevelIndex.ToString());
        }

        private void ChangeState(GameState newState) {
            CurrentState = newState;
            Debug.Log($"[GameManager] State changed to: {newState}");
        }
    }
}
