using System.Collections.Generic;
using UnityEngine;

namespace _game._GameViews {
    public class BubbleView : MonoBehaviour {

        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private List<SlotView> itemSlots;

        public BubbleSize Size { get; private set; }
        private readonly List<ItemView> _items = new List<ItemView>();

        public void Setup(BubbleSize size) {
            Size = size;
            _items.Clear();
            gameObject.SetActive(true);
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        public void AddItem(ItemView item) {
            if (_items.Count >= itemSlots.Count) return;

            item.transform.SetParent(itemSlots[_items.Count].transform);
            item.transform.localPosition = Vector3.zero;
            _items.Add(item);
        }

        public void RemoveItem(ItemView item) {
            _items.Remove(item);
        }

        public bool IsEmpty => _items.Count == 0;
    }
}
