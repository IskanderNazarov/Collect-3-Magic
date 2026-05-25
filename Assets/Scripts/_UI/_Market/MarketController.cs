// file: MarketController.cs

using System;
using System.Collections.Generic;
using _Infrastructure._Analytics;
using _UI;
using Core._Purchasing.UI;
using core.ads;
using core.purchasing;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Core._Purchasing {
    public class MarketController : Dialog {
        [SerializeField] private List<MarketPanel> _panels;
        [SerializeField] private Button _duplicatedAdsBtn;

        [Inject] private IPurchaser _purchaser;
        [Inject] private RewardHandler _rewardHandler;
        [Inject] private IAdsService _adsService;
        [Inject] private MarketConfig _config;

        public override void Show(Action onComplete = null) {
            base.Show(onComplete);
            _purchaser.OnPurchaseCompletedEvent += OnPurchaseCompleted;
            InitializePanels();
        }

        public override void Hide(Action onComplete = null) {
            base.Hide(onComplete);
            _purchaser.OnPurchaseCompletedEvent -= OnPurchaseCompleted;
        }

        private void InitializePanels() {
            MarketPanel adsPanel = null;
            foreach (var panel in _panels) {
                if (panel.ItemData == null) {
                    Debug.LogWarning("// panel misses item data");
                    continue;
                }

                var productInfo = _purchaser.GetProdInfoByID(panel.ItemData.ProductId);
                var priceString = productInfo != null ? productInfo.price : "N/A";

                panel.Initialize(priceString, OnPanelClicked);
                if (panel.ItemData.IsRewardedVideo) {
                    adsPanel = panel;
                }
            }

            if (_duplicatedAdsBtn != null && adsPanel != null) {
                _duplicatedAdsBtn.onClick.RemoveAllListeners();
                _duplicatedAdsBtn.onClick.AddListener(() => {
                    OnPanelClicked(adsPanel.ItemData.ProductId, true);
                });
            }
        }

        private void OnPanelClicked(string id, bool isRewarded) {
            if (isRewarded) {
                _adsService.ShowRewarded(
                    onRewardGranted: () => {
                        var itemData = _config.GetItem(id);
                        if (itemData != null) {
                             _rewardHandler.HandlerReward(itemData.RewardContent, "market_ads", true);
                        }
                    },
                    onAdClosed: null
                );
            }
            else {
                _purchaser.BuyItem(id);
            }
        }

        private void OnPurchaseCompleted(string id, bool isRestoring) {
            // Purchase logic (rewards, consumption, analytics) is now handled globally in PurchaseHandler.cs
            
            // Close market after a successful manual purchase
            if (!isRestoring) {
                Hide();
            }
        }
    }
    }
