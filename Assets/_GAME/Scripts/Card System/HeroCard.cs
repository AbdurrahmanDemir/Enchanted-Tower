using System;
using UnityEngine;

[System.Serializable]
public class HeroCard
{
    public HeroCardSO heroCardSO;
    public int level;
    public int collectedCards;

    public event Action<HeroCard> OnLevelUp;

    public HeroCard(HeroCardSO heroCardSO)
    {
        this.heroCardSO = heroCardSO;
        this.level = 1;
        this.collectedCards = 0;
    }

    public void AddCards(int amount)
    {
        collectedCards += amount;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        if (level >= heroCardSO.maxLevel) return;

        int requiredCards = heroCardSO.upgradeRequirements[level - 1];
        if (collectedCards >= requiredCards)
        {
            collectedCards -= requiredCards;
            level++;
            OnLevelUp?.Invoke(this);
        }
    }

    // Kaydetme
    public void Save()
    {
        string keyPrefix = heroCardSO.cardName;
        PlayerPrefs.SetInt(keyPrefix + "_Level", level);
        PlayerPrefs.SetInt(keyPrefix + "_Cards", collectedCards);
    }

    // Yükleme
    public void Load()
    {
        string keyPrefix = heroCardSO.cardName;
        level = PlayerPrefs.GetInt(keyPrefix + "_Level", 1);
        collectedCards = PlayerPrefs.GetInt(keyPrefix + "_Cards", 0);
    }
}
