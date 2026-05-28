using _Data;
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

        public override void InstallBindings() {
            Container.BindInstance(_uiManager).AsSingle();
            Container.BindInstance(_gameplayView).AsSingle();
            Container.BindInstance(_levelConfig).AsSingle();
            Container.BindInstance(_gameplayConfig).AsSingle();
            
            Container.BindInterfacesAndSelfTo<GameplayController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameManager>().AsSingle().NonLazy();
        }
    }
}
