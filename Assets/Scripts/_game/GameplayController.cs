using System;
using UnityEngine;

public class GameplayController : MonoBehaviour {
    [SerializeField] private GameUI gameUI;
    [SerializeField] private Camera cam;
    [SerializeField] private ParticleSystem newLevelFX;
    [SerializeField] private ParticleSystemRenderer newLevelFXRenderer;

    [SerializeField] private Collider2D col;

    public Action OnGameStarted;
    public Action OnGameOver;

    //----------------------------------------------------------------------------
    public void Initialize() {
    }

    //----------------------------------------------------------------------------
    public void StartNewGame() {
    }

    //----------------------------------------------------------------------------
    private void StartGame() {

        OnGameStarted?.Invoke();
    }

    //----------------------------------------------------------------------------
    public void RestartGame() {
    }
}
