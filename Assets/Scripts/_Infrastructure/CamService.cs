using UnityEngine;

namespace core.purchasing {
    public class CamService {
        private readonly Camera _camera;

        private CamService(Camera camera) {
            _camera = camera;
        }
    }
}
