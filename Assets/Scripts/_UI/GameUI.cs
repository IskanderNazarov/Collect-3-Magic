using System;
using UnityEngine;
using UnityEngine.UI;

namespace _UI {
    public class GameUI : MonoBehaviour {
    
        [SerializeField] private Button _changeBgBtn;
        [SerializeField] private SpriteRenderer _spriteRendererMain;
        [SerializeField] private SpriteRenderer _spriteRendererTop;

        private int _counter;
        private void Start() {
            _changeBgBtn.onClick.AddListener(() => {
                _counter++;
                var isFistBg = _counter % 2 == 0;
                _spriteRendererMain.gameObject.SetActive(isFistBg);
                _spriteRendererTop.gameObject.SetActive(!isFistBg);
            });
        }

    }
}
