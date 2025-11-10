using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroCardsList : MonoBehaviour
{
    //[Header("Hero Panel")]
    //[SerializeField] private HeroCardSO[] heroes; 
    //[SerializeField] private GameObject heroCardPrefab;
    //[SerializeField] private Transform heroTransform;
    //[SerializeField] private GameObject heroCardDetailsPrefabs;
    //[SerializeField] private Transform heroDetailsTransform;

    //private void Start()
    //{
    //    HeroPanelUpdate();
    //}

    //public void HeroPanelUpdate()
    //{
    //    foreach (Transform child in heroTransform)
    //    {
    //        Destroy(child.gameObject);
    //    }

    //    for (int i = 0; i < heroes.Length; i++)
    //    {
    //        GameObject cardPrefabs = Instantiate(heroCardPrefab, heroTransform);
    //        MenuHeroListCard heroScript = cardPrefabs.GetComponent<MenuHeroListCard>();

    //        HeroCard card = HeroCardManager.Instance.GetCard(heroes[i]);
    //        if (card == null)
    //        {
    //            card = new HeroCard(heroes[i]);
    //            card.Load(); 
    //            HeroCardManager.Instance.playerCards.Add(card);
    //        }

    //        heroScript.SetHeroCard(card);

    //        Button cardButton = heroScript.detailsButton;
    //        int capturedIndex = i;
    //        cardButton.onClick.AddListener(() => CardDetailsPanel(capturedIndex));
    //    }
    //}

    //public void CardDetailsPanel(int index)
    //{
    //    foreach (Transform child in heroDetailsTransform)
    //        Destroy(child.gameObject);

    //    GameObject cardDetails = Instantiate(heroCardDetailsPrefabs, heroDetailsTransform);
    //    MenuHeroDetails cardScript = cardDetails.GetComponent<MenuHeroDetails>();
    //    DOTween.Kill(cardDetails.transform);
    //    cardDetails.transform.localScale = Vector3.zero;

    //    if (cardDetails != null)
    //        cardDetails.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);

    //    HeroCard card = HeroCardManager.Instance.GetCard(heroes[index]);

    //    cardScript.Config(
    //                    heroes[index],

    //        heroes[index].name,
    //        heroes[index].heroIcon,
    //        heroes[index].cardType.ToString(),
    //        heroes[index].cardDescription,
    //        heroes[index].GetCurrentDamage(card.level),
    //        heroes[index].GetCurrentHealth(card.level),
    //        heroes[index].range,
    //        heroes[index].cooldown,
    //        heroes[index].moveSpeed,
    //        heroes[index].GetUpgradeCost(card.level),
    //        heroes[index].specialStatName,
    //        heroes[index].specialStat,
    //        heroes[index].purchasePrice
    //    );

    //    Button upgradeButton = cardScript.GetUpgradeButton();

    //    upgradeButton.onClick.RemoveAllListeners();
    //    upgradeButton.onClick.AddListener(() =>
    //    {
    //        card.AddCards(card.heroCardSO.upgradeRequirements[card.level - 1]); 
    //        HeroCardManager.Instance.SaveAllCards();
    //        HeroPanelUpdate(); 
    //        CardDetailsPanel(index); 
    //    });
    //}
}
