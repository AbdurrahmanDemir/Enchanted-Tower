using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Units/Unit Data")]
public class PlacementHeroData : ScriptableObject
{
    public string unitName;
    public Sprite cardIcon;
    public UnitType unitType;
    public GameObject prefab;

    [Header("Elixir Cost")]
    public int elixirCost; 

    public int cost; 
    [Header("Purchase Settings")]
    public bool requiresPurchase = false; 
    public int purchasePrice = 500; 

    public bool IsPurchased()
    {
        if (!requiresPurchase)
        {
            Debug.Log($"{unitName} - Satýn alma gerektirmiyor, otomatik açýk.");
            return true;
        }

        bool purchased = PlayerPrefs.GetInt($"{unitName}_Purchased", 0) == 1;
        Debug.Log($"{unitName} - Satýn alma durumu: {purchased} (Key: {unitName}_Purchased)");
        return purchased;
    }

    public bool PurchaseHero()
    {
        if (!requiresPurchase || IsPurchased())
            return false;

        if (DataManager.instance.TryPurchaseGold(purchasePrice))
        {
            PlayerPrefs.SetInt($"{unitName}_Purchased", 1);
            PlayerPrefs.Save();
            return true;
        }

        return false;
    }
}

public enum UnitType
{
    Hero,
    Building,
    Spell
}