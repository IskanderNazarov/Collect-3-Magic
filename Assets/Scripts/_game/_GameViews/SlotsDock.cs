using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace _game._GameViews {
    public class SlotsDock : MonoBehaviour {
        private List<SlotView> _slotPositions;
        private readonly List<ItemView> _items = new List<ItemView>();
        
        public int OccupiedCount => _items.Count;
        public int Capacity => _slotPositions.Count;

        public void Initialize(List<SlotView> slotPositions) {
            _slotPositions = slotPositions;
            _items.Clear();
        }

        public bool TryAddItem(ItemView item, float travelDuration, Action onComplete) {
            if (_items.Count >= Capacity) return false;

            var targetIndex = _items.Count;
            _items.Add(item);
            
            item.transform.SetParent(transform);
            item.DisableInteraction();

            // Reset physics state
            var rb = item.GetComponent<Rigidbody2D>();
            if (rb != null) {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.simulated = false;
            }
            
            var targetSlot = _slotPositions[targetIndex];
            //var targetPos = targetSlot.transform.position;
            var targetPos = targetSlot.ItemTargetPos.position;
            // Add a small Z offset to be in front of the dock
            targetPos.z = transform.position.z - 3f;

            //var targetScale = targetSlot.transform.localScale;
            var targetScale = targetSlot.ItemTargetPos.localScale;

            // ANIMATION CORE: Fly, Scale and Rotate to 0
            item.transform.DOMove(targetPos, travelDuration).SetEase(Ease.OutQuad);
            item.transform.DOScale(targetScale, travelDuration).SetEase(Ease.OutQuad);
            item.transform.DORotate(Vector3.zero, travelDuration).SetEase(Ease.OutQuad).OnComplete(() => {
                onComplete?.Invoke();
            });
            
            return true;
        }

        public List<ItemView> ExtractItems(ItemType type, int maxCount) {
            var matchingItems = _items.Where(i => i.Type == type).Take(maxCount).ToList();
            
            foreach (var item in matchingItems) {
                _items.Remove(item);
            }

            if (matchingItems.Count > 0) {
                ShiftItems();
            }

            return matchingItems;
        }

        private void ShiftItems() {
            for (var i = 0; i < _items.Count; i++) {
                var targetSlot = _slotPositions[i];
                var targetPos = targetSlot.transform.position;
                targetPos.z = transform.position.z - 3f;
                
                var targetScale = targetSlot.transform.localScale;

                _items[i].transform.DOMove(targetPos, 0.3f).SetEase(Ease.OutQuad);
                _items[i].transform.DOScale(targetScale, 0.3f).SetEase(Ease.OutQuad);
                // Rotation should already be zero, but we can enforce it just in case
                _items[i].transform.DORotate(Vector3.zero, 0.3f).SetEase(Ease.OutQuad);
            }
        }

        public ItemView GetItemAt(int index) {
            if (index >= 0 && index < _items.Count) return _items[index];
            return null;
        }
        
        public IEnumerable<ItemView> GetAllItems() => _items;
    }
}