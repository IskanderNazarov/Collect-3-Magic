using System.Collections.Generic;
using __Gameplay;
using _Infrastructure._SoundsManagement;
using _Services._Localization;
using _UI._RewardPresenter;
using Core._Purchasing;
using Core._Rewards;
using Core._Services.SoundManagement;
using core.purchasing;
using core.rewards;
using Game.SoundManagement;
using UnityEngine;
using Zenject;

namespace _Zenject {
    public class ProjectContextInstaller : MonoInstaller {
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private MarketConfig _marketConfig;
        [SerializeField] private LocalesSettings _localesSettings;
        [SerializeField] private BoostersConfig _boostersConfig;
        [SerializeField] private SoundsDatabase _soundsDatabase;
        [SerializeField] private RewardPresenterView _rewardPresenterView;

        public override void InstallBindings() {
            Container.Bind<Localizer>().FromNew().AsSingle().WithArguments(_localesSettings).NonLazy();

            Container.Bind<GameConfig>().FromScriptableObject(_gameConfig).AsSingle().NonLazy();
            Container.Bind<MarketConfig>().FromScriptableObject(_marketConfig).AsSingle().NonLazy();
            Container.Bind<SoundsDatabase>().FromScriptableObject(_soundsDatabase).AsSingle().NonLazy();

            Container.Bind<ISoundStateProvider>().To<GameSoundStateProvider>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<AudioService>().FromNew().AsSingle();
            Container.Bind<IRewardAudioPlayer>().To<GameRewardAudioPlayer>().AsSingle();

            Container.BindInterfacesAndSelfTo<PlayerProgressService>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PurchaseHandler>().AsSingle().NonLazy();

            Container.Bind<IRewardApplier>().To<GameRewardApplier>().FromNew().AsSingle().NonLazy();
            Container.Bind<IRewardPresenter>().To<RewardPresenter>().AsSingle()
                .WithArguments(_rewardPresenterView, Container).NonLazy();

            InstallBoostersServices();
        }

        private void InstallBoostersServices() {
            //todo default values are set in the keys storage
            var caps = new Dictionary<BoosterId, int> {
                /*{ BoosterId.Hint, 500 },
                { BoosterId.Hammer, 500 },
                { BoosterId.Ruler, 500 },*/
            };
            Container.BindInterfacesAndSelfTo<GameBoosterInventory>().AsSingle().WithArguments( /*defaultValues, */caps);
            Container.Bind<BoostersConfig>().FromScriptableObject(_boostersConfig).AsSingle();
        }
    }
}
