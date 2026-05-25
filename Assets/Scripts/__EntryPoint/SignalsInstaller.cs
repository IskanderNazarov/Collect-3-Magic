using Zenject;

namespace _Zenject {
    public class SignalsInstaller : MonoInstaller {

        public override void InstallBindings() {
            SignalBusInstaller.Install(Container);
        }
    }
}
