using System.Collections.Generic;

namespace _game._GameModel {
    public class BubbleModel {
        public BubbleSize Size { get; set; }
        public List<ItemModel> Items { get; set; } = new List<ItemModel>();
    }
}
