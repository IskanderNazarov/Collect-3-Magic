using __Gameplay;
using _Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _UI {
    public class GameUI : BaseScreen {

        [SerializeField] private Button _changeBgBtn;
        [SerializeField] private SpriteRenderer _spriteRendererMain;
        [SerializeField] private SpriteRenderer _spriteRendererTop;

        [Header("Boosters")]
        [SerializeField] private Button _shuffleBtn;
        [SerializeField] private Button _removerBtn;

        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private int _counter;
        private void Start() {
            _changeBgBtn.onClick.AddListener(() => {
                _counter++;
                var isFistBg = _counter % 2 == 0;
                _spriteRendererMain.gameObject.SetActive(isFistBg);
                _spriteRendererTop.gameObject.SetActive(!isFistBg);
            });

            _shuffleBtn.onClick.AddListener(() => {
                _signalBus.Fire(new UseBoosterSignal(BoosterId.Shuffle, _shuffleBtn.transform.position));
            });

            _removerBtn.onClick.AddListener(() => {
                _signalBus.Fire(new UseBoosterSignal(BoosterId.Remover, _removerBtn.transform.position));
            });
        }
    }
}
