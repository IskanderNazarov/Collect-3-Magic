using __CoreGameLib._Scripts._Services._Leaderboards;
using _Infrastructure.Services._Leaderboards;
using UnityEngine;
using Zenject;

namespace _Infrastructure.Services {
    public class Bootstrapper_MainScene : MonoBehaviour {
        //[Inject] private GameManager _gameManager;
        [Inject] private ILeaderboardService _leaderboardService;


        private void Start() {
            //_gameManager.Initialize();
        }
    }
}