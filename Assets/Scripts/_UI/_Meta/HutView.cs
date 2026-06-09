using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace _UI._Meta {
    public class HutView : MonoBehaviour {
        [SerializeField] private Image _icon;
        [SerializeField] private Image _progressBar;
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private GameObject _lockOverlay;
        [SerializeField] private GameObject _unlockEffect;

        public void Setup(Sprite icon, int currentStars, int requiredStars, bool isUnlocked) {
            if (_icon != null) _icon.sprite = icon;
            
            if (isUnlocked) {
                if (_lockOverlay != null) _lockOverlay.SetActive(false);
                if (_progressBar != null) _progressBar.transform.parent.gameObject.SetActive(false);
            } else {
                if (_lockOverlay != null) _lockOverlay.SetActive(true);
                if (_progressBar != null) {
                    _progressBar.transform.parent.gameObject.SetActive(true);
                    _progressBar.fillAmount = requiredStars > 0 ? (float)currentStars / requiredStars : 1f;
                }
                if (_progressText != null) _progressText.text = $"{currentStars}/{requiredStars}";
            }
        }

        public void AnimateStarArrival(Vector3 starStartPos, System.Action onComplete) {
            // Star VFX and fly to progress bar
            onComplete?.Invoke();
        }

        public void PlayUnlockEffect() {
            if (_unlockEffect != null) {
                _unlockEffect.SetActive(true);
                // Play some animation
            }
            if (_lockOverlay != null) _lockOverlay.SetActive(false);
            if (_progressBar != null) _progressBar.transform.parent.gameObject.SetActive(false);
        }
    }
}