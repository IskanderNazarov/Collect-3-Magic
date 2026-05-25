// Файл: Assets/Game/Scripts/Rewards/GameRewardsItems.cs
// Сборка: Game.asmdef

using System;
using __Gameplay;
using Core._RewardPresenter;

namespace Rewards {
    [Serializable]
    public class CoinsReward : IRewardItem {
        public int Count;
    }

    [Serializable]
    public class BoosterReward : IRewardItem {
        public BoosterId BoosterId; 
        public int Count;
    }
}
