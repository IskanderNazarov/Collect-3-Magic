using System;
using System.Collections.Generic;
using _Infrastructure._SoundsManagement;
using Core._Services;
using Core._Services.SoundManagement;
using Hellmade.Sound;
using UnityEngine;
using Zenject;

namespace Game.SoundManagement {
    public class AudioService : IDisposable {
        private readonly SoundManager _soundManager;
        private readonly SoundsDatabase _database;
        private readonly Dictionary<GameSoundId, SoundInfo> _soundsMap;
        public SoundInfo _arrowSound;

        private Audio _bgAudio;

        private List<string> _clickMelody = new() {
            // Восходящее движение
            "C5",
            "E5",
            "G5",
            "C6",
            "E6",
            "D6",
            "C6",
            "B5",
            "A5",
            "G5",
            // Развитие с переходом в верхний регистр
            "F5",
            "A5",
            "C6",
            "F6",
            "E6",
            "D6",
            "C6",
            "B5",
            "G5",
            "E5",
            // Кульминация на высоких нотах
            "A5",
            "C6",
            "E6",
            "A6",
            "G6",
            "F6",
            "E6",
            "D6",
            "B5",
            "G5",
            // Плавный спуск
            "C6",
            "E6",
            "G6",
            "C7",
            "B6",
            "A6",
            "G6",
            "F6",
            "E6",
            "D6",
            // Завершение и подготовка к зацикливанию
            "C6",
            "G5",
            "E5",
            "C5",
            "G5",
            "B5",
            "C6",
            "E6",
            "G6",
            "C7"
        };


        [Inject]
        private AudioService(SoundManager soundManager, SoundsDatabase database) {
            _soundManager = soundManager;
            _database = database;

            // Собираем быстрый словарь
            _soundsMap = new Dictionary<GameSoundId, SoundInfo>();
            foreach (var entry in _database.Entries) {
                _soundsMap.TryAdd(entry.Id, entry.Info);
            }

        }
        public void Initialize() {
            Debug.Log("AudioService Initialize");
            _soundManager.OnMusicStateChanged += OnMusicStateChanged;
            PlayBgMusic();
        }

        public void Dispose() {
            _soundManager.OnMusicStateChanged -= OnMusicStateChanged;
        }

        private void OnMusicStateChanged(bool isOn) {
            Debug.Log($"Ads__ OnMusicStateChanged: {isOn}");
            if (isOn) PlayBgMusic();
            else StopBgMusic();
        }

        public void PlayBgMusic() {
            StopBgMusic();
            if (_database.MainBgMusic != null) {
                _bgAudio = _soundManager.PlayMusic(_database.MainBgMusic);
            }
        }

        public void StopBgMusic() => _bgAudio?.Stop();

        public void Play(SoundInfo soundInfo, float pitch = 1) {
            if (soundInfo != null) {
                _soundManager.PlaySound(soundInfo, pitch);
            }
        }

        // --- ГЛАВНЫЙ МЕТОД ---
        public void Play(GameSoundId soundId) {
            if (_soundsMap.TryGetValue(soundId, out var info)) {
                _soundManager.PlaySound(info);
            }
            else {
                Debug.LogWarning($"[AudioService] Sound {soundId} is missing in SoundsDatabase!");
            }
        }

        // --- МЕТОД ДЛЯ РАКЕТ (С ИЗМЕНЕНИЕМ ПИТЧА) ---
        // Использование: _audioService.PlayWithPitch(GameSoundId.RocketMove, 1.5f);
        public void PlayWithPitch(GameSoundId soundId, float customPitch) {
            if (_soundsMap.TryGetValue(soundId, out var info)) {
                _soundManager.PlaySound(info, customPitch); // Безопасно передаем питч
            }
        }
    }
}
