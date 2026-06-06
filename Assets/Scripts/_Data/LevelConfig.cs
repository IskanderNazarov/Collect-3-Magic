using System.Collections.Generic;
using _game;
using UnityEngine;

namespace _Data {
    [System.Serializable]
    public class BubbleSpawnWeight {
        public BubbleSize size;
        public float weight;
        public int itemsCount;
    }

    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Scriptables/LevelConfig")]
    public class LevelConfig : ScriptableObject {
        public int totalItemsCount = 30; // Should be multiple of 3
        [SingleEnum]public List<ItemType> availableTypes;
        public int containerCount = 3;
        public int slotCount = 5;
        public float bubbleSpawnWidth = 5f;
        public float spawnHeightOffset = 10f;
        
        public List<BubbleSpawnWeight> bubbleWeights;
    }
}