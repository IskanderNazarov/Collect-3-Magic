using System;
using System.Collections.Generic;
using System.Linq;
using _Data;
using _game._GameModel;
using _game._GameViews;
using _Infrastructure;
using _Signals;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;
using DG.Tweening;

namespace _game {
    public class GameplayController {
        private readonly SignalBus _signalBus;
        private readonly GameplayView _view;
        private readonly LevelConfig _levelConfig;
        private readonly GameplayConfig _gameplayConfig;

        private SimpleObjectPool<ItemView> _itemPool;
        private SimpleObjectPool<BubbleView> _smallBubblePool;
        private SimpleObjectPool<BubbleView> _bigBubblePool;

        private readonly GameplayModel _model = new GameplayModel();

        private const float TravelDuration = 0.5f;

        public GameplayController(
            SignalBus signalBus,
            GameplayView view,
            LevelConfig levelConfig,
            GameplayConfig gameplayConfig) {
            _signalBus = signalBus;
            _view = view;
            _levelConfig = levelConfig;
            _gameplayConfig = gameplayConfig;
        }

        public void Initialize() {
            InitializePools();
            GenerateLevel();
        }

        private void InitializePools() {
            _itemPool = new SimpleObjectPool<ItemView>(_view.ItemPrefab, 50, _view.PoolContainer);
            _smallBubblePool = new SimpleObjectPool<BubbleView>(_view.SmallBubblePrefab, 20, _view.PoolContainer);
            _bigBubblePool = new SimpleObjectPool<BubbleView>(_view.BigBubblePrefab, 10, _view.PoolContainer);
        }

        public void GenerateLevel() {
            _model.Clear();

            var itemsToPack = new List<ItemType>();
            for (var i = 0; i < _levelConfig.totalItemsCount / 3; i++) {
                var type = _levelConfig.availableTypes[Random.Range(0, _levelConfig.availableTypes.Count)];
                itemsToPack.Add(type);
                itemsToPack.Add(type);
                itemsToPack.Add(type);
            }

            itemsToPack = itemsToPack.OrderBy(x => Random.value).ToList();

            var index = 0;
            while (index < itemsToPack.Count) {
                var isBig = Random.value > 0.5f && (itemsToPack.Count - index >= 3);
                var size = isBig ? BubbleSize.Big : BubbleSize.Small;
                var count = isBig ? 3 : 1;

                var bubbleView = (size == BubbleSize.Big) ? _bigBubblePool.Get() : _smallBubblePool.Get();
                bubbleView.Setup(size);
                bubbleView.transform.position = new Vector3(
                    Random.Range(-_levelConfig.bubbleSpawnWidth, _levelConfig.bubbleSpawnWidth),
                    _levelConfig.spawnHeightOffset + (index * 0.5f),
                    0
                );

                var bubbleModel = new BubbleModel {
                    Size = size
                };
                _model.Bubbles.Add(bubbleModel);

                for (var i = 0; i < count; i++) {
                    var type = itemsToPack[index++];

                    bubbleModel.Items.Add(new ItemModel {
                        Type = type
                    });

                    var itemView = _itemPool.Get();
                    var data = _gameplayConfig.GetItemData(type);
                    itemView.Setup(type, data.sprite, data.color);
                    itemView.OnClicked = HandleItemClick;
                    bubbleView.AddItem(itemView);
                }
            }

            InitializeContainers();
            InitializeSlots();
        }

        private void InitializeContainers() {
            for (var i = 0; i < _view.Containers.Count; i++) {
                var view = _view.Containers[i];
                if (i < _levelConfig.containerCount) {
                    view.gameObject.SetActive(true);
                    var targetType = _levelConfig.availableTypes[Random.Range(0, _levelConfig.availableTypes.Count)];
                    var model = new ContainerModel(targetType);
                    _model.Containers.Add(model);
                    var data = _gameplayConfig.GetItemData(targetType);
                    view.SetTarget(targetType, data.sprite, data.color);
                }
                else {
                    view.gameObject.SetActive(false);
                }
            }
        }

        private void InitializeSlots() {
            var count = _levelConfig.slotCount;
            if (count <= 0) {
                count = _view.Slots.Count;
                Debug.LogWarning($"[GameplayController] slotCount in LevelConfig is {_levelConfig.slotCount}. Fallback to View slots count: {count}");
            }

            // Ensure we don't exceed the number of physical SlotViews assigned in GameplayView
            count = Math.Min(count, _view.Slots.Count);

            for (var i = 0; i < count; i++) {
                _model.Slots.Add(new SlotModel(i));
            }
        }

