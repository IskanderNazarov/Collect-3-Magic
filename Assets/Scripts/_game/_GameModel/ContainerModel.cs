namespace _game._GameModel {
    public class ContainerModel {
        public ItemType TargetType { get; private set; }
        public int ReservedCount { get; private set; }
        public int LandedCount { get; private set; }
        
        public bool IsFull => ReservedCount >= 3;
        public bool AllLanded => LandedCount >= 3;

        public ContainerModel(ItemType targetType) {
            TargetType = targetType;
            ReservedCount = 0;
            LandedCount = 0;
        }

        public void ReserveItem() {
            ReservedCount++;
        }

        public void ItemLanded() {
            LandedCount++;
        }

        public void Reset(ItemType newType) {
            TargetType = newType;
            ReservedCount = 0;
            LandedCount = 0;
        }
    }
}