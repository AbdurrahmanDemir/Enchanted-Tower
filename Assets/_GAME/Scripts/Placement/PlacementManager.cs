using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class PlacementManager : MonoBehaviour
{
    //public static PlacementManager instance;

    //[Header("Settings")]
    //public int maxCapacity = 30;
    //public int currentUsedCapacity = 0;

    //[Header("Card Panel")]
    //[SerializeField] PlacementHeroData[] cards; 
    //[SerializeField] GameObject CardPrefab;
    //[SerializeField] Transform cardParent;

    //[Header("Placement")]
    //[SerializeField] private LayerMask groundLayer;
    //[SerializeField] private Camera mainCamera;
    //[SerializeField] Transform createTransform;
    //private PlacementHeroData selectedUnitData;
    //private bool isPlacing = false;

    //[Header("Grid System")]
    //[SerializeField] private float cellSize = 1f; // grid hücrelerinin geniþliði
    //[SerializeField] private Vector2 gridOrigin = Vector2.zero; // grid baþlangýç noktasý (sol-alt köþe)


    //[Header("UI")]
    //[SerializeField] private TextMeshProUGUI capacityText;

    //// Aktif gösterilen kart indeksleri
    //private int[] activeCardIndexes = new int[3];
    //private PlacementCardUI currentlySelectedCard;

    //private void Awake()
    //{
    //    if (instance != null && instance != this)
    //        Destroy(gameObject);
    //    else
    //        instance = this;
    //}

    //private void Start()
    //{
    //    GenerateInitialCards();
    //}

    //private void GenerateInitialCards()
    //{
    //    activeCardIndexes = Enumerable.Range(0, cards.Length)
    //                                  .OrderBy(x => Random.value)
    //                                  .Take(3)
    //                                  .ToArray();

    //    foreach (int index in activeCardIndexes)
    //    {
    //        CreateCardUI(index);
    //    }
    //}

    //private void CreateCardUI(int cardIndex)
    //{
    //    GameObject cardObj = Instantiate(CardPrefab, cardParent);
    //    PlacementCardUI cardScript = cardObj.GetComponent<PlacementCardUI>();

    //    var data = cards[cardIndex];
    //    cardScript.Config(data.name, data.cardIcon, data.size, data.cost);
    //    cardScript.cardIndex = cardIndex;

    //    Button cardButton = cardScript.selectButton;
    //    cardButton.onClick.AddListener(() => SelectUnit(cardIndex, cardScript));

    //}
    //private Vector2 SnapToGrid(Vector2 rawPos)
    //{
    //    float x = Mathf.Floor((rawPos.x - gridOrigin.x) / cellSize) * cellSize + cellSize / 2f + gridOrigin.x;
    //    float y = Mathf.Floor((rawPos.y - gridOrigin.y) / cellSize) * cellSize + cellSize / 2f + gridOrigin.y;
    //    return new Vector2(x, y);
    //}

    //private void Update()
    //{
    //    // UPDATE içerisinde:
    //    if (isPlacing)
    //    {
    //        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    //        Vector2 snappedPos = SnapToGrid(mousePos);

    //        // çarpýþma kontrolü için snappedPos kullan
    //        var size = selectedUnitData.prefab.GetComponent<BoxCollider2D>().size;
    //        Collider2D overlap = Physics2D.OverlapBox(snappedPos, size, 0f);
    //        bool can = (overlap == null || overlap.CompareTag("Ground"));
    //    }

    //    if (isPlacing && Input.GetMouseButtonDown(0))
    //    {
    //        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    //        Vector2 snappedPos = SnapToGrid(mousePos);

    //        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, groundLayer);
    //        if (hit.collider != null && CanPlaceUnit(selectedUnitData))
    //        {
    //            var size = selectedUnitData.prefab.GetComponent<BoxCollider2D>().size;
    //            Collider2D overlap = Physics2D.OverlapBox(snappedPos, size, 0f);
    //            if (overlap != null && !overlap.CompareTag("Ground"))
    //            {
    //                Debug.Log("Buraya yerleþtirilemez");
    //                return;
    //            }

    //            var unitObj = Instantiate(selectedUnitData.prefab, snappedPos, Quaternion.identity, createTransform);
    //            Hero heroComponent = unitObj.GetComponent<Hero>();
    //            if (heroComponent != null)
    //                heroComponent.Initialize(selectedUnitData);

    //            PlaceUnit(selectedUnitData);
    //            ReplaceCard(selectedUnitData);
    //        }
    //    }

    //}

    //public void ReduceCapacity(int size)
    //{
    //    currentUsedCapacity = Mathf.Max(0, currentUsedCapacity - size);
    //    capacityText.text = $"{currentUsedCapacity} / {maxCapacity}";
    //}

    //public void SelectUnit(int unitData, PlacementCardUI placementCardUI)
    //{
    //    if (currentlySelectedCard != null)
    //        currentlySelectedCard.SelectedImage().SetActive(false);

    //    placementCardUI.SelectedImage().SetActive(true);
    //    currentlySelectedCard = placementCardUI;

    //    selectedUnitData = cards[unitData];
    //    isPlacing = true;

    //    RectTransform rt = placementCardUI.GetComponent<RectTransform>();
    //    Vector3 originalScale = rt.localScale;
    //    rt.DOScale(originalScale * 1.1f, 0.15f)
    //      .SetEase(Ease.OutQuad)
    //      .OnComplete(() => rt.DOScale(originalScale, 0.15f));
    //}

    //public bool CanPlaceUnit(PlacementHeroData unit)
    //{
    //    return (currentUsedCapacity + unit.size <= maxCapacity) &&
    //           (DataManager.instance.TryPurchaseGold(unit.cost));
    //}

    //public void PlaceUnit(PlacementHeroData unit)
    //{
    //    currentUsedCapacity += unit.size;
    //    DataManager.instance.TryPurchaseGold(unit.cost);
    //    capacityText.text = $"{currentUsedCapacity} / {maxCapacity}";

    //    if (currentlySelectedCard != null)
    //    {
    //        currentlySelectedCard.SelectedImage().SetActive(false);
    //        currentlySelectedCard = null;
    //        isPlacing = false;
    //    }
    //}

    //private void ReplaceCard(PlacementHeroData placedUnit)
    //{
    //    for (int i = 0; i < cardParent.childCount; i++)
    //    {
    //        PlacementCardUI ui = cardParent.GetChild(i).GetComponent<PlacementCardUI>();
    //        if (cards[ui.cardIndex] == placedUnit)
    //        {
    //            Destroy(cardParent.GetChild(i).gameObject);

    //            var availableIndexes = Enumerable.Range(0, cards.Length)
    //                                             .Where(x => !activeCardIndexes.Contains(x))
    //                                             .OrderBy(x => Random.value)
    //                                             .ToList();

    //            if (availableIndexes.Count > 0)
    //            {
    //                int newIndex = availableIndexes[0];
    //                CreateCardUI(newIndex);

    //                for (int j = 0; j < activeCardIndexes.Length; j++)
    //                {
    //                    if (activeCardIndexes[j] == ui.cardIndex)
    //                    {
    //                        activeCardIndexes[j] = newIndex;
    //                        break;
    //                    }
    //                }
    //            }
    //            break;
    //        }
    //    }
    //}
}
