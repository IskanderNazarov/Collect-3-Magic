using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using _Signals;
using _Infrastructure;
using _Data;
using DG.Tweening;
using TMPro;
using _UI._Meta;

namespace _UI {
    public class MainScreen : BaseScreen {
        [SerializeField] private Button _playButton;
        [SerializeField] private Transform _playButtonArrow;
        [SerializeField] private float _arrowAppearanceDelay = 5f;
        
        [Header("Meta Assets")]
        [SerializeField] private HutView[] _huts;
        [SerializeField] private RectTransform _coinsPanel;
        [SerializeField] private TextMeshProUGUI _coinsText;
        [SerializeField] private CanvasGroup _canvasGroup;

        private SignalBus _signalBus;
        private MetaProgressionService _metaService;
        private PlayerProgressService _playerProgress;
        private GameplayConfig _gameplayConfig;
        
        private Coroutine _arrowCoroutine;
        private bool _isInteracting;

        [Inject]
        public void Construct(
            SignalBus signalBus, 
            MetaProgressionService metaService, 
            PlayerProgressService playerProgress,
            GameplayConfig gameplayConfig) {
            _signalBus = signalBus;
            _metaService = metaService;
            _playerProgress = playerProgress;
            _gameplayConfig = gameplayConfig;
        }

        private void Start() {
            _playButton.onClick.AddListener(() => {
                StopArrow();
            });
        }

        private void OnEnable() {
            UpdateHuts();
            UpdateCoinsText();
            ResetInteractionTimer();
        }

        public override void Show(System.Action onComplete) {
            base.Show(() => {
                onComplete?.Invoke();
                // Sequence of animations will be triggered from GameManager or here
            });
        }

        public void UpdateHuts() {
            int unlockedCount = _metaService.UnlockedCrittersCount;
            for (int i = 0; i < _huts.Length; i++) {
                if (i < _gameplayConfig.CritterDataList.Count) {
                    var critterData = _gameplayConfig.CritterDataList[i];
                    bool isUnlocked = i < unlockedCount;
                    // For the active hut, we show stars progress
                    int stars = (i == unlockedCount) ? _playerProgress.Data.CurrentStars : 0;
                    _huts[i].Setup(critterData.CritterSprite, stars, critterData.StarsToUnlock, isUnlocked);
                } else {
                    _huts[i].gameObject.SetActive(false);
                }
            }
        }

        public void UpdateCoinsText() {
            _coinsText.text = _playerProgress.CurrentCoins.ToString();
        }

        public void SetPlayAction(System.Action action) {
            _playButton.onClick.RemoveAllListeners();
            _playButton.onClick.AddListener(() => {
                StopArrow();
                action?.Invoke();
            });
        }

        public void BlockInteraction(bool block) {
            if (_canvasGroup != null) {
                _canvasGroup.interactable = !block;
                _canvasGroup.blocksRaycasts = !block;
            }
        }

        public void AnimateRewards(int coinsEarned, int starsEarned, System.Action onComplete) {
            BlockInteraction(true);
            
            Sequence seq = DOTween.Sequence();
            
            // 1. Coins animation
            if (coinsEarned > 0) {
                // Mock animation for coins flying to _coinsPanel
                seq.AppendCallback(() => Debug.Log("Animating coins jump and fly..."));
                seq.AppendInterval(1f);
                seq.AppendCallback(() => UpdateCoinsText());
            }
            
            // 2. Stars animation
            if (starsEarned > 0) {
                seq.AppendInterval(0.5f);
                seq.AppendCallback(() => {
                    int currentUnlocked = _metaService.UnlockedCrittersCount;
                    if (currentUnlocked < _huts.Length) {
                        _huts[currentUnlocked].AnimateStarArrival(Vector3.zero, () => {
                            // After star arrival, check for unlock
                            if (_metaService.TryUnlockNext()) {
                                _huts[currentUnlocked].PlayUnlockEffect();
                            }
                        });
                    }
                });
                seq.AppendInterval(1.5f);
            }
            
            // 3. Play button bounce
            seq.Append(_playButton.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 5));
            
            seq.OnComplete(() => {
                BlockInteraction(false);
                onComplete?.Invoke();
                ResetInteractionTimer();
            });
        }

        private void Update() {
            if (Input.anyKeyDown || Input.touchCount > 0 || Input.mouseScrollDelta.y != 0) {
                ResetInteractionTimer();
            }
        }

        private void ResetInteractionTimer() {
            StopArrow();
            _arrowCoroutine = StartCoroutine(ArrowTimerCoroutine());
        }

        private void StopArrow() {
            if (_arrowCoroutine != null) StopCoroutine(_arrowCoroutine);
            if (_playButtonArrow != null) {
                _playButtonArrow.gameObject.SetActive(false);
                _playButtonArrow.DOKill();
            }
        }

        private IEnumerator ArrowTimerCoroutine() {
            yield return new WaitForSeconds(_arrowAppearanceDelay);
            if (_playButtonArrow != null) {
                _playButtonArrow.gameObject.SetActive(true);
                _playButtonArrow.localScale = Vector3.one;
                _playButtonArrow.DOScale(1.2f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            }
        }
    }
}