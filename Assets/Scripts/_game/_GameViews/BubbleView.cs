using System.Collections.Generic;
using _game._GameModel;
using UnityEngine;

namespace _game._GameViews {
    public class BubbleView : MonoBehaviour {

        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private List<SlotView> itemSlots;

        public BubbleSize Size { get; private set; }
        public BubbleModel Model { get; private set; }
        public IReadOnlyList<ItemView> Items => _items;
        public IReadOnlyList<SlotView> Slots => itemSlots;
        public int Capacity => itemSlots.Count;
        private readonly List<ItemView> _items = new List<ItemView>();

        public void Setup(BubbleModel model) {
            Model = model;
            Size = model.Size;
            _items.Clear();
            gameObject.SetActive(true);
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        public void AddItem(ItemView item, bool snap = true) {
            if (_items.Count >= itemSlots.Count) return;

            var slot = itemSlots[_items.Count];
            var oldGlobalPos = item.transform.position;
            var oldGlobalRot = item.transform.rotation;
            
            item.transform.SetParent(slot.transform);
            
            if (snap) {
                item.transform.localPosition = new Vector3(0, 0, -3f);
                var localScale = item.transform.localScale;
                localScale.z = 1f;
                item.transform.localScale = localScale;
                item.transform.localRotation = Quaternion.identity;
            } else {
                item.transform.position = oldGlobalPos;
                item.transform.rotation = oldGlobalRot;
            }
            
            _items.Add(item);
        }

        public void RemoveItem(ItemView item) {
            _items.Remove(item);
        }

        public bool IsEmpty => _items.Count == 0;
    }
}
