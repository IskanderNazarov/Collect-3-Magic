using _Data;
using _Infrastructure;
using _Infrastructure._Boosters;
using _UI;
using _game;
using _game._GameViews;
using UnityEngine;
using Zenject;

namespace _Zenject {
    public class MainSceneInstaller : MonoInstaller {
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private GameplayView _gameplayView;
        [SerializeField] private LevelConfig _levelConfig;
        [SerializeField] private GameplayConfig _gameplayConfig;
        [SerializeField] private BoosterVisualService _boosterVisualService;

        public override void InstallBindings() {
            Container.BindInstance(_uiManager).AsSingle();
            Container.BindInstance(_gameplayView).AsSingle();
            Container.BindInstance(_levelConfig).AsSingle();
            Container.BindInstance(_gameplayConfig).AsSingle();
            Container.BindInstance(_boosterVisualService).AsSingle();
            
            Container.BindInterfacesAndSelfTo<GameplayController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BoosterUseService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MetaProgressionService>().AsSingle().NonLazy();
        }
    }
}
