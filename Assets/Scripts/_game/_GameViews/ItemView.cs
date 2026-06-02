using System;
using UnityEngine;

namespace _game._GameViews {
    public class ItemView : MonoBehaviour {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D itemCollider;
        
        public ItemType Type { get; private set; }
        public Action<ItemView> OnClicked;

        public void Setup(ItemType type, Sprite sprite, Color color) {
            Type = type;
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = color;
            
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
