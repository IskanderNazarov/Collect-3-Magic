using _UI._RewardPresenter;
using Game.SoundManagement;

namespace _Infrastructure._SoundsManagement {
    public class GameRewardAudioPlayer : IRewardAudioPlayer {
        private readonly AudioService _audioService;

        public GameRewardAudioPlayer(AudioService audioService) {
            _audioService = audioService;
        }

        public void PlayRewardShowSound() {
            _audioService.Play(GameSoundId.RewardShow);
        }

        public void PlayItemSpawnSound() {
            _audioService.Play(GameSoundId.RewardSpawn);
        }
    }
}