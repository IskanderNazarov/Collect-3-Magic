namespace _game._GameModel {
    public class ContainerModel {
        public ItemType TargetType { get; private set; }
        public int CurrentCount { get; private set; }
        public bool IsFull => CurrentCount >= 3;

        public ContainerModel(ItemType targetType) {
            TargetType = targetType;
            CurrentCount = 0;
        }

        public void AddItem() {
            CurrentCount++;
        }

        public void Reset(ItemType newType) {
            TargetType = newType;
            CurrentCount = 0;
        }
    }
}