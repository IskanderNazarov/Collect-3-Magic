using System;
using System.Collections.Generic;
using System.Linq;
using _Data;
using _game._GameModel;
using _game._GameViews;
using _Infrastructure;
using _Signals;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace _game {
    public class GameplayController {
        private readonly SignalBus _signalBus;
        private readonly GameplayView _view;
        private readonly LevelConfig _levelConfig;
        private readonly GameplayConfig _gameplayConfig;

        private SimpleObjectPool<ItemView> _itemPool;
        private Dictionary<BubbleSize, SimpleObjectPool<BubbleView>> _bubblePools;

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

        public bool IsInputBlocked { get; set; }

        public void Initialize() {
            InitializePools();
            _view.SlotsDock.Initialize(_view.Slots);
            IsInputBlocked = false;
            GenerateLevel();
        }

        private void InitializePools() {
            _itemPool = new SimpleObjectPool<ItemView>(_view.ItemPrefab, 50, _view.PoolContainer);
            _bubblePools = new Dictionary<BubbleSize, SimpleObjectPool<BubbleView>>();
            foreach (var config in _view.BubbleConfigs) {
                _bubblePools[config.size] = new SimpleObjectPool<BubbleView>(config.prefab, config.initialPoolSize, _view.PoolContainer);
            }
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

            // Improved spawn logic: use lanes and track height per lane to avoid overlaps
            var spawnLaneCount = 5;
            var laneWidth = (_levelConfig.bubbleSpawnWidth * 2f) / spawnLaneCount;
            var laneHeights = new float[spawnLaneCount];
            for (var i = 0; i < spawnLaneCount; i++) laneHeights[i] = _levelConfig.spawnHeightOffset;

            var index = 0;
            while (index < itemsToPack.Count) {
                var weightConfig = PickRandomBubbleWeight(itemsToPack.Count - index);
                if (weightConfig == null) break;

                var size = weightConfig.size;
                var count = weightConfig.itemsCount;

                if (!_bubblePools.TryGetValue(size, out var pool)) {
                    Debug.LogError($"[GameplayController] No pool for bubble size {size}");
                    index++;
                    continue;
                }

                var bubbleModel = new BubbleModel {
                    Size = size
                };
                _model.Bubbles.Add(bubbleModel);

                var bubbleView = pool.Get();
                bubbleView.Setup(bubbleModel);

                // Position in lanes to avoid overlap

                for (var i = 0; i < count; i++) {
                    var type = itemsToPack[index++];
                    bubbleModel.Items.Add(new ItemModel {
                        Type = type
                    });

                    var itemView = _itemPool.Get();
                    var data = _gameplayConfig.GetItemData(type);
                    itemView.Setup(data);
                    itemView.OnClicked = HandleItemClick;
                    bubbleView.AddItem(itemView);
                }
            }

            InitializeContainers();
        }

        private BubbleSpawnWeight PickRandomBubbleWeight(int remainingItems) {
            var possibleConfigs = _levelConfig.bubbleWeights
                .Where(w => w.itemsCount <= remainingItems)
                .ToList();

            if (possibleConfigs.Count == 0) return null;

            float totalWeight = possibleConfigs.Sum(w => w.weight);
            float randomValue = Random.Range(0, totalWeight);
            float currentSum = 0;

            foreach (var config in possibleConfigs) {
                currentSum += config.weight;
                if (randomValue <= currentSum) return config;
            }

            return possibleConfigs.Last();
        }

        private void InitializeContainers() {
            for (var i = 0; i < _view.Containers.Count; i++) {
                var view = _view.Containers[i];
                if (i < _levelConfig.containerCount) {
                    view.gameObject.SetActive(true);

                    // Initially 2 are open, others locked (if we have more than 2)
                    var isLocked = i >= 2;
                    var model = new ContainerModel(ItemType.None, isLocked);
                    _model.Containers.Add(model);

                    if (!isLocked) {
                        UpdateContainerTarget(view, model);
                    }
                    else {
                        view.ClearTarget();
                    }
                }
                else {
                    view.gameObject.SetActive(false);
                }
            }
        }
        public void UnlockContainer(int index) {
            if (index >= 0 && index < _model.Containers.Count) {
                var model = _model.Containers[index];
                if (model.IsLocked) {
                    model.Unlock();
                    UpdateContainerTarget(_view.Containers[index], model);
                }
            }
        }

        private void UpdateContainerTarget(ContainerView view, ContainerModel model) {
            var itemCounts = GetGlobalItemCounts();
            
            // Logic: Target an item currently on the board where (Total - ReservedByOthers) >= 3
            var candidates = new List<ItemType>();
            foreach (var type in _levelConfig.availableTypes) {
                var totalOnBoard = itemCounts.ContainsKey(type) ? itemCounts[type] : 0;
                
                var bookedByOthers = 0;
                foreach (var otherContainer in _model.Containers) {
                    if (otherContainer != model && otherContainer.IsOccupied && otherContainer.TargetType == type) {
                        bookedByOthers += (3 - otherContainer.ReservedCount);
                    }
                }

                if (totalOnBoard - bookedByOthers >= 3) {
                    candidates.Add(type);
                }
            }

            if (candidates.Count > 0) {
                var targetType = candidates[Random.Range(0, candidates.Count)];
                Debug.Log($"[GameplayController] Container assigned new target: {targetType}. Candidates found: {candidates.Count}. Board count for this type: {itemCounts[targetType]}");
                
                // Pick a critter that can request this item
                var critterData = _gameplayConfig.CritterDataList
                    .Where(c => (c.AllowedItems & targetType) != 0 || c.AllowedItems == ItemType.All)
                    .OrderBy(x => Random.value)
                    .FirstOrDefault();

                if (critterData == null) {
                    critterData = _gameplayConfig.CritterDataList[Random.Range(0, _gameplayConfig.CritterDataList.Count)];
                }

                var itemData = _gameplayConfig.GetItemData(targetType);
                model.Reset(targetType);
                
                // Wait for arrival animation before checking dock
                view.SetTarget(critterData, itemData).OnComplete(() => {
                    CheckSlotsDockForMatches();
                });
            }
            else {
                Debug.Log("[GameplayController] No candidates for container. Keeping it empty.");
                model.Clear();
                view.ClearTarget();
            }
        }

        private Dictionary<ItemType, int> GetGlobalItemCounts() {
            var counts = new Dictionary<ItemType, int>();

            // From bubbles
            foreach (var bubble in _model.Bubbles) {
                foreach (var item in bubble.Items) {
                    if (!item.IsCollected) {
                        if (!counts.ContainsKey(item.Type)) counts[item.Type] = 0;
                        counts[item.Type]++;
                    }
                }
            }

            // From dock
            foreach (var item in _view.SlotsDock.GetAllItems()) {
                if (!counts.ContainsKey(item.Type)) counts[item.Type] = 0;
                counts[item.Type]++;
            }

            // DO NOT subtract ReservedCount here anymore, we handle it in UpdateContainerTarget
            return counts;
        }

        private List<ItemType> GetAvailableItemsOnBoard() {
            var items = new HashSet<ItemType>();

            // From bubbles
            foreach (var bubble in _model.Bubbles) {
                foreach (var item in bubble.Items) {
                    if (!item.IsCollected) items.Add(item.Type);
                }
            }

            // From dock
            foreach (var item in _view.SlotsDock.GetAllItems()) {
                items.Add(item.Type);
            }

            return items.ToList();
        }

        private void HandleItemClick(ItemView itemView) {
            if (IsInputBlocked) return;

            var bubbleView = itemView.GetComponentInParent<BubbleView>();
            if (bubbleView == null) return;

            // 1. Logic matching - find destination
            var containerIndex = -1;
            var targetContainerView = FindTargetContainer(itemView.Type, out containerIndex);

            if (targetContainerView != null) {
                ProcessItemToContainer(itemView, targetContainerView, _model.Containers[containerIndex]);
            }
            else {
                // Change layer for flight to dock
                itemView.SetSortingLayer(_gameplayConfig.TopSortingLayer);

                var added = _view.SlotsDock.TryAddItem(itemView, TravelDuration, () => {
                    // Item reached dock - change to dock layer
                    itemView.SetSortingLayer(_gameplayConfig.DockSortingLayer);
                });

                if (!added) {
                    _signalBus.Fire<LevelFailedSignal>();
                    return;
                }
            }

            // 2. Update Model state
            UpdateModelOnItemCollected(itemView, bubbleView);

            // 3. Update View
            bubbleView.RemoveItem(itemView);
            if (bubbleView.IsEmpty) {
                if (_bubblePools.TryGetValue(bubbleView.Size, out var pool)) {
                    pool.Release(bubbleView);
                }
                else {
                    Debug.LogError($"[GameplayController] No pool to release bubble size {bubbleView.Size}");
                }
            }

            _signalBus.Fire(new ItemCollectedSignal((int)itemView.Type, targetContainerView != null));
            CheckWinCondition();
        }

        private void ProcessItemToContainer(ItemView itemView, ContainerView containerView, ContainerModel containerModel) {
            var index = containerModel.ReservedCount;
            var targetPos = containerView.GetPositionForIndex(index);
            var targetScale = containerView.GetScaleForIndex(index);

            containerModel.ReserveItem();

            // If it's the 3rd item, scale down the cloud while flying
            if (index == 2) {
                containerView.ScaleCloud(false, TravelDuration);
            }

            // Change layer for flight
            itemView.SetSortingLayer(_gameplayConfig.TopSortingLayer);

            MoveItemToTarget(itemView, targetPos, targetScale, () => {
                containerModel.ItemLanded();

                var data = _gameplayConfig.GetItemData(itemView.Type);
                containerView.ShowLandedItem(index, data);

                if (containerModel.AllLanded) {
                    // Small delay before scaling critter down
                    DOVirtual.DelayedCall(0.5f, () => {
                        containerView.HideItemVisuals();
                        containerView.ScaleCritter(false, 0.3f, () => {
                            HandleContainerMatch(containerModel, containerView);
                        });
                    });
                }

                // Return to pool and reset layer
                itemView.ResetSortingLayer();
                _itemPool.Release(itemView);
            });
        }

        private void CheckSlotsDockForMatches() {
            for (var i = 0; i < _model.Containers.Count; i++) {
                var containerModel = _model.Containers[i];
                var containerView = _view.Containers[i];

                if (!containerView.gameObject.activeSelf || containerModel.IsLocked || !containerModel.IsOccupied)
                    continue;

                var needed = 3 - containerModel.ReservedCount;
                if (needed <= 0) continue;

                var itemsFromDock = _view.SlotsDock.ExtractItems(containerModel.TargetType, needed);

                foreach (var dockItem in itemsFromDock) {
                    ProcessItemToContainer(dockItem, containerView, containerModel);
                }
            }
        }

        private void UpdateModelOnItemCollected(ItemView itemView, BubbleView bubbleView) {
            var bubbleModel = bubbleView.Model;
            if (bubbleModel != null) {
                var itemModel = bubbleModel.Items.FirstOrDefault(im => im.Type == itemView.Type && !im.IsCollected);
                if (itemModel != null) {
                    itemModel.IsCollected = true;
                }

                if (bubbleModel.Items.All(im => im.IsCollected)) {
                    _model.Bubbles.Remove(bubbleModel);
                }
            }
        }

        private ContainerView FindTargetContainer(ItemType type, out int index) {
            for (var i = 0; i < _model.Containers.Count; i++) {
                var model = _model.Containers[i];
                if (model.IsOccupied && !model.IsLocked && model.TargetType == type && !model.IsFull) {
                    index = i;
                    return _view.Containers[index];
                }
            }

            index = -1;
            return null;
        }

        private void MoveItemToTarget(ItemView item, Vector3 targetPos, Vector3 targetScale, Action onComplete) {
            item.transform.SetParent(_view.PoolContainer);
            item.DisableInteraction();

            // Reset physics state to prevent jitter or unwanted rotation during tween
            var rb = item.GetComponent<Rigidbody2D>();
            if (rb != null) {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.simulated = false;
            }

            // ANIMATION CORE: Fly, Scale and Rotate to 0
            item.transform.DOMove(targetPos, TravelDuration).SetEase(Ease.OutQuad);
            item.transform.DOScale(targetScale, TravelDuration).SetEase(Ease.OutQuad);
            item.transform.DORotate(Vector3.zero, TravelDuration).SetEase(Ease.OutQuad).OnComplete(() => {
                onComplete?.Invoke();
            });
        }

        private void HandleContainerMatch(ContainerModel model, ContainerView view) {
            _model.CompletedMatchesCount++;
            
            // Mark the model as not occupied anymore so RefreshAllCritters knows it needs a new target
            model.Clear();
            // We don't call view.ClearTarget here because the critter is already at scale 0 
            // and we want to immediately start the appearing animation if a new target is found.
            
            RefreshAllCritters();
            CheckWinCondition();
        }

        private void RefreshAllCritters() {
            for (var i = 0; i < _model.Containers.Count; i++) {
                var containerModel = _model.Containers[i];
                var containerView = _view.Containers[i];
                
                if (containerModel.IsLocked) continue;

                if (!containerModel.IsOccupied) {
                    UpdateContainerTarget(containerView, containerModel);
                }
                else {
                    var type = containerModel.TargetType;
                    var totalOnBoard = GetTotalItemCount(type);
                    var bookedByOthers = 0;
                    foreach (var other in _model.Containers) {
                        if (other != containerModel && other.IsOccupied && other.TargetType == type) {
                            bookedByOthers += (3 - other.ReservedCount);
                        }
                    }

                    if (totalOnBoard - bookedByOthers < (3 - containerModel.ReservedCount)) {
                        containerModel.Clear();
                        containerView.ClearTarget();
                        UpdateContainerTarget(containerView, containerModel);
                    }
                }
            }
        }

        public List<BubbleView> GetAllActiveBubbles() {
            return GameObject.FindObjectsByType<BubbleView>(FindObjectsSortMode.None)
                .Where(b => b.gameObject.activeSelf).ToList();
        }

        public void ManualRemoveItem(ItemView itemView) {
            var bubbleView = itemView.GetComponentInParent<BubbleView>();
            if (bubbleView != null) {
                bubbleView.RemoveItem(itemView);
                if (bubbleView.IsEmpty) {
                    if (_bubblePools.TryGetValue(bubbleView.Size, out var pool)) {
                        pool.Release(bubbleView);
                    }
                }
            }

            UpdateModelOnItemCollected(itemView, bubbleView);
            _itemPool.Release(itemView);
            
            RefreshAllCritters();
            CheckWinCondition();
        }

        public int GetTotalItemCount(ItemType type) {
            var counts = GetGlobalItemCounts();
            return counts.ContainsKey(type) ? counts[type] : 0;
        }

        public int GetTotalBookedCount(ItemType type) {
            var booked = 0;
            foreach (var container in _model.Containers) {
                if (container.IsOccupied && container.TargetType == type) {
                    booked += (3 - container.ReservedCount);
                }
            }
            return booked;
        }

        private void CheckWinCondition() {
            var boardEmpty = _model.Bubbles.Count == 0 && _view.SlotsDock.OccupiedCount == 0;

            if (boardEmpty) {
                Debug.Log($"[GameplayController] Win condition triggered. Bubbles={_model.Bubbles.Count}, Dock={_view.SlotsDock.OccupiedCount}");
                _signalBus.Fire<LevelCompletedSignal>();
            }
        }
}
}
