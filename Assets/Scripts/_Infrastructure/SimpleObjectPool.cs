using System.Collections.Generic;
using UnityEngine;

namespace _Infrastructure {
    public class SimpleObjectPool<T> where T : Component {
        private readonly T _prefab;
        private readonly Transform _container;
        private readonly Stack<T> _pool = new Stack<T>();

        public SimpleObjectPool(T prefab, int initialSize, Transform container) {
            _prefab = prefab;
            _container = container;

            for (int i = 0; i < initialSize; i++) {
                CreateNewInstance();
            }
        }

        private void CreateNewInstance() {
            T instance = Object.Instantiate(_prefab, _container);
            instance.gameObject.SetActive(false);
            _pool.Push(instance);
        }

        public T Get() {
            if (_pool.Count == 0) {
                CreateNewInstance();
            }
            T instance = _pool.Pop();
            instance.gameObject.SetActive(true);
            return instance;
        }

        public void Release(T instance) {
            instance.gameObject.SetActive(false);
            _pool.Push(instance);
        }
    }
}