        private void HandleItemClick(ItemView itemView) {
            var bubbleView = itemView.GetComponentInParent<BubbleView>();
            if (bubbleView == null) return;

            // 1. Logic matching - find destination
            var containerIndex = -1;
            var targetContainerView = FindTargetContainer(itemView.Type, out containerIndex);

            if (targetContainerView != null) {
                var containerModel = _model.Containers[containerIndex];
                var targetPos = targetContainerView.GetPositionForIndex(containerModel.CurrentCount);

                MoveItemToTarget(itemView, targetPos, () => {
                    containerModel.AddItem();
                    if (containerModel.IsFull) {
                        HandleContainerMatch(containerModel, targetContainerView);
                    }

                    _itemPool.Release(itemView);
                });
            }
            else {
                var slotIndex = -1;
                var targetSlotView = FindFreeSlot(out slotIndex);

                if (targetSlotView != null) {
                    var slotModel = _model.Slots[slotIndex];
                    slotModel.Occupy(itemView.Type);
                    var targetPos = targetSlotView.transform.position;

                    MoveItemToTarget(itemView, targetPos, () => {
                        // In a more complex game, we might check for matches in slots here
                    });
                }
                else {
                    _signalBus.Fire<LevelFailedSignal>();
                    return;
                }
            }

            // 2. Update Model state
            UpdateModelOnItemCollected(itemView, bubbleView);

            // 3. Update View
            bubbleView.RemoveItem(itemView);
            if (bubbleView.IsEmpty) {
                if (bubbleView.Size == BubbleSize.Big) _bigBubblePool.Release(bubbleView);
                else _smallBubblePool.Release(bubbleView);
            }

            _signalBus.Fire(new ItemCollectedSignal((int)itemView.Type, targetContainerView != null));
            CheckWinCondition();
        }

        private void UpdateModelOnItemCollected(ItemView itemView, BubbleView bubbleView) {
            // Find the logical bubble that matches the physical one
            // In a real-world scenario, we'd have a mapping, but for now we look for a bubble containing this type
            var bubbleModel =
                _model.Bubbles.FirstOrDefault(bm => bm.Items.Any(im => im.Type == itemView.Type && !im.IsCollected));
            if (bubbleModel != null) {
                var itemModel = bubbleModel.Items.First(im => im.Type == itemView.Type && !im.IsCollected);
                itemModel.IsCollected = true;

                if (bubbleModel.Items.All(im => im.IsCollected)) {
                    _model.Bubbles.Remove(bubbleModel);
                }
            }
        }

        private ContainerView FindTargetContainer(ItemType type, out int index) {
            for (var i = 0; i < _model.Containers.Count; i++) {
                if (_model.Containers[i].TargetType == type && !_model.Containers[i].IsFull) {
                    index = i;
                    return _view.Containers[i];
                }
            }

            index = -1;
            return null;
        }

        private SlotView FindFreeSlot(out int index) {
            for (var i = 0; i < _model.Slots.Count; i++) {
                if (!_model.Slots[i].IsOccupied) {
                    index = i;
                    return _view.Slots[i];
                }
            }

            index = -1;
            return null;
        }

        private void MoveItemToTarget(ItemView item, Vector3 targetPos, Action onComplete) {
            item.transform.SetParent(_view.PoolContainer);
            item.DisableInteraction();

            item.transform.DOMove(targetPos, TravelDuration).SetEase(Ease.OutQuad).OnComplete(() => {
                onComplete?.Invoke();
            });
        }

        private void HandleContainerMatch(ContainerModel model, ContainerView view) {
            var newType = _levelConfig.availableTypes[Random.Range(0, _levelConfig.availableTypes.Count)];
            model.Reset(newType);
            var data = _gameplayConfig.GetItemData(newType);
            view.SetTarget(newType, data.sprite, data.color);
        }

        private void CheckWinCondition() {
            if (_model.Bubbles.Count == 0 && _model.Slots.All(s => !s.IsOccupied)) {
                _signalBus.Fire<LevelCompletedSignal>();
            }
        }
    }
}