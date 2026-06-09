using System;
using System.Collections.Generic;
using _Data;
using DG.Tweening;
using UnityEngine;

namespace _game._GameViews {
    public class ContainerView : MonoBehaviour {
        [SerializeField] private SpriteRenderer _targetIconRenderer;
        [SerializeField] private SpriteRenderer _critterIcon;
        [SerializeField] private List<Transform> _itemsPositions;
        
        [Header("Animations")]
        [SerializeField] private float _scaleDuration = 0.3f;
        [SerializeField] private Ease _scaleEase = Ease.OutBack;

        public ItemType TargetType { get; private set; }

        private readonly List<SpriteRenderer> _itemRenderers = new List<SpriteRenderer>();

        private void Awake() {
            foreach (var pos in _itemsPositions) {
                var renderer = pos.GetComponent<SpriteRenderer>();
                if (renderer != null) {
                    _itemRenderers.Add(renderer);
                    renderer.gameObject.SetActive(false);
                }
            }
        }

        public Vector3 GetPositionForIndex(int index) {
            if (index >= 0 && index < _itemsPositions.Count) {
                return _itemsPositions[index].position;
            }
            return transform.position;
        }

        public Vector3 GetScaleForIndex(int index) {
            if (index >= 0 && index < _itemsPositions.Count) {
                return _itemsPositions[index].localScale;
            }
            return Vector3.one;
        }

        public Sequence SetTarget(CritterData critterData, ItemData itemData) {
            TargetType = itemData.type;
            
            // Swap animation sequence
            Sequence swapSequence = DOTween.Sequence();
            
            // 1. Ensure initial state
            _critterIcon.transform.localScale = Vector3.zero;
            if (_targetIconRenderer != null) {
                _targetIconRenderer.transform.parent.localScale = Vector3.zero;
            }

            // 2. Change visuals and scale up
            swapSequence.AppendCallback(() => {
                _critterIcon.sprite = critterData.CritterSprite;
                if (_targetIconRenderer != null) {
                    _targetIconRenderer.sprite = itemData.sprite;
                }
                // Clear existing items visuals
                foreach (var renderer in _itemRenderers) {
                    renderer.gameObject.SetActive(false);
                }
            });

            swapSequence.Append(_critterIcon.transform.DOScale(1, _scaleDuration).SetEase(_scaleEase));
            if (_targetIconRenderer != null) {
                swapSequence.Join(_targetIconRenderer.transform.parent.DOScale(1, _scaleDuration).SetEase(_scaleEase));
            }

            return swapSequence;
        }

        public void ScaleCloud(bool visible, float duration) {
            if (_targetIconRenderer != null) {
                _targetIconRenderer.transform.parent.DOScale(visible ? 1f : 0f, duration).SetEase(Ease.InOutQuad);
            }
        }

        public void ScaleCritter(bool visible, float duration, Action onComplete = null) {
            _critterIcon.transform.DOScale(visible ? 1f : 0f, duration).SetEase(visible ? _scaleEase : Ease.InBack).OnComplete(() => onComplete?.Invoke());
        }

        public void HideItemVisuals() {
            foreach (var renderer in _itemRenderers) {
                renderer.gameObject.SetActive(false);
            }
        }

        public void ClearTarget() {
            TargetType = ItemType.None;
            ScaleCritter(false, _scaleDuration);
            ScaleCloud(false, _scaleDuration);
            HideItemVisuals();
        }

        public void ShowLandedItem(int index, ItemData data) {
            if (index >= 0 && index < _itemRenderers.Count) {
                var renderer = _itemRenderers[index];
                renderer.sprite = data.sprite;
                renderer.gameObject.SetActive(true);
            }
        }
    }
}
