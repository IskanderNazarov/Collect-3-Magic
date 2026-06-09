using __Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _UI {
    public class BuyBoosterDialog : Dialog {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _buyButton;
        
        public void Setup(BoosterId boosterId) {
            _titleText.text = "Buy " + boosterId;
            // Setup icon and price from config if needed
        }
    }
}