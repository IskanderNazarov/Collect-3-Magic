using System;
using __Gameplay;
using _Services._Localization;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class BoosterInfo {
    public BoosterId BoosterId;
    [FormerlySerializedAs("Name")] [TranslationKey] public string NameKey;
    [FormerlySerializedAs("Description")] [TranslationKey] public string DescriptionKey;
    public Sprite Icon;
    public int PriceInCoins; //price of the booster in coins
    public int RewardForCoins; //how many boosters for the price
    public int RewardCount = 1; //how many boosters for rewarded ad.
    public int RequiredLevel; // level after which the booster is available
}