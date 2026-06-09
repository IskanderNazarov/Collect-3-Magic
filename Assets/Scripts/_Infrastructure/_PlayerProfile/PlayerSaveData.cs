using System;
using System.Collections.Generic;

[Serializable]
public class PlayerSaveData {
    public int Version = 1;
    public int CurrentLevelIndex = 0;
    public int CurrentScore = 0;

    // new fields for meta-game
    public int CurrentStars = 0;
    public int CurrentCoins = 110;
    public int BigStars = 0;
    public int UnlockedPlanetsCount = 0;
    public int UnlockedCrittersCount = 1; // Initially the first critter is unlocked

    // serialized state for MetaProgressionService
    public string MetaProgressionStateJson = "";

    public bool IsMusicOn = true;
    public bool IsSoundOn = true;

    public int CurrentLives = 5;
    public long LastLifeRegenTimeTicks = 0;

    public List<int> BoosterCounts = new List<int> { 2, 2, 2 };
    public List<string> CompletedTutorials = new List<string>();
}