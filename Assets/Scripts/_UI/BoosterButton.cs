using __Gameplay;
using _Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _UI {
    [RequireComponent(typeof(Button))]
    public class BoosterButton : MonoBehaviour {
        [SerializeField] private BoosterId _boosterId;
        private Button _button;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void Awake() {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClicked);
        }

        private void OnClicked() {
            _signalBus.Fire(new UseBoosterSignal(_boosterId, transform.position));
        }
    }
}
