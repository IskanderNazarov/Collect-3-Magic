using System.Collections.Generic;
using UnityEngine;

namespace _game._GameViews {
    public class ContainerView : MonoBehaviour {
        [SerializeField] private SpriteRenderer targetIconRenderer;
        [SerializeField] private List<Transform> fruitPositions;

        public ItemType TargetType { get; private set; }
        
        public Vector3 GetPositionForIndex(int index) {
            if (index >= 0 && index < fruitPositions.Count) {
                return fruitPositions[index].position;
            }
            return transform.position;
        }

        public void SetTarget(ItemType type, Sprite icon) {
            TargetType = type;
            if (targetIconRenderer != null) {
                targetIconRenderer.sprite = icon;
            }
        }
    }
}
