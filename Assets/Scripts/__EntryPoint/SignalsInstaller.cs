using _Signals;
using Zenject;

namespace _Zenject {
    public class SignalsInstaller : MonoInstaller {

        public override void InstallBindings() {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<LevelStartedSignal>();
            Container.DeclareSignal<LevelCompletedSignal>();
            Container.DeclareSignal<LevelFailedSignal>();
            Container.DeclareSignal<ItemCollectedSignal>();
            Container.DeclareSignal<CoinsChangedSignal>();
            Container.DeclareSignal<UseBoosterSignal>();
            Container.DeclareSignal<ShowBuyBoosterSignal>();
        }
}
}
