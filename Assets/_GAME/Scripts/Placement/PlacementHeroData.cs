using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Units/Unit Data")]
public class PlacementHeroData : ScriptableObject
{
    public string unitName;
    public Sprite cardIcon;
    public UnitType unitType;
    public GameObject prefab;
    public int size;
    public int cost;

    [Header("Purchase Settings")]
    public bool requiresPurchase = false; // Bu karakter satýn alýnmasý mý gerekiyor?
    public int purchasePrice = 500; // Satýn alma fiyatý

    // Karakterin satýn alýnýp alýnmadýðýný kontrol et
    public bool IsPurchased()
    {
        // Eðer satýn alma gerektirmiyorsa (baþlangýç karakteri), her zaman true döner
        if (!requiresPurchase)
        {
            Debug.Log($"{unitName} - Satýn alma gerektirmiyor, otomatik açýk.");
            return true;
        }

        // MenuHeroCardSO'daki ayný isimli karakterin satýn alma durumunu kontrol et
        bool purchased = PlayerPrefs.GetInt($"{unitName}_Purchased", 0) == 1;
        Debug.Log($"{unitName} - Satýn alma durumu: {purchased} (Key: {unitName}_Purchased)");
        return purchased;
    }

    // Karakteri satýn al
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