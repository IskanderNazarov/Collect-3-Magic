// file: BoosterUseService.cs

using System;
using __Gameplay;
using UnityEngine;
using Zenject;

namespace _Infrastructure._Boosters {
    public class BoosterUseService {
        [Inject] private GameplayController _controller;
        [Inject] private BoosterVisualService _boosterVisual;
        [Inject] private BoostersConfig _config;

        public void TryUseBooster(BoosterId boosterId, Vector3 buttonScreenPos, Action onConsumed) {
switch (boosterId) {
            }
        }
    }
}