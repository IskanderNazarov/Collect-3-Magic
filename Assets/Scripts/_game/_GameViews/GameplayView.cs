using System.Collections.Generic;
using UnityEngine;

namespace _game._GameViews {
    public class GameplayView : MonoBehaviour {
        [Header("Prefabs")]
        public ItemView ItemPrefab;
        public BubbleView SmallBubblePrefab;
        public BubbleView BigBubblePrefab;

        [Header("Scene Refs")]
        public Transform PoolContainer;
        public BucketView Bucket;
        public List<ContainerView> Containers;
        public List<SlotView> Slots;
    }
}
