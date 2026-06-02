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
            
            // Enforce Z and Scale
            var localScale = item.transform.localScale;
            localScale.z = 1f;
            item.transform.localScale = localScale;

            var targetPos = _slotPositions[targetIndex].transform.position;
            // Add a small Z offset to be in front of the dock
            targetPos.z = transform.position.z - 3f;

            item.transform.DOMove(targetPos, travelDuration).SetEase(Ease.OutQuad).OnComplete(() => {
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
                var targetPos = _slotPositions[i].transform.position;
                targetPos.z = transform.position.z - 3f;
                _items[i].transform.DOMove(targetPos, 0.3f).SetEase(Ease.OutQuad);
            }
        }

        public ItemView GetItemAt(int index) {
            if (index >= 0 && index < _items.Count) return _items[index];
            return null;
        }
        
        public IEnumerable<ItemView> GetAllItems() => _items;
    }
}