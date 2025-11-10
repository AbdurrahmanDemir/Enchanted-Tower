using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LibraryList : MonoBehaviour
{
    [Header("Hero Panel")]
    [SerializeField] BonusCardSO[] bonusCard;
    [SerializeField] GameObject bonusCardPrefab;
    [SerializeField] Transform bonusCardParent;
    [SerializeField] GameObject bonusCardDetailsPrefabs;
    [SerializeField] Transform bonusCardDetailsParent;

    private void Start()
    {
        BonusCard();
    }

    public void BonusCard()
    {
        for (int i = 0; i < bonusCard.Length; i++)
        {
            GameObject cardPrefabs = Instantiate(bonusCardPrefab, bonusCardParent);

            BonusCard heroScript = cardPrefabs.GetComponent<BonusCard>();

            heroScript.Config(
                bonusCard[i].name,
                bonusCard[i].heroIcon
                );

            heroScript.cardIndex = i;

            Button cardButton = heroScript.detailsButton;

            int capturedIndex = i;
            cardButton.onClick.AddListener(() => CardDetailsPanel(capturedIndex));
        }
    }

    public void CardDetailsPanel(int index)
    {
        foreach (Transform child in bonusCardDetailsParent)
        {
            Destroy(child.gameObject);
        }

        GameObject cardDetails = Instantiate(bonusCardDetailsPrefabs, bonusCardDetailsParent);
        BonusCardDetails cardScript = cardDetails.GetComponent<BonusCardDetails>();
        DOTween.Kill(cardDetails.transform);
        cardDetails.transform.localScale = Vector3.zero;
        if (cardDetails != null)
        {
            cardDetails.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }

        cardScript.Config(
            bonusCard[index].name,
            bonusCard[index].heroIcon,
            bonusCard[index].cardDescription
        );

    }

}
