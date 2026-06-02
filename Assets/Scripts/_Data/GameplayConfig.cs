using System.Collections.Generic;
using System.Linq;
using _game;
using UnityEngine;

namespace _Data {
    [CreateAssetMenu(fileName = "GameplayConfig", menuName = "Scriptables/GameplayConfig")]
    public class GameplayConfig : ScriptableObject {
        [Header("Item Visuals")] [SerializeField]
        private List<ItemData> itemDataList;

        /// <summary>
        /// Возвращает спрайт для конкретного типа предмета.
        /// </summary>
        public ItemData GetItemData(ItemType type) {
            var data = itemDataList.FirstOrDefault(s => s.type == type);
            if (data == null) {
                Debug.LogError($"[GameplayConfig] No data found for ItemType: {type}");
                return null;
            }

            return data;
        }

        public Sprite GetSprite(ItemType type) {
            var data = itemDataList.FirstOrDefault(s => s.type == type);
            if (data == null) {
                Debug.LogError($"[GameplayConfig] No sprite found for ItemType: {type}");
                return null;
            }

            return data.sprite;
        }

        // Сюда же можно добавить другие глобальные настройки:
        // public float itemFlyDuration = 0.5f;
        // public AnimationCurve flyCurve;
    }
}