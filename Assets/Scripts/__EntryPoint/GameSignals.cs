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
}