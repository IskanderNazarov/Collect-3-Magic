// File: Assets/Game/Scripts/Rewards/UI/RewardPresentView.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core._Rewards.UI {
    public class RewardPresentItem : MonoBehaviour {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _amountText;

        public void Setup(Sprite icon, int amount) {
            _iconImage.sprite = icon;
            _amountText.text = $"x{amount}";
            gameObject.SetActive(true);
        }
    }
}