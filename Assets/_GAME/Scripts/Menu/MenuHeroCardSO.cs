using UnityEngine;

[CreateAssetMenu(fileName = "MenuHero", menuName = "HeroCard")]

public class MenuHeroCardSO : ScriptableObject
{
    [Header("Hero")]
    public string cardName;
    public Sprite heroIcon;
    public CardType cardType;
    public float baseUpgradeCost = 100;
    public float range;
    public int damage;
    public int health;
    public float cooldown;
    public float moveSpeed;
    public float undergroundRange;
    public int purchasePrice;
    public bool requiresPurchase = false;

    [Header("Other Stat")]
    public string specialStatName;
    public float specialStat;
    [TextArea] public string cardDescription;

    public bool IsPurchased()
    {
        return PlayerPrefs.GetInt($"{cardName}_Purchased", requiresPurchase ? 0 : 1) == 1;
    }

    public bool PurchaseHero()
    {
        if (!requiresPurchase || IsPurchased())
            return false;

        if (DataManager.instance.TryPurchaseGold((int)purchasePrice))
        {
            PlayerPrefs.SetInt($"{cardName}_Purchased", 1);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }
    public int GetCurrentHealth()
    {
        return PlayerPrefs.GetInt($"{cardName}_Health", health);
    }
    public int GetCurrentDamage()
    {
        return PlayerPrefs.GetInt($"{cardName}_Damage", damage);
    }
    public int GetUpgradeCost()
    {
        return PlayerPrefs.GetInt($"{cardName}_UpgradeCost", (int)baseUpgradeCost);
    }

    public void UpgradeHero()
    {
        int currentDamage = GetCurrentDamage();
        int upgradeCost = GetUpgradeCost();   
        int currentHealth = GetCurrentHealth();
        int upgradeLevel = PlayerPrefs.GetInt($"{cardName}_UpgradeLevel", 1);

        if (DataManager.instance.TryPurchaseHeroUpgradeToken(upgradeCost))
        {
            int newDamage = currentDamage + 5;
            int newHealth = currentHealth + 50;

            int newUpgradeCost = Mathf.RoundToInt(upgradeCost * 1.5f);

            upgradeLevel++;

            PlayerPrefs.SetInt($"{cardName}_Damage", newDamage);
            PlayerPrefs.SetInt($"{cardName}_Health", newHealth);
            PlayerPrefs.SetInt($"{cardName}_UpgradeCost", newUpgradeCost);
            PlayerPrefs.SetInt($"{cardName}_UpgradeLevel", upgradeLevel);
            PlayerPrefs.Save();
        }
    }

}
public enum CardType
{
    Cammon=0,
    Rare=1,
    Epic=2,
    Legendary=3
}


