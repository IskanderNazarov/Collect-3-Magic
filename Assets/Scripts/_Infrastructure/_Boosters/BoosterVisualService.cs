using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class BoosterVisualService : MonoBehaviour {
    [Header("Hint FX")] [SerializeField] private ParticleSystem _hintTrailPrefab;
    [SerializeField] private ParticleSystem _hintExplosionPrefab;

    [FormerlySerializedAs("_hammerPrefab")] [Header("Hammer FX")] [SerializeField] private Transform _hammerAnim;
    [SerializeField] private Transform _hammerEndPos;
    [SerializeField] private ParticleSystem _hammerImpactPrefab;

    private Camera _cam;

    [Inject]
    private void Construct() {
        _cam = Camera.main;
    }

    private void Start() {
        _hammerAnim.gameObject.SetActive(false);
        _hammerImpactPrefab.gameObject.SetActive(false);
    }

    // 1. Анимация полета "Искры" от кнопки к голове змейки (Hint / Ruler)
    public void PlayHintProjectile(Vector3 buttonScreenPos, Vector3 targetWorldPos, Action onArrived) {
        print("PlayHintProjectile");
        var startWorldPos = buttonScreenPos;//;_cam.ScreenToWorldPoint(new Vector3(buttonScreenPos.x, buttonScreenPos.y, 10f));

        var trail = Instantiate(_hintTrailPrefab, startWorldPos, Quaternion.identity, transform);
        trail.gameObject.SetActive(true);

        // Летим по дуге для красоты
        trail.transform.DOJump(targetWorldPos, jumpPower: 2f, numJumps: 1, duration: 1f)
            .SetEase(Ease.InSine)
            .OnComplete(() => {
                // Взрыв по прибытии
                if (_hintExplosionPrefab != null) {
                    var explosion = Instantiate(_hintExplosionPrefab, targetWorldPos, Quaternion.identity);
                    explosion.gameObject.SetActive(true);
                    Destroy(explosion.gameObject, 1.5f);
                }

                Destroy(trail.gameObject);
                onArrived?.Invoke(); // Говорим контроллеру: "Включай линию!"
            });
    }

    // 2. Анимация удара Молотком
    public void PlayHammerSequence(Vector3 buttonScreenPos, Vector3 targetWorldPos, Action onImpact) {
        var startPos =      _hammerAnim.position;
        var startRotation = _hammerAnim.eulerAngles;
        var startScale =    _hammerAnim.localScale;
        
        var endPos =      _hammerEndPos.position;
        var endRotation = _hammerEndPos.eulerAngles;
        var endScale =    _hammerEndPos.localScale;
        
        _hammerAnim.gameObject.SetActive(true);


        var seq = DOTween.Sequence();
        // Молоток появляется и летит к центру доски
        var dur = 0.75f;
        var overshoot = 3.5f;
        seq.Append(_hammerAnim.DOMove(endPos, dur)/*.From(startPos - Vector3.up * 2)*/.SetEase(Ease.InBack, overshoot));
        seq.Join(_hammerAnim.DOScale(endScale, dur).SetEase(Ease.InBack, overshoot));
        seq.Join(_hammerAnim.DORotate(endRotation, dur).SetEase(Ease.InBack, overshoot));

        seq.OnComplete(() => {
            if (_hammerImpactPrefab != null) {
                var impact = Instantiate(_hammerImpactPrefab, transform);
                impact.gameObject.SetActive(true);
                Destroy(impact.gameObject, 1f);
            }

            // Заставляем молоток исчезнуть
            _hammerAnim.DOScale(endScale, 0.70f).OnComplete(() => {
                _hammerAnim.gameObject.SetActive(false);
                _hammerAnim.eulerAngles = startRotation;
                _hammerAnim.localScale = startScale;
                _hammerAnim.position = startPos;
            });

            onImpact?.Invoke(); // Говорим контроллеру: "Тряси камеру и запускай змеек!"
        });
    }
}