using Core._Services;
using Core._Services._Saving;
using core.ads;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _UI {
    public class SettingsDialog : Dialog {
        [SerializeField] private Button musicBtn;
        [SerializeField] private Button soundBtn;

        [Header("Sprites")] [SerializeField] private Sprite musicOnSprite;
        [SerializeField] private Sprite musicOffSprite;
        [SerializeField] private Sprite soundOnSprite;
        [SerializeField] private Sprite soundOffSprite;
        [SerializeField] private GameObject debugPanel;

        [Inject] private SoundManager _soundManager;
        [Inject] private UIManager _uiManager;
        [Inject] private IDataSaver _dataSaver;
        [Inject] private SignalBus _signalBus;
        [Inject] private IAdsService _adsService;
        [Inject] private PlayerProgressService _playerProgressService;
        private RectTransform _rectTransform;

        private void Start() {
            soundBtn.onClick.AddListener(OnSoundClicked);
            musicBtn.onClick.AddListener(OnMusicClicked);
        }

        private void OnCloseClicked() {
            _adsService.ShowInterstitial(AdPlacementType.AfterLogicPause, null);
            Hide();
        }

        private void OnEnable() {
            UpdateVisuals();
        }

        private void UpdateVisuals() {
            var isMusicOn = _soundManager.IsMusicOn;
            var isSoundOn = _soundManager.IsSoundOn;

            if (musicOnSprite != null && musicOffSprite != null)
                musicBtn.image.sprite = isMusicOn ? musicOnSprite : musicOffSprite;

            if (soundOnSprite != null && soundOffSprite != null)
                soundBtn.image.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
        }

        private void OnDestroy() {
            soundBtn.onClick.RemoveListener(OnSoundClicked);
            musicBtn.onClick.RemoveListener(OnMusicClicked);
        }

        private void OnMusicClicked() {
            var isMusicOn = _soundManager.IsMusicOn;
            _soundManager.SetMusicOn(!isMusicOn);
            UpdateVisuals();
        }

        private void OnSoundClicked() {
            var isSfxOn = _soundManager.IsSoundOn;
            _soundManager.SetSFXOn(!isSfxOn);
            UpdateVisuals();
        }


        private int _debugPanelOpenCounter;

        private int counter;
        public void EnableSolveButton() {
            _debugPanelOpenCounter++;
#if UNITY_EDITOR
            debugPanel.gameObject.SetActive(_debugPanelOpenCounter % 2 == 0);
#else
            debugPanel.gameObject.SetActive(counter % 10 == 0);
#endif
        }


        public void DeleteDebug() {
            _playerProgressService.ResetProgress();
        }
    }
}
