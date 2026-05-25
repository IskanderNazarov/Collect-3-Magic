// File: Assets/Game/Scripts/Rewards/GameRewardApplier.cs

using Core._RewardPresenter;
using Rewards;
using Zenject;

namespace core.rewards {
    public class GameRewardApplier : IRewardApplier {
        [Inject] private PlayerProgressService _playerProgress;

        public void ApplyReward(Reward reward, string placement) {
            if (reward == null || reward.Items == null) return;

            foreach (var item in reward.Items) {
                if (item is CoinsReward coins) {
                    _playerProgress.AddCoins(coins.Count);
                }
                else if (item is BoosterReward booster) {
                    var currentCount = _playerProgress.GetBoosterCount((int)booster.BoosterId);
                    _playerProgress.SetBoosterCount((int)booster.BoosterId, currentCount + booster.Count);
                }
                // add other types here (e.g. skins)
            }
        }
    }
}
