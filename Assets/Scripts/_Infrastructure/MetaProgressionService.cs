using System;
using _Data;
using UnityEngine;
using Zenject;

namespace _Infrastructure {
    public class MetaProgressionService {
        private readonly PlayerProgressService _playerProgress;
private readonly GameplayConfig _gameplayConfig;

        public MetaProgressionService(PlayerProgressService playerProgress, GameplayConfig gameplayConfig) {
            _playerProgress = playerProgress;
            _gameplayConfig = gameplayConfig;
        }

        public int UnlockedCrittersCount => _playerProgress.Data.UnlockedCrittersCount;
        public int TotalStars => _playerProgress.Data.CurrentStars;

        public bool IsFirstCritterUnlocked => UnlockedCrittersCount >= 1;
        public bool IsSecondCritterUnlocked => UnlockedCrittersCount >= 2;

        public CritterData GetCurrentTargetCritter() {
            int targetIndex = UnlockedCrittersCount; // Rabbit is 0 (already unlocked), so next is index 1
            if (targetIndex < _gameplayConfig.CritterDataList.Count) {
                return _gameplayConfig.CritterDataList[targetIndex];
            }
            return null; // All unlocked
        }

        public void AddStars(int count) {
            _playerProgress.AddStars(count);
            CheckUnlocks();
        }

        public bool CanUnlockNext() {
            var target = GetCurrentTargetCritter();
            if (target == null) return false;
            return TotalStars >= target.StarsToUnlock;
        }

        public bool TryUnlockNext() {
            if (CanUnlockNext()) {
                _playerProgress.Data.UnlockedCrittersCount++;
                _playerProgress.MarkDirty();
                _playerProgress.SaveImmediate();
                return true;
            }
            return false;
        }

        private void CheckUnlocks() {
            // Automatic unlock logic if preferred, or manual via UI
        }
    }
}