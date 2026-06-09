using core.ads;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _UI {
    public class WinScreen : BaseScreen {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _coinsText;
        [SerializeField] private TextMeshProUGUI _starsText;
        [SerializeField] private Button _claimButton;
        [SerializeField] private Button _claim2xButton;

        private int _baseCoins;
        private int _baseStars;

        [Inject] private PlayerProgressService _playerProgress;
        [Inject] private IAdsService _adsService;

        private void Start() {
            _claimButton.onClick.AddListener(Claim);
            _claim2xButton.onClick.AddListener(Claim2x);
        }

        public void Setup(int coins, int stars) {
            _baseCoins = coins;
            _baseStars = stars;
            UpdateUI();
        }

        private void UpdateUI() {
            _coinsText.text = _baseCoins.ToString();
            _starsText.text = _baseStars.ToString();
        }

        private void Claim() {
            RewardPlayer(_baseCoins, _baseStars);
            NotifyClosed();
            Hide();
        }

        private void Claim2x() {
            _adsService.ShowRewarded(
                onRewardGranted: () => {
                    // Double (or 10x as per user request, let's go with 2x as button says, or 10x if we want)
                    RewardPlayer(_baseCoins * 2, _baseStars); // Stars usually aren't doubled, but can be
                    NotifyClosed();
                    Hide();
                },
                onAdClosed: null
            );
        }

        private void RewardPlayer(int coins, int stars) {
            // VFX Place: You can trigger coin fly animation here
            Debug.Log($"[WinScreen] Rewarding player with {coins} coins and {stars} stars.");

            _playerProgress.AddCoins(coins);
            _playerProgress.AddStars(stars);
        }
    }
}
