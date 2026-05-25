using _UI;
using UnityEngine;
using Zenject;

namespace _Zenject {
    public class MainSceneInstaller : MonoInstaller {
        [SerializeField] private UIManager _uiManager;

        //todo create game manager and controller here if needed
        
        public override void InstallBindings() {
            //Container.BindInstance(_gameplayController).AsSingle();
            
            Container.BindInstance(_uiManager).AsSingle();
        }
    }
}
