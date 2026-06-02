using System.Collections.Generic;

namespace _game._GameModel {

    public class GameplayModel {
        public List<BubbleModel> Bubbles { get; } = new List<BubbleModel>();
        public List<ContainerModel> Containers { get; } = new List<ContainerModel>();
        public int CompletedMatchesCount { get; set; }

        public void Clear() {
            Bubbles.Clear();
            Containers.Clear();
            CompletedMatchesCount = 0;
        }
    }
}
