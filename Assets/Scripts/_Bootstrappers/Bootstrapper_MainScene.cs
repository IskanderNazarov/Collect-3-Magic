using __CoreGameLib._Scripts._Services._Leaderboards;
using _game;
using UnityEngine;
using Zenject;

namespace _Bootstrappers {
    public class Bootstrapper_MainScene : MonoBehaviour {
        [Inject] private GameManager _gameManager;
        [Inject] private ILeaderboardService _leaderboardService;


        private void Start() {
            _gameManager.Initialize();
        }
    }
}
