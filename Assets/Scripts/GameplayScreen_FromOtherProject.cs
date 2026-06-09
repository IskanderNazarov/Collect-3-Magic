/*using System;
using System.Collections;
using System.Collections.Generic;
using __CoreGameLib._Scripts._Services._Leaderboards;
using __Gameplay;
using _Services._Localization;
using _UI;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

// Добавлен using для ILevelProvider

public class GameplayScreen : BaseScreen {
    [Header("Boosters")] [SerializeField] private BoosterButton[] _boosterButtons;
    [Space(20)] [SerializeField] private Image _difficultyIcon;
    [SerializeField] private Image _difficultyPanel;
    [SerializeField] private TextMeshProUGUI _difficultyTMP;
    [SerializeField] private TextMeshProUGUI _levelTMP;
    [SerializeField] private TextMeshProUGUI _tapToContinueTMP;

    [Header("Difficulty Intro")] [SerializeField]
    private GameObject _introPanel;

    [SerializeField] private Image _introArrow;
    [SerializeField] private TextMeshProUGUI _introText;
    [SerializeField] private float _introAutoCloseTime = 3f;
    [SerializeField] private Vector2 _introArrowStartPAnchoredPos;
    [SerializeField] private float _introArrowStartTargetScale = 2.5f;
    [SerializeField, TranslationKey] private string _hardTitleKey;
    [SerializeField, TranslationKey] private string _superhardTitleKey;
    [SerializeField, TranslationKey] private string _levelShortTitle;

    [Header("System Buttons")] [SerializeField]
    private Button _restartButton;

    [SerializeField] private Button _restartDebugButton;
    [SerializeField] private Button _resetLevelNumberButton;

    [Header("Score & UI")] [SerializeField]
    private GameObject _adSign;

    [SerializeField] private TMP_Dropdown dropdownDebug;

    [Inject] private GameplayController _gameplayController;
    [Inject] private SignalBus _signalBus;
    [Inject] private ILeaderboardService _leaderboardService;
    [Inject] private PlayerProgressService _playerProgressService;
    [Inject] private LevelsDatabase _levelsDatabase;
    [Inject] private GameBoosterInventory _boosterInventory;
    [Inject] private GameConfig _gameConfig;
    [Inject] private BoostersConfig _boostersConfig;
    [Inject] private ILevelProvider _levelProvider;
    [Inject] private Localizer _localizer;

    private Coroutine _introCoroutine;

    private void Start() {
        _introPanel.SetActive(false);
        foreach (var bb in _boosterButtons) {
            var count = _boosterInventory.GetCount(bb.BoosterId);
            var boosterInfo = _boostersConfig.GetBooster(bb.BoosterId);
            int requiredLevel = boosterInfo != null ? boosterInfo.RequiredLevel : 0;
            
            bb.Initialize(OnBoosterClicked, requiredLevel, _localizer);
            bb.UpdateVisuals(count);
        }

        UpdateBoostersAvailability();

        _boosterInventory.OnChanged += OnBoosterInventoryChanged;

        _restartButton.onClick.AddListener(() => _signalBus.Fire<RestartLevelClickedSignal>());
        _restartDebugButton.onClick.AddListener(() => _signalBus.Fire<RestartLevelClickedSignal>());

        var N = _levelsDatabase.levelDatas.Count;
        var optionsList = new List<TMP_Dropdown.OptionData>();
        for (var i = 0; i < N; i++) {
            optionsList.Add(new TMP_Dropdown.OptionData(i.ToString()));
        }

        dropdownDebug.options = optionsList;
        dropdownDebug.onValueChanged.AddListener(LevelSelectedFromDropDown);
    }

    private void OnDestroy() {
        _boosterInventory.OnChanged -= OnBoosterInventoryChanged;
        dropdownDebug.onValueChanged.RemoveListener(LevelSelectedFromDropDown);
    }

    // ==========================================
    // ЛОГИКА ЖИЗНЕННОГО ЦИКЛА ЭКРАНА
    // ==========================================

    public override void Show(Action onComplete) {
        base.Show(onComplete); // Включает gameObject

        // Как только экран показан, обновляем и анимируем сложность
        UpdateDifficultyVisual();
    }

    private void UpdateDifficultyVisual() {
        // Убиваем старые анимации
        _difficultyIcon.transform.DOKill();
        _difficultyIcon.DOKill();
        _difficultyTMP.transform.DOKill();

        if (_introPanel != null) {
            _introPanel.SetActive(false);
            _introArrow.transform.DOKill();
            _introArrow.DOKill();
            _introText.DOKill();
            DOTween.Kill(_introPanel);
        }

        // Берем актуальные данные уровня
        var currentLevel = _levelProvider.GetCurrentLevel();
        var info = _gameConfig.GetDifficultData(currentLevel.DifficultyType);

        _levelTMP.text = $"{_localizer.Get(_levelShortTitle)} {currentLevel.LevelIndex + 1}";
        _difficultyIcon.sprite = info.Icon;
        var key = currentLevel.DifficultyType == 1 ? _hardTitleKey : _superhardTitleKey;
        _difficultyTMP.text = _localizer.Get(key);
        _difficultyTMP.color = info.TextColor;

        UpdateBoostersAvailability();

        if (_introCoroutine != null) StopCoroutine(_introCoroutine);


        if (currentLevel.DifficultyType > 0) {
            _introCoroutine = StartCoroutine(StartDifficultyIntro(info));
        } else {
            _difficultyIcon.gameObject.SetActive(false);
            _difficultyTMP.gameObject.SetActive(false);
            _difficultyTMP.alpha = 1f;
            if (_introPanel != null) _introPanel.SetActive(false);
        }
    }

    private bool _introClickTriggered;

    

    // ==========================================
    // ЛОГИКА БУСТЕРОВ
    // ==========================================

    private void UpdateBoostersAvailability() {
        var currentLevelIndex = _levelProvider.GetCurrentLevel().LevelIndex;
        foreach (var btn in _boosterButtons) {
            var boosterInfo = _boostersConfig.GetBooster(btn.BoosterId);
            if (boosterInfo != null) {
                btn.EnableView(currentLevelIndex >= boosterInfo.RequiredLevel);
            }
        }
    }

    private void OnBoosterClicked(BoosterId id, Transform btnTransform) {
        _signalBus.Fire(new BoosterBtnClickSignal(id, btnTransform));
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

    private void ShowNativeLeaderboard() {
        var playerScore = _playerProgressService.Score;
        _leaderboardService.SetPlayerScore("score", playerScore,
            delegate { DOVirtual.DelayedCall(0.1f, delegate { _leaderboardService.ShowNativeLeaderboard("score", null); }); });
    }

    private void LevelSelectedFromDropDown(int index) {
        Debug.Log($"index: {index}");
        _signalBus.Fire(new RestartLevelClickedSignal { levelNumber = index });
    }
}*/