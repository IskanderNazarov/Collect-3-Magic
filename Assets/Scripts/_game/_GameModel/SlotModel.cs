namespace _game._GameModel {
    public class SlotModel {
        public int Index { get; private set; }
        public bool IsOccupied { get; private set; }
        public ItemType? OccupiedType { get; private set; }

        public SlotModel(int index) {
            Index = index;
            IsOccupied = false;
        }

        public void Occupy(ItemType type) {
            IsOccupied = true;
            OccupiedType = type;
        }

        public void Clear() {
            IsOccupied = false;
            OccupiedType = null;
        }
    }
}