using UnityEngine;

[CreateAssetMenu(fileName = "config", menuName = "Scriptables/BoxConfig", order = 5)]
public class GameConfig : ScriptableObject {
    public Color removerPriceColor;
    public Color removerPriceNoCoinsColor;
    public Color marketValueCountColor;
    [Space(15)] public Material boxMaterial;

    public Material grayscaleUIMaterial;
    public int minLevel = 1;
    public int maxLevel = 10;
    public int timeForNextFlower = 3;
    public int minItemsFlowerForWindRemove;
    public int minItemsForBeeRemove;
    public int gemsForRewarded;


}