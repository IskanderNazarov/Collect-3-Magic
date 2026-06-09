using System;
using _Infrastructure;
using _Infrastructure._Analytics;
using _Infrastructure._Boosters;
using _Signals;
using _UI;
using core.ads;
using DG.Tweening;
using UnityEngine;
using Zenject;

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
        private readonly UIManager _uiManager;
        private readonly BoosterUseService _boosterUseService;
        private readonly MetaProgressionService _metaService;

        public GameState CurrentState { get; private set; }

        public GameManager(
            SignalBus signalBus,
            GameplayController gameplayController,
            PlayerProgressService playerProgress,
            IAdsService adsService,
            IAnalyticsService analyticsService,
            UIManager uiManager,
            BoosterUseService boosterUseService,
            MetaProgressionService metaService) {
            _signalBus = signalBus;
            _gameplayController = gameplayController;
            _playerProgress = playerProgress;
            _adsService = adsService;
            _analyticsService = analyticsService;
            _uiManager = uiManager;
            _boosterUseService = boosterUseService;
            _metaService = metaService;
        }

        public void Initialize() {
            _signalBus.Subscribe<LevelCompletedSignal>(OnLevelCompleted);
            _signalBus.Subscribe<LevelFailedSignal>(OnLevelFailed);
            _signalBus.Subscribe<UseBoosterSignal>(OnUseBooster);
            _signalBus.Subscribe<ShowBuyBoosterSignal>(OnShowBuyBooster);

            // First time user flow: go directly to Level 1
            if (_playerProgress.CurrentLevelIndex == 0 && _playerProgress.Score == 0) {
                StartGame();
            } else {
                ShowMeta();
            }
        }

        private void ShowMeta(int coinsEarned = 0, int starsEarned = 0) {
            ChangeState(GameState.None);
            _uiManager.HideAllScreens();
            var mainScreen = _uiManager.ShowScreen<MainScreen>();
            if (mainScreen != null) {
                mainScreen.SetPlayAction(StartGame);
                if (coinsEarned > 0 || starsEarned > 0) {
                    mainScreen.AnimateRewards(coinsEarned, starsEarned, null);
                }
            }
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
            var dialog = _uiManager.ShowScreen<BuyBoosterDialog>();
            if (dialog != null) {
                dialog.Setup(signal.BoosterId);
            }
        }

        private void StartGame() {
            _uiManager.HideAllScreens();
            _uiManager.ShowScreen<GameUI>();

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

            // Sequence: delay for animations -> show WinScreen
            DOVirtual.DelayedCall(1.5f, () => {
                var winScreen = _uiManager.ShowScreen<WinScreen>(false);
                if (winScreen != null) {
                    int coins = 20;
                    int stars = 1;
                    winScreen.Setup(coins, stars); 
                    winScreen.OnClosed += () => {
                        _playerProgress.IncrementLevel();
                        ShowMeta(coins, stars);
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
