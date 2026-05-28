using System;
using UnityEngine;

namespace _game._GameViews {
    public class ItemView : MonoBehaviour {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D itemCollider;
        
        public ItemType Type { get; private set; }
        public Action<ItemView> OnClicked;

        public void Setup(ItemType type, Sprite sprite) {
            Type = type;
            spriteRenderer.sprite = sprite;
            
            // Ensure proper layering/distance for clicks
            var localPos = transform.localPosition;
            localPos.z = -3f;
            transform.localPosition = localPos;
            
            var localScale = transform.localScale;
            localScale.z = 1f;
            transform.localScale = localScale;

            if (itemCollider != null) {
                itemCollider.enabled = true;
            }
        }

        public void DisableInteraction() {
            if (itemCollider != null) {
                itemCollider.enabled = false;
            }
        }

        private void OnMouseDown() {
            print("OnMouseDown");
            OnClicked?.Invoke(this);
        }
    }
}
