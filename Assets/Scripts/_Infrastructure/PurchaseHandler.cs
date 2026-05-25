// file: assets/game/scripts/purchasehandler.cs
// assembly: game.asmdef

using _Analytics;
using _Infrastructure._Analytics;
using Core._Purchasing;
using UnityEngine;
using Zenject;

namespace core.purchasing {
    public class PurchaseHandler {
        private readonly IPurchaser _purchaser;
        private readonly MarketConfig _marketConfig;
        private readonly RewardHandler _rewardHandler;
        private readonly IAnalyticsService _analyticsService;

        [Inject]
        public PurchaseHandler(IPurchaser purchaser, MarketConfig marketConfig, RewardHandler rewardHandler, IAnalyticsService analyticsService) {
            _purchaser = purchaser;
            _marketConfig = marketConfig;
            _rewardHandler = rewardHandler;
            _analyticsService = analyticsService;
        }

        public void Initialize() {
            _purchaser.OnPurchaseCompletedEvent += OnPurchaseCompleted;
        }

        private void OnPurchaseCompleted(string id, bool isRestoringPurchase) {
            Debug.Log($"purchasehandler: received purchase id: {id}, isrestoring: {isRestoringPurchase}");

            var iapData = _marketConfig.GetItem(id);
            if (iapData == null) {
                Debug.LogError($"purchasehandler: iap_info not found for id: {id}");
                return;
            }

            // 1. grant reward
            var reward = iapData.RewardContent;

            // Log analytics for real purchases (not restoration on startup)
            if (!isRestoringPurchase) {
                _analyticsService.LogEvent(AnalyticsEventName.PurchaseSuccess, id);
            }

            // don't show ui popup if restoring
            _rewardHandler.HandlerReward(reward, "purchase", !isRestoringPurchase);

            // 2. consume if needed
            if (iapData.IsConsumable) {
                Debug.Log($"purchasehandler: consuming item id: {id}");
                _purchaser.ConsumePurchase(id);
            }
        }
    }
}
