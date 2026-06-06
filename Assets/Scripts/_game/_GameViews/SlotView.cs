using UnityEngine;

namespace _game._GameViews {
    public class SlotView : MonoBehaviour {
        [SerializeField] private Transform _itemTargetPos;
        
        public Transform ItemTargetPos => _itemTargetPos;
    }
}
