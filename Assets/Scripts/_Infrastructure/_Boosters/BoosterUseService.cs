// file: BoosterUseService.cs

using System;
using __Gameplay;
using UnityEngine;
using _game;
using Zenject;
using _Signals;

namespace _Infrastructure._Boosters {
    public class BoosterUseService {
        [Inject] private GameplayController _controller;
        [Inject] private BoosterVisualService _boosterVisual;
        [Inject] private BoostersConfig _config;
        [Inject] private PlayerProgressService _playerProgress;
        [Inject] private SignalBus _signalBus;

        public void TryUseBooster(BoosterId boosterId, Vector3 buttonScreenPos, Action onConsumed) {
            if (_controller.IsInputBlocked) return;

            var count = 100;//_playerProgress.GetBoosterCount((int)boosterId);
            if (count > 0) {
                ExecuteBooster(boosterId, onConsumed);
            } else {
                _signalBus.Fire(new ShowBuyBoosterSignal(boosterId));
            }
        }

        private void ExecuteBooster(BoosterId boosterId, Action onConsumed) {
            switch (boosterId) {
                case BoosterId.Shuffle:
                    _boosterVisual.ExecuteShuffle(() => {
                        _playerProgress.SetBoosterCount((int)boosterId, _playerProgress.GetBoosterCount((int)boosterId) - 1);
                        onConsumed?.Invoke();
                    });
                    break;
                case BoosterId.Remover:
                    _boosterVisual.ExecuteRemover(() => {
                        _playerProgress.SetBoosterCount((int)boosterId, _playerProgress.GetBoosterCount((int)boosterId) - 1);
                        onConsumed?.Invoke();
                    });
                    break;
            }
        }
    }
}