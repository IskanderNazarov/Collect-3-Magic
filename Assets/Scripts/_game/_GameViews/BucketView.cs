using UnityEngine;

namespace _game._GameViews {
    public class BucketView : MonoBehaviour {
        [SerializeField] private BoxCollider2D bottom;
        [SerializeField] private BoxCollider2D leftWall;
        [SerializeField] private BoxCollider2D rightWall;
    }
}
