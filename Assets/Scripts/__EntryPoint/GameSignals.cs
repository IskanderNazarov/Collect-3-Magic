using __Gameplay;
using UnityEngine;

namespace _Signals {
    public class LevelStartedSignal { }
    public class LevelCompletedSignal { }
    public class LevelFailedSignal { }
    
    public class ItemCollectedSignal { 
        public int ItemId { get; }
        public bool ToContainer { get; }
        
        public ItemCollectedSignal(int itemId, bool toContainer) {
            ItemId = itemId;
            ToContainer = toContainer;
        }
    }
    
    public class CoinsChangedSignal {
        public int NewValue { get; }
        public CoinsChangedSignal(int newValue) { NewValue = newValue; }
    }

    public class UseBoosterSignal {
        public BoosterId BoosterId { get; }
        public Vector3 ButtonPosition { get; }
        public UseBoosterSignal(BoosterId id, Vector3 pos) {
            BoosterId = id;
            ButtonPosition = pos;
        }
    }

    public class ShowBuyBoosterSignal {
        public BoosterId BoosterId { get; }
        public ShowBuyBoosterSignal(BoosterId id) => BoosterId = id;
    }
}