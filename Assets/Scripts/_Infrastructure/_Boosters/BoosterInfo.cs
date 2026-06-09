using System;
using __Gameplay;
using _Services._Localization;
using UnityEngine;
[Serializable]
public class BoosterInfo {
    public BoosterId BoosterId;
    [TranslationKey] public string NameKey;
    [TranslationKey] public string DescriptionKey;
    public Sprite Icon;
    public int PriceInCoins; //price of the booster in coins
    public int RewardForCoins; //how many boosters for the price
    public int RewardCount = 1; //how many boosters for rewarded ad.
    public int RequiredLevel; // level after which the booster is available
}