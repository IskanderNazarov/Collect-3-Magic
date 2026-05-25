using System;
using _Infrastructure._SoundsManagement;
using Core._Services.SoundManagement;
using UnityEngine;

namespace Game.SoundManagement {
    [CreateAssetMenu(fileName = "SoundsDatabase", menuName = "Audio/SoundsDatabase")]
    public class SoundsDatabase : ScriptableObject {

        [Serializable]
        public struct SoundEntry {
            public GameSoundId Id;
            public SoundInfo Info;
        }

        [Header("Background Music")] public SoundInfo MainBgMusic;
        [Header("SFX Library")] public SoundEntry[] Entries;
    }
}