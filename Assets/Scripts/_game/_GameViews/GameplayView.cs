using System.Collections.Generic;
using UnityEngine;

namespace _game._GameViews {
    [System.Serializable]
    public class BubblePrefabConfig {
        public BubbleSize size;
        public BubbleView prefab;
        public int initialPoolSize = 10;
    }

    public class GameplayView : MonoBehaviour {
        [Header("Prefabs")]
        public ItemView ItemPrefab;
        public List<BubblePrefabConfig> BubbleConfigs;

        [Header("Scene Refs")]
        public Transform PoolContainer;
        public BucketView Bucket;
        public List<ContainerView> Containers;
        public List<SlotView> Slots;
        public SlotsDock SlotsDock;
    }
}
