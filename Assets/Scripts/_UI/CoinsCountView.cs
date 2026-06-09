using _Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Core._Purchasing;

namespace _UI {
    public class CoinsCountView : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private Button _openMarketButton;

        private PlayerProgressService _progressService;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(PlayerProgressService progressService, SignalBus signalBus) {
            _progressService = progressService;
            _signalBus = signalBus;
        }

        private void Start() {
            if (_openMarketButton == null) {
                _openMarketButton = GetComponent<Button>();
            }

            if (_openMarketButton != null) {
                _openMarketButton.onClick.AddListener(OnOpenMarketClicked);
            }
        }

        private void OnDestroy() {
            if (_openMarketButton != null) {
                _openMarketButton.onClick.RemoveListener(OnOpenMarketClicked);
            }
        }

        private void OnEnable() {
            UpdateDisplay(_progressService.CurrentCoins);
            _signalBus.Subscribe<CoinsChangedSignal>(OnCoinsChanged);
        }

        private void OnDisable() {
            _signalBus.TryUnsubscribe<CoinsChangedSignal>(OnCoinsChanged);
        }

        private void OnOpenMarketClicked() {
            print("OnOpenMarketClicked");
            //_signalBus.Fire(new ShowDialogSignal(typeof(MarketController)));
        }

        private void OnCoinsChanged(CoinsChangedSignal signal) {
            UpdateDisplay(signal.NewValue);
        }

        private void UpdateDisplay(int count) {
            if (_countText != null) {
                _countText.text = count.ToString();
            }
        }
    }
}