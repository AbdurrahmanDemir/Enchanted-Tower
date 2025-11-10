using System.Collections.Generic;
using UnityEngine;

public class HeroCardManager : MonoBehaviour
{
    public static HeroCardManager Instance { get; private set; }

    public List<HeroCard> playerCards = new List<HeroCard>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddCard(HeroCardSO heroCardSO, int amount)
    {
        HeroCard card = playerCards.Find(c => c.heroCardSO == heroCardSO);
        if (card == null)
        {
            card = new HeroCard(heroCardSO);
            card.OnLevelUp += HandleLevelUp;
            card.Load(); 
            playerCards.Add(card);
        }

        card.AddCards(amount);
        card.Save(); 
    }

    private void HandleLevelUp(HeroCard card)
    {
        Debug.Log($"{card.heroCardSO.cardName} Level Up! New Level: {card.level}");
        card.Save();
    }

    public void SaveAllCards()
    {
        foreach (var card in playerCards)
        {
            card.Save();
        }
    }

    public void LoadAllCards()
    {
        foreach (var card in playerCards)
        {
            card.Load();
        }
    }

    public HeroCard GetCard(HeroCardSO heroCardSO)
    {
        return playerCards.Find(c => c.heroCardSO == heroCardSO);
    }
}
