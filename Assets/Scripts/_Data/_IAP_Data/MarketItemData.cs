using Core._RewardPresenter;
using UnityEngine;

namespace Core._Purchasing {
    [CreateAssetMenu(fileName = "MarketData", menuName = "Data/Market/MarketData")]
    public class MarketItemData : ScriptableObject {
        // string from store or custom id for rewarded
        public string ProductId;
        public bool IsRewardedVideo;
        public bool IsConsumable;
        public Reward RewardContent;
    }
}