using System.Collections.Generic;
using __Gameplay;
using UnityEngine;

[CreateAssetMenu(fileName = "BoostersConfig", menuName = "Data/Game/Boosters Config")]
public class BoostersConfig : ScriptableObject {
    public List<BoosterInfo> Boosters = new List<BoosterInfo>();

    public BoosterInfo GetBooster(BoosterId id) {
        return Boosters.Find(b => b.BoosterId == id);
    }
}