using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlacementController : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;

    [Header("Settings")]
    public int maxCapacity = 30;
    public int currentUsedCapacity = 0;

    [Header("Card Panel")]
    [SerializeField] PlacementHeroData[] cards;
    [SerializeField] GameObject CardPrefab;
    [SerializeField] Transform cardParent;

    [Header("Placement")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Camera mainCamera;
    [SerializeField] Transform createTransform;
    private PlacementHeroData selectedUnitData;
    private bool isPlacing = false;
    [SerializeField] private Transform heroTransform;
    private List<Hero> placedUnits = new List<Hero>();

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI capacityText;

    private int[] activeCardIndexes = new int[3];
    private PlacementCardUI currentlySelectedCard;

    bool GameOver = false;

    private List<int> purchasedCardIndexes = new List<int>();

    private void Start()
    {
        LoadPurchasedHeroes();
        GenerateInitialCards();
    }

    private void LoadPurchasedHeroes()
    {
        purchasedCardIndexes.Clear();

        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i].IsPurchased())
            {
                purchasedCardIndexes.Add(i);
            }
        }

        if (purchasedCardIndexes.Count == 0)
        {
            Debug.LogWarning("Hiç satýn alýnmýþ karakter yok! En az bir karakteri açýk olarak iþaretleyin.");
        }
        else
        {
            Debug.Log($"Yüklenen satýn alýnmýþ karakter sayýsý: {purchasedCardIndexes.Count}");
        }
    }

    private void GenerateInitialCards()
    {
        if (purchasedCardIndexes.Count == 0)
        {
            Debug.LogError("Oyuna baþlamak için en az 1 karakter satýn alýnmalý!");
            return;
        }

        activeCardIndexes = new int[3];
        for (int i = 0; i < 3; i++)
        {
            activeCardIndexes[i] = purchasedCardIndexes[Random.Range(0, purchasedCardIndexes.Count)];
        }

        foreach (int index in activeCardIndexes)
        {
            CreateCardUI(index);
        }
    }

    private void CreateCardUI(int cardIndex)
    {
        GameObject cardObj = Instantiate(CardPrefab, cardParent);
        PlacementCardUI cardScript = cardObj.GetComponent<PlacementCardUI>();
        var data = cards[cardIndex];
        cardScript.Config(data.unitName, data.cardIcon, data.size, data.cost);
        cardScript.cardIndex = cardIndex;

        Button cardButton = cardScript.selectButton;
        cardButton.onClick.AddListener(() => SelectUnit(cardIndex, cardScript));
    }

    private void Update()
    {
        if (isPlacing && Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, groundLayer);

                if (hit.collider != null && CanPlaceUnit(selectedUnitData))
                {
                    var unitObj = Instantiate(selectedUnitData.prefab, hit.point, Quaternion.identity, createTransform);
                    Hero heroComponent = unitObj.GetComponent<Hero>();
                    if (heroComponent != null) heroComponent.Initialize(selectedUnitData);

                    PlaceUnit(selectedUnitData);
                    ReplaceCard(selectedUnitData);
                }
            }
        }

        if (CheckLoseCondition() && !GameOver)
        {
            GameOver = true;
            uiManager.GameLosePanel();
            Debug.Log("bitti oyun");
        }
    }

    public void ReduceCapacity(int size)
    {
        currentUsedCapacity = Mathf.Max(0, currentUsedCapacity - size);
        capacityText.text = $"{currentUsedCapacity} / {maxCapacity}";
    }

    public void SelectUnit(int unitData, PlacementCardUI placementCardUI)
    {
        if (currentlySelectedCard != null)
            currentlySelectedCard.SelectedImage().SetActive(false);

        placementCardUI.SelectedImage().SetActive(true);
        currentlySelectedCard = placementCardUI;
        selectedUnitData = cards[unitData];
        isPlacing = true;

        RectTransform rt = placementCardUI.GetComponent<RectTransform>();
        Vector3 originalScale = rt.localScale;
        rt.DOScale(originalScale * 1.1f, 0.15f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => rt.DOScale(originalScale, 0.15f));
    }

    public bool CanPlaceUnit(PlacementHeroData unit)
    {
        return (currentUsedCapacity + unit.size <= maxCapacity)
            && (DataManager.instance.GetGoldCount() >= unit.cost);
    }

    public void PlaceUnit(PlacementHeroData unit)
    {
        currentUsedCapacity += unit.size;

        DataManager.instance.TryPurchaseGold(unit.cost);

        capacityText.text = $"{currentUsedCapacity} / {maxCapacity}";

        if (currentlySelectedCard != null)
        {
            currentlySelectedCard.SelectedImage().SetActive(false);
            currentlySelectedCard = null;
            isPlacing = false;
        }

        if (CheckLoseCondition())
        {
            GameOver = true;
            uiManager.GameLosePanel();
        }
    }

    private bool CheckLoseCondition()
    {
        bool noUnitsOnScene = createTransform.childCount == 0;

        bool noPlayableCards = true;
        foreach (int index in activeCardIndexes)
        {
            if (CanPlaceUnit(cards[index]))
            {
                noPlayableCards = false;
                break;
            }
        }

        return noUnitsOnScene && noPlayableCards;
    }

    public void AddCapacity()
    {
        if (DataManager.instance.TryPurchaseGold(100))
        {
            maxCapacity += 1;
            capacityText.text = $"{currentUsedCapacity} / {maxCapacity}";
        }
    }

    private void ReplaceCard(PlacementHeroData placedUnit)
    {
        int replacedCardIndex = -1;
        int arrayPosition = -1;

        for (int i = 0; i < cardParent.childCount; i++)
        {
            PlacementCardUI ui = cardParent.GetChild(i).GetComponent<PlacementCardUI>();

            if (cards[ui.cardIndex] == placedUnit)
            {
                replacedCardIndex = ui.cardIndex;
                Destroy(cardParent.GetChild(i).gameObject);

                for (int j = 0; j < activeCardIndexes.Length; j++)
                {
                    if (activeCardIndexes[j] == replacedCardIndex)
                    {
                        arrayPosition = j;
                        break;
                    }
                }
                break;
            }
        }

        if (replacedCardIndex != -1 && arrayPosition != -1)
        {
            int newIndex = purchasedCardIndexes[Random.Range(0, purchasedCardIndexes.Count)];

            CreateCardUI(newIndex);

            activeCardIndexes[arrayPosition] = newIndex;
        }
    }

    public void RefreshPurchasedHeroes()
    {
        LoadPurchasedHeroes();
    }
}