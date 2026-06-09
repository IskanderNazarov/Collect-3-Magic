using __Gameplay;
using _Data;
using _Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

using __Gameplay;
using _Data;
using _Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Collections.Generic;
using _Infrastructure._Boosters;
using _Services._Localization;
using DG.Tweening;

namespace _UI {
    public class GameUI : BaseScreen {

        [SerializeField] private Button _changeBgBtn;
        [SerializeField] private SpriteRenderer _spriteRendererMain;
        [SerializeField] private SpriteRenderer _spriteRendererTop;

        [Header("Boosters")]
        [SerializeField] private BoosterButton[] _boosterButtons;

        [Header("Progress")]
        [SerializeField] private TextMeshProUGUI _levelLabel;
        [SerializeField] private Image _progressFill;

        private SignalBus _signalBus;
        private PlayerProgressService _playerProgress;
        private LevelConfig _levelConfig;
        private GameBoosterInventory _boosterInventory;
        private BoostersConfig _boostersConfig;
        private Localizer _localizer;
        private int _completedMatches;

        [Inject]
        public void Construct(
            SignalBus signalBus, 
            PlayerProgressService playerProgress, 
            LevelConfig levelConfig,
            GameBoosterInventory boosterInventory,
            BoostersConfig boostersConfig,
            Localizer localizer) {
            _signalBus = signalBus;
            _playerProgress = playerProgress;
            _levelConfig = levelConfig;
            _boosterInventory = boosterInventory;
            _boostersConfig = boostersConfig;
            _localizer = localizer;
        }

        private int _counter;
        private void Start() {
            _changeBgBtn.onClick.AddListener(() => {
                _counter++;
                var isFistBg = _counter % 2 == 0;
                _spriteRendererMain.gameObject.SetActive(isFistBg);
                _spriteRendererTop.gameObject.SetActive(!isFistBg);
            });

            foreach (var bb in _boosterButtons) {
                var count = _boosterInventory.GetCount(bb.BoosterId);
                var boosterInfo = _boostersConfig.GetBooster(bb.BoosterId);
                int requiredLevel = boosterInfo != null ? boosterInfo.RequiredLevel : 0;
                
                bb.Initialize(OnBoosterClicked, requiredLevel, _localizer);
                bb.UpdateVisuals(count);
                bb.EnableView(_playerProgress.CurrentLevelIndex >= requiredLevel);
            }

            _boosterInventory.OnChanged += OnBoosterInventoryChanged;
            _signalBus.Subscribe<MatchCompletedSignal>(OnMatchCompleted);
            
            SetupProgress();
        }

        private void OnDestroy() {
            if (_boosterInventory != null) _boosterInventory.OnChanged -= OnBoosterInventoryChanged;
            _signalBus.Unsubscribe<MatchCompletedSignal>(OnMatchCompleted);
        }

        private void OnBoosterClicked(BoosterId id, Transform btnTransform) {
            _signalBus.Fire(new UseBoosterSignal(id, btnTransform.position));
        }

        private void OnBoosterInventoryChanged(BoosterId id, int delta) {
            var newCount = _boosterInventory.GetCount(id);
            foreach (var btn in _boosterButtons) {
                if (btn.BoosterId == id) {
                    btn.UpdateVisuals(newCount);
                    if (delta > 0) {
                        btn.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 5);
                    }
                    break;
                }
            }
        }

        private void SetupProgress() {
            _levelLabel.text = $"Level {_playerProgress.CurrentLevelIndex + 1}";
            _completedMatches = 0;
            UpdateProgressUI();
        }

        private void OnMatchCompleted() {
            _completedMatches++;
            UpdateProgressUI();
        }

        private void UpdateProgressUI() {
            float total = _levelConfig.totalItemsCount / 3f;
            _progressFill.fillAmount = total > 0 ? (float)_completedMatches / total : 0;
        }
    }
}
