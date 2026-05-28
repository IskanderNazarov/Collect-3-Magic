using _Signals;
using Zenject;
using _Infrastructure._Analytics;
using core.ads;
using System;
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

        public GameState CurrentState { get; private set; }

        public GameManager(
            SignalBus signalBus, GameplayController gameplayController, PlayerProgressService playerProgress,
            IAdsService adsService, IAnalyticsService analyticsService) {
            _signalBus = signalBus;
            _gameplayController = gameplayController;
            _playerProgress = playerProgress;
            _adsService = adsService;
            _analyticsService = analyticsService;
        }

        public void Initialize() {
            _signalBus.Subscribe<LevelCompletedSignal>(OnLevelCompleted);
            _signalBus.Subscribe<LevelFailedSignal>(OnLevelFailed);

            StartGame();
        }

        public void Dispose() {
            _signalBus.Unsubscribe<LevelCompletedSignal>(OnLevelCompleted);
            _signalBus.Unsubscribe<LevelFailedSignal>(OnLevelFailed);
        }

        private void StartGame() {
            ChangeState(GameState.Setup);
            _gameplayController.Initialize();

            ChangeState(GameState.Playing);
            _signalBus.Fire<LevelStartedSignal>();
            _analyticsService.LogEvent("level_start", _playerProgress.CurrentLevelIndex.ToString());
        }

        private void OnLevelCompleted() {
            ChangeState(GameState.Win);
            _playerProgress.AddCoins(20);
            _analyticsService.LogEvent("level_finish", _playerProgress.CurrentLevelIndex.ToString());

            _adsService.ShowInterstitial(AdPlacementType.AfterGameAction, () => {
                Debug.Log("[GameManager] Interstitial closed.");
            });
        }

        private void OnLevelFailed() {
            ChangeState(GameState.Lose);
            _analyticsService.LogEvent("level_fail", _playerProgress.CurrentLevelIndex.ToString());
        }

        private void ChangeState(GameState newState) {
            CurrentState = newState;
            Debug.Log($"[GameManager] State changed to: {newState}");
        }
    }
}
