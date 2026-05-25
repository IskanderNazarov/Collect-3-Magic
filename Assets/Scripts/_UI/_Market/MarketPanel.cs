// file: MarketPanel.cs

using System;
using _Services._Localization;
using Rewards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

// required for Reward and IRewardItem

namespace Core._Purchasing.UI {
    public class MarketPanel : MonoBehaviour {
        // direct reference set in inspector
        [SerializeField] private MarketItemData _itemData;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private Button _actionButton;

        public MarketItemData ItemData => _itemData;

        private Action<string, bool> _onClicked;

        public void Initialize(string price, Action<string, bool> onClicked) {
            _onClicked = onClicked;

            //print($"Initialize market panel {name}, price: {price}");
            //_priceText.text = _itemData.IsRewardedVideo ? "WATCH" : price;
            if (!_itemData.IsRewardedVideo) {
                _priceText.text = price;
            }

            // extract and set reward count
            UpdateCountText();

            _actionButton.onClick.RemoveAllListeners();
            _actionButton.onClick.AddListener(OnButtonClicked);
        }

        private void UpdateCountText() {
            if (_itemData.RewardContent == null || _itemData.RewardContent.Items == null ||
                _itemData.RewardContent.Items.Count == 0) {
                _countText.text = string.Empty;
                return;
            }

            // get the first reward item for simple panels
            var firstItem = _itemData.RewardContent.Items[0];
            var countAmount = 0;

            if (firstItem is CoinsReward coins) {
                countAmount = coins.Count;
            } else if (firstItem is BoosterReward booster) {
                countAmount = booster.Count;
            }

            _countText.text = countAmount > 0 ? countAmount.ToString() : string.Empty;
        }

        private void OnButtonClicked() {
            if (_itemData != null) {
                _onClicked?.Invoke(_itemData.ProductId, _itemData.IsRewardedVideo);
            }
        }
    }
}