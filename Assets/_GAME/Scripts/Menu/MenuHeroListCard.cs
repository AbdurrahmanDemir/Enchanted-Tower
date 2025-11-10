using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuHeroListCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private Image cardIconImage;
    [SerializeField] private GameObject[] cardTypes;
    [SerializeField] private Slider cardSlider;
    [SerializeField] private TextMeshProUGUI cardSliderText;
    [SerializeField] private TextMeshProUGUI heroCardLevel;

    public Button detailsButton;
    public int cardIndex;

    private HeroCard heroCard;

    public void Config(string name, Sprite icon, string type)
    {
        cardNameText.text = name;
        cardIconImage.sprite = icon;
        switch (type)
        {
            case "Cammon":
                cardTypes[0].SetActive(true);
                cardTypes[1].SetActive(false);
                cardTypes[2].SetActive(false);
                cardTypes[3].SetActive(false);
                break;
            case "Rare":
                cardTypes[0].SetActive(false);
                cardTypes[1].SetActive(true);
                cardTypes[2].SetActive(false);
                cardTypes[3].SetActive(false);
                break;
            case "Epic":
                cardTypes[0].SetActive(false);
                cardTypes[1].SetActive(false);
                cardTypes[2].SetActive(true);
                cardTypes[3].SetActive(false);
                break;
            case "Legendary":
                cardTypes[0].SetActive(false);
                cardTypes[1].SetActive(false);
                cardTypes[2].SetActive(false);
                cardTypes[3].SetActive(true);
                break;
        }
    }

    public void SetHeroCard(HeroCard card)
    {
        heroCard = card;

        cardNameText.text = heroCard.heroCardSO.cardName;
        cardIconImage.sprite = heroCard.heroCardSO.heroIcon; 
        heroCardLevel.text = $"Level {heroCard.level}";

        UpdateSliderUI();

        heroCard.OnLevelUp += (c) => RefreshUI();
    }

    private void UpdateSliderUI()
    {
        if (heroCard.level <= heroCard.heroCardSO.maxLevel)
        {
            int requiredCards = heroCard.heroCardSO.upgradeRequirements[heroCard.level - 1];

            cardSlider.maxValue = requiredCards;
            cardSlider.value = heroCard.collectedCards;
            cardSliderText.text = $"{heroCard.collectedCards}/{requiredCards}";
            heroCardLevel.text = $"Level {heroCard.level}";
        }
        else
        {
            cardSlider.maxValue = 1;
            cardSlider.value = 1;
            cardSliderText.text = "Max Level";
            heroCardLevel.text = $"Level {heroCard.level}";
        }
    }

    public void RefreshUI()
    {
        UpdateSliderUI();
    }
}
