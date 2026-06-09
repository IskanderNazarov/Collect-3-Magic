using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _UI {
    public class Dialog : BaseScreen {
        [SerializeField] private Transform _panel;
        [SerializeField] private Image _darkBg;
        [SerializeField] private float _showHideDur = 0.55f;
        [SerializeField] private GameObject[] _itemsToShowInstantly;


        private GraphicRaycaster _raycaster;
        private Color _darkBgOriginColor;
        private bool _isInited;

        private void Init() {
            ColorUtility.TryParseHtmlString("#00001DB2", out _darkBgOriginColor);
            if (_darkBg != null) _darkBgOriginColor = _darkBg.color;
            _raycaster = GetComponent<GraphicRaycaster>();
        }


        public override void Show(Action onShowFinished) {
            if (!_isInited) {
                _isInited = true;
                Init();
            }
            foreach (var o in _itemsToShowInstantly) o.SetActive(false);

            gameObject.SetActive(true);
            StartCoroutine(PlayShowAnim(onShowFinished));
        }

        public override void Hide(Action onComplete) {
            foreach (var o in _itemsToShowInstantly) o.SetActive(false);
            StartCoroutine(HideAnim(onComplete));
        }

        protected virtual IEnumerator PlayShowAnim(Action onShowFinished = null) {
            StopAnim();
            EnableInteraction(false);

            if (_panel != null) {
                _panel.localScale = Vector3.one;
                _panel.DOPunchScale(Vector3.one * 0.1f, _showHideDur, 2);
            }

            if (_darkBg != null) {
                _darkBg.DOFade(_darkBgOriginColor.a, 0.5f).From(0);
            }

            yield return new WaitForSeconds(_showHideDur);
            EnableInteraction(true);
            foreach (var o in _itemsToShowInstantly) o.SetActive(true);
            onShowFinished?.Invoke();
        }

        protected virtual IEnumerator HideAnim(Action onCompleted) {
            StopAnim();
            EnableInteraction(false);

            if (_panel != null) {
                _panel.DOScale(Vector3.zero, _showHideDur).SetEase(Ease.InBack);
            }

            if (_darkBg != null) {
                //_darkBg.DOColor(_darkBgOriginColor, 0.5f).From(new Color(0, 0, 0, 0));
                _darkBg.DOFade(0, 0.5f);
            }

            yield return new WaitForSeconds(_showHideDur);
            gameObject.SetActive(false);
            onCompleted?.Invoke();
        }

        private void StopAnim() {
            if (_panel != null) _panel.DOKill();
            if (_darkBg != null) _darkBg.DOKill();
        }

        public void EnableInteraction(bool enable) {
            _raycaster.enabled = enable;
        }
    }
}
