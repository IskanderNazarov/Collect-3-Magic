using System.Collections.Generic;
using _Data;
using UnityEngine;

namespace _game._GameViews {
    public class ContainerView : MonoBehaviour {
        [SerializeField] private SpriteRenderer _targetIconRenderer;
        [SerializeField] private List<Transform> _itemsPositions;

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

        public void SetTarget(ItemData data) {
            TargetType = data.type;
            if (_targetIconRenderer != null) {
                _targetIconRenderer.sprite = data.sprite;
                _targetIconRenderer.color = data.color;
            }

            // Clear existing visuals on target change
            foreach (var renderer in _itemRenderers) {
                renderer.gameObject.SetActive(false);
            }
        }

        public void ShowLandedItem(int index, ItemData data) {
            if (index >= 0 && index < _itemRenderers.Count) {
                var renderer = _itemRenderers[index];
                renderer.sprite = data.sprite;
                renderer.color = data.color;
                renderer.gameObject.SetActive(true);
            }
        }
    }
}
