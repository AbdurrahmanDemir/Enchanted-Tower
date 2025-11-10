using UnityEngine;

[CreateAssetMenu(fileName = "NewHeroCard", menuName = "Card System/HeroCard")]
public class HeroCardSO : ScriptableObject
{
    public string cardName;       
    public Sprite heroIcon;        
    public HeroCardType cardType;      
    public string cardDescription; 

    public int[] upgradeRequirements; 

    public int baseDamage;
    public int baseHealth;
    public float range;
    public float cooldown;
    public float moveSpeed;
    public string specialStatName;
    public float specialStat;
    public int purchasePrice;
    public bool requiresPurchase = false;
    public int maxLevel => upgradeRequirements.Length;

    public int GetCurrentDamage(int level)
    {
        return baseDamage + (level - 1) * 5;
    }

    public int GetCurrentHealth(int level)
    {
        return baseHealth + (level - 1) * 10; 
    }

    public int GetUpgradeCost(int level)
    {
        if (level - 1 < upgradeRequirements.Length)
            return upgradeRequirements[level - 1];
        return 0;
    }
}

public enum HeroCardType
{
    Common,
    Rare,
    Epic,
    Legendary
}
