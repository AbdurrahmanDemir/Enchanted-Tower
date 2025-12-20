using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlacementController : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;

    [Header("Elixir Settings")]
    public float maxElixir = 10f;
    public float currentElixir = 5f;
    public float elixirRegenRate = 1f;
    private float elixirRegenTimer = 0f;

    [Header("Card Panel")]
    [SerializeField] private PlacementHeroData[] cards;
    [SerializeField] private GameObject CardPrefab;
    [SerializeField] private Transform cardParent;

    [Header("Placement")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask towerLayer;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform createTransform;
    private PlacementHeroData selectedUnitData;
    private bool isPlacing = false;
    private bool isDragging = false;

    [Header("Tower Placement Settings")]
    [SerializeField] private float heroOverlapCheckRadius = 0.5f;
    [SerializeField] private float towerPadding = 0.05f;

    [Header("Drag Settings")]
    [SerializeField] private float dragThreshold = 10f; // Sürükleme baþlangýç mesafesi
    private Vector2 dragStartPos;
    private PlacementCardUI draggedCard;

    [Header("Visual Feedback")]
    [SerializeField] private Color validPlacementColor = new Color(0, 1, 0, 0.5f);
    [SerializeField] private Color invalidPlacementColor = new Color(1, 0, 0, 0.5f);
    private GameObject currentPreview;
    private SpriteRenderer previewRenderer;

    [Header("UI")]
    [SerializeField] private Slider elixirSlider;
    [SerializeField] private TextMeshProUGUI elixirText;

    private int[] activeCardIndexes = new int[3];
    private PlacementCardUI currentlySelectedCard;
    private bool GameOver = false;

    private readonly List<int> purchasedCardIndexes = new List<int>();

    private void Awake()
    {
        UpgradeSelectManager.addCapacity += AddElixirPowerUp;
    }

    private void OnDestroy()
    {
        UpgradeSelectManager.addCapacity -= AddElixirPowerUp;

        if (currentPreview != null)
            Destroy(currentPreview);
    }

    private void Start()
    {
        LoadPurchasedHeroes();
        GenerateInitialCards();
        UpdateElixirUI();
    }

    private void Update()
    {
        RegenerateElixir();

        // Sürükleme kontrolü
        if (isDragging)
        {
            HandleDragging();
        }
        else if (isPlacing)
        {
            ShowPlacementPreview();

            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, groundLayer);

                    if (hit.collider != null && CanPlaceUnit(selectedUnitData))
                    {
                        bool isTower = IsTowerUnit(selectedUnitData);

                        if (IsPositionValid(hit.point, isTower))
                        {
                            var unitObj = Instantiate(selectedUnitData.prefab, hit.point, Quaternion.identity, createTransform);
                            Hero heroComponent = unitObj.GetComponent<Hero>();
                            if (heroComponent != null) heroComponent.Initialize(selectedUnitData);

                            PlaceUnit(selectedUnitData);
                            ReplaceCard(selectedUnitData);
                        }
                        else
                        {
                            ShowInvalidPlacementFeedback(hit.point);
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelPlacement();
            }
        }

        if (CheckLoseCondition() && !GameOver)
        {
            GameOver = true;
            Debug.Log("bitti oyun");
        }
    }

    private void HandleDragging()
    {
        ShowPlacementPreview();

        // Mouse býrakýldýðýnda
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, groundLayer);

            bool placementSuccessful = false;

            if (hit.collider != null && CanPlaceUnit(selectedUnitData))
            {
                bool isTower = IsTowerUnit(selectedUnitData);

                if (IsPositionValid(hit.point, isTower))
                {
                    var unitObj = Instantiate(selectedUnitData.prefab, hit.point, Quaternion.identity, createTransform);
                    Hero heroComponent = unitObj.GetComponent<Hero>();
                    if (heroComponent != null) heroComponent.Initialize(selectedUnitData);

                    PlaceUnit(selectedUnitData);
                    ReplaceCard(selectedUnitData);
                    placementSuccessful = true;
                }
                else
                {
                    ShowInvalidPlacementFeedback(hit.point);
                }
            }

            // Yerleþtirme baþarýsýz olduysa kartý normal boyuta döndür
            if (!placementSuccessful)
            {
                if (currentlySelectedCard != null)
                {
                    RectTransform rt = currentlySelectedCard.GetComponent<RectTransform>();
                    rt.DOKill();
                    rt.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutQuad);
                }
                CancelPlacement();
            }

            isDragging = false;
        }
    }

    public void OnCardPointerDown(PlacementCardUI cardUI, int cardIndex)
    {
        dragStartPos = Input.mousePosition;
        draggedCard = cardUI;
        selectedUnitData = cards[cardIndex];
    }

    public void OnCardDrag(PlacementCardUI cardUI, int cardIndex)
    {
        if (draggedCard == null) return;

        float dragDistance = Vector2.Distance(dragStartPos, Input.mousePosition);

        // Belirli bir mesafe sürüklendiyse drag modunu baþlat
        if (dragDistance > dragThreshold && !isDragging)
        {
            // Ýksir kontrolü - yetersizse drag yapma
            if (!CanPlaceUnit(cards[cardIndex]))
            {
                draggedCard = null;
                return;
            }

            isDragging = true;
            isPlacing = true;

            if (currentlySelectedCard != null && currentlySelectedCard != draggedCard)
            {
                currentlySelectedCard.SelectedImage().SetActive(false);
                RectTransform oldRt = currentlySelectedCard.GetComponent<RectTransform>();
                oldRt.DOKill();
                oldRt.localScale = Vector3.one;
            }

            draggedCard.SelectedImage().SetActive(true);
            currentlySelectedCard = draggedCard;

            // Kartý biraz büyüt
            RectTransform rt = draggedCard.GetComponent<RectTransform>();
            rt.DOKill();
            rt.DOScale(Vector3.one * 1.1f, 0.15f).SetEase(Ease.OutQuad);
        }
    }

    public void OnCardPointerUp(PlacementCardUI cardUI, int cardIndex)
    {
        if (draggedCard != null && !isDragging)
        {
            // Sürükleme olmadý, normal týklama olarak iþle
            SelectUnit(cardIndex, cardUI);
        }

        // Kartý normal boyutuna döndür (eðer drag yapýldýysa)
        if (draggedCard != null && isDragging)
        {
            RectTransform rt = draggedCard.GetComponent<RectTransform>();
            rt.DOKill();
            rt.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutQuad);
        }

        draggedCard = null;
    }

    private void ShowPlacementPreview()
    {
        if (selectedUnitData == null || selectedUnitData.prefab == null) return;

        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (currentPreview == null && selectedUnitData.prefab != null)
        {
            currentPreview = new GameObject("PlacementPreview");
            previewRenderer = currentPreview.AddComponent<SpriteRenderer>();

            var prefabSR = selectedUnitData.prefab.GetComponent<SpriteRenderer>();
            Transform scaleSource = selectedUnitData.prefab.transform;

            if (prefabSR == null)
            {
                prefabSR = selectedUnitData.prefab.GetComponentInChildren<SpriteRenderer>();
                if (prefabSR != null) scaleSource = prefabSR.transform;
            }

            if (prefabSR != null)
            {
                previewRenderer.sprite = prefabSR.sprite;
                previewRenderer.flipX = prefabSR.flipX;
                previewRenderer.flipY = prefabSR.flipY;
                previewRenderer.sortingLayerName = "UI";
                previewRenderer.sortingOrder = 100;

                currentPreview.transform.localScale = scaleSource.lossyScale;
            }
        }

        currentPreview.transform.position = mousePosition;

        bool isTower = IsTowerUnit(selectedUnitData);
        bool isValid = IsPositionValid(mousePosition, isTower);

        previewRenderer.color = isValid ? validPlacementColor : invalidPlacementColor;
    }

    private Vector2 GetTowerCheckSize(PlacementHeroData unitData)
    {
        if (unitData == null || unitData.prefab == null) return Vector2.one;

        var box = unitData.prefab.GetComponent<BoxCollider2D>();
        if (box == null) return Vector2.one;

        Vector3 s = unitData.prefab.transform.lossyScale;
        Vector2 size = new Vector2(box.size.x * Mathf.Abs(s.x), box.size.y * Mathf.Abs(s.y));

        size += Vector2.one * (towerPadding * 2f);

        return size;
    }

    private bool IsPositionValid(Vector2 position, bool isTower)
    {
        if (selectedUnitData == null) return false;

        if (isTower)
        {
            Vector2 boxSize = GetTowerCheckSize(selectedUnitData);

            Collider2D[] hits = Physics2D.OverlapBoxAll(position, boxSize, 0f, towerLayer);
            if (hits != null && hits.Length > 0) return false;

            Collider2D[] allHits = Physics2D.OverlapBoxAll(position, boxSize, 0f);
            foreach (var col in allHits)
            {
                if (col.CompareTag("Tower") || col.CompareTag("EnemyTower")) return false;
                if (col.GetComponent<Tower>() != null) return false;
            }

            return true;
        }
        else
        {
            Collider2D[] obstacleColliders = Physics2D.OverlapCircleAll(position, heroOverlapCheckRadius);
            foreach (var col in obstacleColliders)
            {
                if (col.CompareTag("Obstacle")) return false;
            }
            return true;
        }
    }

    private bool IsTowerUnit(PlacementHeroData unitData)
    {
        if (unitData == null) return false;
        return unitData.IsTower();
    }

    private void ShowInvalidPlacementFeedback(Vector2 position)
    {
        if (PopUpController.instance != null)
            PopUpController.instance.OpenPopUp("You can't place a Tower here!");

        StartCoroutine(ShowInvalidEffect(position));
    }

    private IEnumerator ShowInvalidEffect(Vector2 position)
    {
        GameObject effect = new GameObject("InvalidEffect");
        effect.transform.position = position;

        SpriteRenderer sr = effect.AddComponent<SpriteRenderer>();
        sr.color = invalidPlacementColor;
        sr.sortingOrder = 100;

        Texture2D tex = new Texture2D(64, 64);
        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(32, 32));
                tex.SetPixel(x, y, dist < 30 ? Color.white : Color.clear);
            }
        }
        tex.Apply();

        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));

        sr.DOFade(0, 0.5f);
        effect.transform.DOScale(2f, 0.5f);

        yield return new WaitForSeconds(0.5f);
        Destroy(effect);
    }

    private void CancelPlacement()
    {
        isPlacing = false;
        isDragging = false;

        if (currentlySelectedCard != null)
        {
            currentlySelectedCard.SelectedImage().SetActive(false);

            // Kartý normal boyuta döndür
            RectTransform rt = currentlySelectedCard.GetComponent<RectTransform>();
            rt.DOKill();
            rt.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutQuad);

            currentlySelectedCard = null;
        }

        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }

        selectedUnitData = null;
        draggedCard = null;
    }

    private void RegenerateElixir()
    {
        if (currentElixir < maxElixir)
        {
            elixirRegenTimer += Time.deltaTime;
            if (elixirRegenTimer >= 1.8f)
            {
                currentElixir = Mathf.Min(currentElixir + elixirRegenRate, maxElixir);
                elixirRegenTimer = 0f;
                UpdateElixirUI();
            }
        }
    }

    private void UpdateElixirUI()
    {
        if (elixirSlider != null)
        {
            elixirSlider.maxValue = maxElixir;
            elixirSlider.value = currentElixir;
        }

        if (elixirText != null)
        {
            elixirText.text = Mathf.FloorToInt(currentElixir).ToString();
        }
    }

    private void LoadPurchasedHeroes()
    {
        purchasedCardIndexes.Clear();

        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i].IsPurchased())
                purchasedCardIndexes.Add(i);
        }
    }

    private void GenerateInitialCards()
    {
        if (purchasedCardIndexes.Count == 0) return;

        activeCardIndexes = new int[3];

        for (int i = 0; i < 3; i++)
            activeCardIndexes[i] = purchasedCardIndexes[Random.Range(0, purchasedCardIndexes.Count)];

        for (int i = 0; i < 3; i++)
            CreateCardUI(activeCardIndexes[i]);
    }

    private void CreateCardUI(int cardIndex)
    {
        GameObject cardObj = Instantiate(CardPrefab, cardParent);
        PlacementCardUI cardScript = cardObj.GetComponent<PlacementCardUI>();
        var data = cards[cardIndex];

        cardScript.Config(data.unitName, data.cardIcon, data.elixirCost, data.cost);
        cardScript.cardIndex = cardIndex;

        // EventTrigger ekle
        EventTrigger trigger = cardObj.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = cardObj.AddComponent<EventTrigger>();

        // PointerDown
        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => { OnCardPointerDown(cardScript, cardIndex); });
        trigger.triggers.Add(pointerDown);

        // Drag
        EventTrigger.Entry drag = new EventTrigger.Entry();
        drag.eventID = EventTriggerType.Drag;
        drag.callback.AddListener((data) => { OnCardDrag(cardScript, cardIndex); });
        trigger.triggers.Add(drag);

        // PointerUp
        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => { OnCardPointerUp(cardScript, cardIndex); });
        trigger.triggers.Add(pointerUp);

        Button cardButton = cardScript.selectButton;
        cardButton.onClick.AddListener(() => SelectUnit(cardIndex, cardScript));
    }

    public void SelectUnit(int unitData, PlacementCardUI placementCardUI)
    {
        if (!CanPlaceUnit(cards[unitData])) return;

        if (currentlySelectedCard != null)
        {
            currentlySelectedCard.SelectedImage().SetActive(false);
            // Önceki kartýn scale'ini düzelt
            RectTransform oldRt = currentlySelectedCard.GetComponent<RectTransform>();
            oldRt.DOKill();
            oldRt.localScale = Vector3.one;
        }

        placementCardUI.SelectedImage().SetActive(true);
        currentlySelectedCard = placementCardUI;

        selectedUnitData = cards[unitData];
        isPlacing = true;

        RectTransform rt = placementCardUI.GetComponent<RectTransform>();
        rt.DOKill(); // Mevcut animasyonlarý durdur
        rt.localScale = Vector3.one; // Önce normal boyuta getir

        rt.DOScale(Vector3.one * 1.1f, 0.15f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                rt.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutQuad);
            });
    }

    public bool CanPlaceUnit(PlacementHeroData unit)
    {
        return (currentElixir >= unit.elixirCost) && (DataManager.instance.GetGoldCount() >= unit.cost);
    }

    public void PlaceUnit(PlacementHeroData unit)
    {
        currentElixir -= unit.elixirCost;
        currentElixir = Mathf.Max(0, currentElixir);

        DataManager.instance.TryPurchaseGold(unit.cost);

        UpdateElixirUI();

        if (currentlySelectedCard != null)
        {
            RectTransform rt = currentlySelectedCard.GetComponent<RectTransform>();
            rt.DOKill();
            rt.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutQuad);

            currentlySelectedCard.SelectedImage().SetActive(false);
            currentlySelectedCard = null;
        }

        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }

        isPlacing = false;
        isDragging = false;

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
        for (int i = 0; i < activeCardIndexes.Length; i++)
        {
            int index = activeCardIndexes[i];
            if (CanPlaceUnit(cards[index]))
            {
                noPlayableCards = false;
                break;
            }
        }

        return noUnitsOnScene && noPlayableCards;
    }

    public void AddElixirPowerUp(int amount)
    {
        maxElixir += amount;
        UpdateElixirUI();
    }

    public void IncreaseElixirRegenRate(float amount)
    {
        elixirRegenRate += amount;
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

        if (replacedCardIndex == -1 || arrayPosition == -1) return;

        int newIndex = purchasedCardIndexes[Random.Range(0, purchasedCardIndexes.Count)];
        CreateCardUI(newIndex);
        activeCardIndexes[arrayPosition] = newIndex;
    }

    public void RefreshPurchasedHeroes()
    {
        LoadPurchasedHeroes();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if ((!isPlacing && !isDragging) || mainCamera == null || selectedUnitData == null) return;

        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        bool isTower = IsTowerUnit(selectedUnitData);

        Gizmos.color = IsPositionValid(mousePos, isTower) ? Color.green : Color.red;

        if (isTower)
        {
            Vector2 size = GetTowerCheckSize(selectedUnitData);
            Gizmos.DrawWireCube(mousePos, size);
        }
        else
        {
            Gizmos.DrawWireSphere(mousePos, heroOverlapCheckRadius);
        }
    }
#endif
}