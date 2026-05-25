using System;
using UnityEngine;

namespace _UI {
    public abstract class BaseScreen : MonoBehaviour {

        // event for managers to track screen completion
        public event Action OnClosed;

        // instant show
        public void Show() => Show(null);
        public virtual void Show(Action onComplete) {
            gameObject.SetActive(true);
            onComplete?.Invoke();
        }

        // instant hide
        public void Hide() => Hide(null);
        public virtual void Hide(Action onComplete) {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }

        // useful for initial setup
        public virtual void HideImmediate() {
            gameObject.SetActive(false);
        }

        // helper for child classes to trigger the event safely
        protected void NotifyClosed() {
            OnClosed?.Invoke();
        }
    }
}
