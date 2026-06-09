using System;
using System.Collections.Generic;
using System.Linq;
using _game;
using _game._GameViews;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class BoosterVisualService : MonoBehaviour {
    [Header("Boosters Visuals")]
    [SerializeField] private Transform _catPawLeft;
    [SerializeField] private Transform _catPawRight;
    [SerializeField] private Transform _magicWand;


    private Camera _cam;
    [Inject] private GameplayController _gameplayController;

    [Inject]
    private void Construct() {
        _cam = Camera.main;
    }

    private void Start() {
        if (_catPawLeft != null) _catPawLeft.gameObject.SetActive(false);
        if (_catPawRight != null) _catPawRight.gameObject.SetActive(false);
        if (_magicWand != null) _magicWand.gameObject.SetActive(false);
    }

    public void ExecuteShuffle(Action onComplete) {
        _gameplayController.IsInputBlocked = true;

        Sequence sequence = DOTween.Sequence();

        // 1. Show paws animation
        if (_catPawLeft != null && _catPawRight != null) {
            _catPawLeft.gameObject.SetActive(true);
            _catPawRight.gameObject.SetActive(true);
            _catPawLeft.position = new Vector3(-10, 0, -5);
            _catPawRight.position = new Vector3(10, 0, -5);

            sequence.Append(_catPawLeft.DOMove(Vector3.zero, 0.5f).SetEase(Ease.OutBack));
            sequence.Join(_catPawRight.DOMove(Vector3.zero, 0.5f).SetEase(Ease.OutBack));

            // Chaotic movement
            for (int i = 0; i < 4; i++) {
                sequence.Append(_catPawLeft.DOMove(new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), -5), 0.2f));
                sequence.Join(_catPawRight.DOMove(new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), -5), 0.2f));
            }
        }

        // 2. Perform animated shuffle
        PerformShuffle(sequence);

        if (_catPawLeft != null && _catPawRight != null) {
            sequence.Append(_catPawLeft.DOMove(new Vector3(-10, 0, -5), 0.5f).SetEase(Ease.InBack));
            sequence.Join(_catPawRight.DOMove(new Vector3(10, 0, -5), 0.5f).SetEase(Ease.InBack));
            sequence.AppendCallback(() => {
                _catPawLeft.gameObject.SetActive(false);
                _catPawRight.gameObject.SetActive(false);
            });
        }

        sequence.OnComplete(() => {
            _gameplayController.IsInputBlocked = false;
            onComplete?.Invoke();
        });
    }

    private void PerformShuffle(Sequence sequence) {
        var bubbles = _gameplayController.GetAllActiveBubbles();
        var allItems = new List<ItemView>();
        
        foreach (var bubble in bubbles) {
            var itemsInBubble = bubble.Items.ToList();
            allItems.AddRange(itemsInBubble);
            foreach (var item in itemsInBubble) {
                bubble.RemoveItem(item);
            }
        }

        // Shuffle items
        var shuffledItems = allItems.OrderBy(x => Random.value).ToList();

        // Animate to new positions
        var itemIndex = 0;
        foreach (var bubble in bubbles) {
            for (int i = 0; i < bubble.Capacity && itemIndex < shuffledItems.Count; i++) {
                var item = shuffledItems[itemIndex++];
                var targetSlot = bubble.Slots[i];
                
                // Ensure they are on a high layer during shuffle
                item.SetSortingLayer("Top");
                
                sequence.Join(item.transform.DOMove(targetSlot.transform.position, 0.6f).SetEase(Ease.InOutQuad));
                sequence.Join(item.transform.DORotate(new Vector3(0, 0, Random.Range(-20, 20)), 0.6f));
            }
        }

        // Finalize parenting and logic state
        sequence.AppendCallback(() => {
            itemIndex = 0;
            foreach (var bubble in bubbles) {
                int count = Mathf.Min(bubble.Capacity, shuffledItems.Count - itemIndex);
                for (int i = 0; i < count; i++) {
                    var item = shuffledItems[itemIndex++];
                    bubble.AddItem(item);
                    item.ResetSortingLayer();
                }
            }
        });
    }

    public void ExecuteRemover(Action onComplete) {
        var bubbles = _gameplayController.GetAllActiveBubbles();
        var allItems = bubbles.SelectMany(b => b.Items).ToList();
        
        if (allItems.Count <= 9) {
            onComplete?.Invoke();
            return;
        }

        var targetBubble = bubbles
            .OrderBy(b => b.transform.position.y)
            .ThenByDescending(b => b.Capacity)
            .ThenBy(b => b.Items.Count)
            .FirstOrDefault();

        if (targetBubble == null) {
            onComplete?.Invoke();
            return;
        }

        var firstItem = targetBubble.Items.FirstOrDefault();
        if (firstItem == null) {
            onComplete?.Invoke();
            return;
        }

        var targetType = firstItem.Type;
        var matchingItems = allItems.Where(i => i.Type == targetType).Take(3).ToList();
        
        if (matchingItems.Count < 3) {
            onComplete?.Invoke();
            return;
        }

        if (_gameplayController.GetTotalItemCount(targetType) - 3 < _gameplayController.GetTotalBookedCount(targetType)) {
            onComplete?.Invoke();
            return;
        }

        _gameplayController.IsInputBlocked = true;
        
        Sequence sequence = DOTween.Sequence();
        Vector3 offScreenBottom = new Vector3(0, -20, -5);
        
        if (_magicWand != null) {
            _magicWand.gameObject.SetActive(true);
            _magicWand.position = new Vector3(8, 10, -5);
            
            // 1. Semi-circle arc movement
            Vector3[] path = new Vector3[] {
                new Vector3(8, 10, -5),
                new Vector3(0, 14, -5),
                new Vector3(-8, 10, -5)
            };
            
            sequence.Append(_magicWand.DOPath(path, 1.2f, PathType.CatmullRom).SetEase(Ease.InOutSine));
            
            // 2. Start flying down
            sequence.Append(_magicWand.DOMove(offScreenBottom, 0.8f).SetEase(Ease.InBack));
            
            // 3. Items follow the wand down
            foreach (var item in matchingItems) {
                // Use Insert to start exactly when wand starts moving down (at 1.2s mark)
                sequence.Insert(1.2f, item.transform.DOMove(offScreenBottom, 1f).SetEase(Ease.InQuad));
                sequence.Insert(1.2f, item.transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360));
                
                // Ensure they are visible above bubbles
                sequence.InsertCallback(1.2f, () => item.SetSortingLayer("Top"));
            }
        }

        sequence.AppendCallback(() => {
            foreach (var item in matchingItems) {
                _gameplayController.ManualRemoveItem(item);
            }
        });

        if (_magicWand != null) {
            sequence.AppendCallback(() => _magicWand.gameObject.SetActive(false));
        }

        sequence.OnComplete(() => {
            _gameplayController.IsInputBlocked = false;
            onComplete?.Invoke();
        });
    }
}
