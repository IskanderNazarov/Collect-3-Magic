// File: Assets/Game/Scripts/PlayerProgressService.cs

using System;
using System.Collections;
using Core._Services;
using Core._Services._Saving;
public class PlayerProgressService : SaveManager<PlayerSaveData> {
    private const string SaveKey = "player_data";

    public int CurrentLevelIndex => Data.CurrentLevelIndex;
    public int Score => Data.CurrentScore;
    public bool IsAnyPlanetUnlocked => Data.UnlockedPlanetsCount > 0;
    public int CurrentCoins => Data.CurrentCoins;

    public event Action<int> OnLevelChanged;

    public PlayerProgressService(IDataSaver dataSaver) : base(dataSaver, SaveKey) {
    }

    public override IEnumerator Initialize() {
        yield return base.Initialize();
        CheckMigrations();
    }

    public void AddCoins(int count) {
        Data.CurrentCoins += count;
        MarkDirty();
        //_signalBus.Fire(new CoinsChangedSignal(Data.CurrentCoins));
        SaveImmediate();
    }

    public bool TryConsumeCoins(int count) {
        if (Data.CurrentCoins < count) return false;
        Data.CurrentCoins -= count;
        MarkDirty();
        //_signalBus.Fire(new CoinsChangedSignal(Data.CurrentCoins));
        return true;
    }

    public int GetBoosterCount(int boosterId) {
        if (boosterId < 0 || boosterId >= Data.BoosterCounts.Count) return 0;
        return Data.BoosterCounts[boosterId];
    }

    public void SetBoosterCount(int boosterId, int count) {
        while (Data.BoosterCounts.Count <= boosterId) {
            Data.BoosterCounts.Add(2); //default count
        }

        Data.BoosterCounts[boosterId] = count;
        MarkDirty();
    }

    private void CheckMigrations() {
        if (Data.Version < 1) {
        }
    }

    public void ResetProgress() {
        Data = new PlayerSaveData();
        MarkDirty();
        SaveImmediate();
    }
}
