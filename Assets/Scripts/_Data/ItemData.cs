using System;
using _game;
using UnityEngine;

namespace _Data {
    [Serializable]
    public class ItemData {
        [SingleEnum] public ItemType type;
        public Sprite sprite;
        public Color color;
    }

    public class SingleEnumAttribute : PropertyAttribute {
    }
}
