// File: Assets/Game/Scripts/Purchasing/MarketConfig.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Core._RewardPresenter;
using UnityEngine;

namespace Core._Purchasing {
    [CreateAssetMenu(fileName = "MarketConfig", menuName = "Data/Market/MarketConfig")]
    public class MarketConfig : ScriptableObject {
        public List<MarketItemData> Items;

        public MarketItemData GetItem(string id) {
            return Items.FirstOrDefault(i => i.ProductId == id);
        }
    }
}