using System;
using __Gameplay;
using _Services._Localization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BoosterButton : MonoBehaviour {
    [SerializeField] private BoosterId _boosterId; // Выбираем в инспекторе (Hint, Hammer, Ruler)
    [SerializeField] private Button _button;

    [Header("Panels")] [SerializeField] private GameObject _activePanel;
    [SerializeField] private GameObject _passivePanel;

    [Header("Active Visuals")] [SerializeField]
    private TextMeshProUGUI _counterText; // Текст с количеством

    [SerializeField] private GameObject _adOverlay; // Значок рекламы/плюсик, если бустеров 0

    [Header("Locked Hint")] [SerializeField]
    private GameObject _lockedHintPanel;

    [SerializeField] private TextMeshProUGUI _lockedHintText;
    [SerializeField, TranslationKey] private string _lockedHintKey = "LOCKED_HINT"; // "Available at level {0}"

    private bool _isEnabled = true;
    private int _requiredLevel;
    private Tween _hintTween;

    private Localizer _localizer;

    public BoosterId BoosterId => _boosterId;

    public void Initialize(Action<BoosterId, Transform> onClickCallback, int requiredLevel, Localizer localizer) {
        _requiredLevel = requiredLevel;
        _localizer = localizer;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => {
            if (!_isEnabled) {
                if (_lockedHintPanel != null && _lockedHintPanel.activeSelf) {
                    HideLockedHint();
                } else {
                    ShowLockedHint();
                }
                return;
            }

            // Легкая анимация нажатия самой кнопки для отзывчивости
            transform.DOKill(true);
            transform.DOPunchScale(Vector3.one * -0.1f, 0.15f, 1);

            onClickCallback?.Invoke(_boosterId, transform);
        });

        if (_lockedHintPanel != null) _lockedHintPanel.SetActive(false);
    }

    public void EnableView(bool enable) {
        _isEnabled = enable;

        if (_activePanel != null) _activePanel.SetActive(enable);
        if (_passivePanel != null) _passivePanel.SetActive(!enable);

        // Button itself remains interactable to show the hint when clicked while passive
        // OR we can make it non-interactable but handle clicks via IPointerClickHandler if preferred.
        // But uGUI Button with onClick is easier if we want to catch the click.
    }

    private void ShowLockedHint() {
        if (_lockedHintPanel == null) return;

        _hintTween?.Kill();
        _lockedHintPanel.SetActive(true);
        _lockedHintPanel.transform.localScale = Vector3.zero;

        if (_lockedHintText != null) {
            string localizedFormat = _localizer.Get(_lockedHintKey);
            _lockedHintText.text = string.Format(localizedFormat, _requiredLevel);
        }

        _hintTween = DOTween.Sequence()
            .Append(_lockedHintPanel.transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack))
            .Append(_lockedHintPanel.transform.DOScale(1f, 0.1f))
            .AppendInterval(1f)
            .Append(_lockedHintPanel.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack))
            .OnComplete(() => _lockedHintPanel.SetActive(false));
    }

    private void HideLockedHint() {
        if (_lockedHintPanel == null) return;
        _hintTween?.Kill();
        _lockedHintPanel.SetActive(false);
    }

    // Обновление визуала
    public void UpdateVisuals(int count) {
        if (_counterText != null) {
            _counterText.text = count > 0 ? count.ToString() : "";
        }

        if (_adOverlay != null) {
            _adOverlay.SetActive(count <= 0);
        }
    }
}