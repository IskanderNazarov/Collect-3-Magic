using Core._Services._Saving;
using core.purchasing;
using UnityEngine;
using Zenject;
public class GameUI : MonoBehaviour {


    [Inject]
    private void Construct(IDataSaver dataSaver, GameConfig gameConfig,
        RewardHandler rewardHandler) {
    }

}
