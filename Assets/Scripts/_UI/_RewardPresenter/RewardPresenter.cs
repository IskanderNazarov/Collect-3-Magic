using System;
using Core._RewardPresenter;
using Core._Rewards;
using core.rewards;
using UnityEngine;
using Zenject;

namespace _UI._RewardPresenter {
    public class RewardPresenter : IRewardPresenter {
        private readonly RewardPresenterView _viewPrefab;
        private readonly DiContainer _container;
        private readonly IRewardAudioPlayer _audioPlayer;
        
        private RewardPresenterView _currentViewInstance;

        public bool IsRewardingInProgress { get; private set; }
        public event Action OnSequenceComplete;

        public RewardPresenter(RewardPresenterView viewPrefab, DiContainer container, IRewardAudioPlayer audioPlayer) {
            _viewPrefab = viewPrefab;
            _container = container;
            _audioPlayer = audioPlayer;
        }

        public void ShowReward(Reward reward) {
            if (IsRewardingInProgress) return;
            IsRewardingInProgress = true;

            _audioPlayer?.PlayRewardShowSound();

            // Инстанцируем через Zenject, чтобы внутри View работали инжекты (если понадобятся)
            _currentViewInstance = _container.InstantiatePrefabForComponent<RewardPresenterView>(_viewPrefab);
            
            // Настраиваем Canvas
            var canvas = _currentViewInstance.GetComponent<Canvas>();
            if (canvas != null) {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = Camera.main;
            }

            // Подписываемся на события View
            _currentViewInstance.OnItemSpawned += () => _audioPlayer?.PlayItemSpawnSound();
            _currentViewInstance.OnSequenceComplete += HandleViewClosed;
            
            // Запускаем показ
            _currentViewInstance.ShowReward(reward);
        }

        private void HandleViewClosed() {
            if (_currentViewInstance != null) {
                _currentViewInstance.OnSequenceComplete -= HandleViewClosed;
                GameObject.Destroy(_currentViewInstance.gameObject);
                _currentViewInstance = null;
            }

            IsRewardingInProgress = false;
            OnSequenceComplete?.Invoke();
        }
    }
}