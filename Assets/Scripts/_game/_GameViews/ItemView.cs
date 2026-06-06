using System;
using _Data;
using UnityEngine;

namespace _game._GameViews {
    public class ItemView : MonoBehaviour {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Collider2D _itemCollider;

        public ItemType Type { get; private set; }
        public Action<ItemView> OnClicked;

        private int _originalSortingLayerId;
        private int _originalSortingOrder;

        public void Setup(ItemData itemData) {
            Type = itemData.type;
            _spriteRenderer.sprite = itemData.sprite;

            // Store original sorting values
            _originalSortingLayerId = _spriteRenderer.sortingLayerID;
            _originalSortingOrder = _spriteRenderer.sortingOrder;

            // Ensure proper layering/distance for clicks
            var localPos = transform.localPosition;
            localPos.z = -3f;
            transform.localPosition = localPos;

            var localScale = transform.localScale;
            localScale.z = 1f;
            transform.localScale = localScale;

            // Ensure Kinematic Rigidbody2D for independent click detection from parent bubbles
            var rb = GetComponent<Rigidbody2D>();
            if (rb == null) {
                rb = gameObject.AddComponent<Rigidbody2D>();
            }
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;

            if (_itemCollider != null) {
                _itemCollider.enabled = true;
            }
        }

        public void SetSortingLayer(string layerName) {
            _spriteRenderer.sortingLayerName = layerName;
            _spriteRenderer.sortingOrder = 100; // Put it on top of other things in that layer
        }

        public void ResetSortingLayer() {
            _spriteRenderer.sortingLayerID = _originalSortingLayerId;
            _spriteRenderer.sortingOrder = _originalSortingOrder;
        }

        public void DisableInteraction() {
            if (_itemCollider != null) {
                _itemCollider.enabled = false;
            }
        }

        private void OnMouseDown() {
            OnClicked?.Invoke(this);
        }
    }
}
