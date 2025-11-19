using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuHeroDetails : MonoBehaviour
{
    [Header("Card")]
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private Image cardIconImage;
    [SerializeField] private GameObject[] cardTypes;
    [SerializeField] private TextMeshProUGUI cardText;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI moveSpeedText;
    [SerializeField] private TextMeshProUGUI specialStatNameText;
    [SerializeField] private TextMeshProUGUI specialStatText;

    [Header("Other")]
    [SerializeField] private TextMeshProUGUI upgradePriceText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI buyPriceText;

    private MenuHeroCardSO currentHeroCard;

    public void Config(MenuHeroCardSO heroCard, string name, Sprite icon, string type, string cardDetails,
                       int damage, int health, float range, float attackSpeed, float moveSpeed,
                       float price, string specialStatName, float specialStat, int buyPrice)
    {
        currentHeroCard = heroCard;

        cardNameText.text = name;
        cardIconImage.sprite = icon;
        cardText.text = cardDetails;
        damageText.text = damage.ToString();
        healthText.text = health.ToString();
        rangeText.text = range.ToString();
        attackSpeedText.text = attackSpeed.ToString();
        moveSpeedText.text = moveSpeed.ToString();
        upgradePriceText.text = price.ToString();
        specialStatNameText.text = specialStatName;
        specialStatText.text = specialStat.ToString();
        buyPriceText.text = buyPrice.ToString();

        // Buton durumlarýný güncelle
        UpdateButtonStates();

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

    public void UpdateButtonStates()
    {
        if (currentHeroCard == null) return;

        bool isPurchased = currentHeroCard.IsPurchased();
        bool canAfford = DataManager.instance.GetGoldCount() >= currentHeroCard.purchasePrice;

        if (currentHeroCard.requiresPurchase && !isPurchased)
        {
            buyButton.gameObject.SetActive(true);
            buyButton.interactable = canAfford;
            upgradeButton.gameObject.SetActive(false);
        }
        else
        {
            buyButton.gameObject.SetActive(false);
            upgradeButton.gameObject.SetActive(true);
        }
    }

    public Button GetUpgradeButton()
    {
        return upgradeButton;
    }

    public Button GetBuyingButton()
    {
        return buyButton;
    }

    public void TogglePanel()
    {
        if (gameObject.activeSelf)
        {
            gameObject.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
            DOTween.Kill(gameObject);
            Destroy(gameObject, 5f);
        }
        else
        {
            gameObject.SetActive(true);
            gameObject.transform.localScale = Vector3.zero;
            gameObject.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }
    }
}