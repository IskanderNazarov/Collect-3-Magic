using System;
using _game;
using UnityEngine;

namespace _Data {
    [Serializable]
    public class CritterData {
        public CritterType critterType;
        public Sprite CritterSprite;
        // Flags to define which items this critter can request. 
        public ItemType AllowedItems = ItemType.All; 
    }
}
