// File: Assets/Game/Scripts/Rewards/RewardPresenterView.cs

using System;
using System.Collections.Generic;
using Core._RewardPresenter;
using Core._Rewards.UI;
using DG.Tweening;
using Rewards;
using UnityEngine;
using UnityEngine.UI;

namespace Core._Rewards {
    // Этот класс больше не отвечает за свое уничтожение, он только сообщает о завершении
    [RequireComponent(typeof(Canvas))]
    public class RewardPresenterView : MonoBehaviour {
        [SerializeField] private Transform _gridLayoutParent;
        [SerializeField] private RewardPresentItem _rewardItemPrefab;
        [SerializeField] private Button _closeScreenButton;
        [SerializeField] private Sprite _coinsSprite;
        [SerializeField] private List<Sprite> _boosterSprites;

        [Header("Animation Settings")]
        [SerializeField] private RectTransform _mainPanel;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeInDuration = 0.4f;
        [SerializeField] private float _itemStaggerDelay = 0.1f;

        public event Action OnSequenceComplete;
        public event Action OnItemSpawned;

        private List<RewardPresentItem> _activeViews = new List<RewardPresentItem>();
        private bool _isInit;

        private void Initialize() {
            if (_isInit) return;
            _isInit = true;

            _closeScreenButton.onClick.AddListener(CloseAndCleanup);

            // Setup initial state for animation
            if (_canvasGroup != null) _canvasGroup.alpha = 0;
            if (_mainPanel != null) _mainPanel.localScale = Vector3.one * 0.8f;
        }

        public void ShowReward(Reward reward) {
            if (reward == null || reward.Items == null) return;
            Initialize();

            // Start base animation
            if (_canvasGroup != null) _canvasGroup.DOFade(1, _fadeInDuration).SetEase(Ease.OutCubic);
            if (_mainPanel != null) _mainPanel.DOScale(1, _fadeInDuration).SetEase(Ease.OutBack);

            float delay = _fadeInDuration * 0.5f;
            foreach (var item in reward.Items) {
                if (item is CoinsReward coins) {
                    SpawnView(_coinsSprite, coins.Count, delay);
                }
                else if (item is BoosterReward booster) {
                    var sprite = GetBoosterSprite((int)booster.BoosterId);
                    SpawnView(sprite, booster.Count, delay);
                }
                delay += _itemStaggerDelay;
            }
        }

        private Sprite GetBoosterSprite(int id) {
            if (id >= 0 && id < _boosterSprites.Count) return _boosterSprites[id];
            return null;
        }

        private void SpawnView(Sprite icon, int amount, float delay) {
            var view = Instantiate(_rewardItemPrefab, _gridLayoutParent);
            view.Setup(icon, amount);
            _activeViews.Add(view);

            OnItemSpawned?.Invoke();

            // Animate item
            view.transform.localScale = Vector3.zero;
            view.transform.DOScale(1, 0.5f)
                .SetDelay(delay)
                .SetEase(Ease.OutBack);
        }

        private void CloseAndCleanup() {
            // Outro animation before notifying completion
            if (_canvasGroup != null) {
                _canvasGroup.DOFade(0, 0.3f).SetEase(Ease.InCubic).OnComplete(NotifyComplete);
            }
            else {
                NotifyComplete();
            }
        }

        private void NotifyComplete() {
            foreach (var view in _activeViews) {
                if (view != null) Destroy(view.gameObject);
            }

            _activeViews.Clear();

            // Сообщаем презентеру, что пора нас уничтожать
            OnSequenceComplete?.Invoke();
        }
    }
}
