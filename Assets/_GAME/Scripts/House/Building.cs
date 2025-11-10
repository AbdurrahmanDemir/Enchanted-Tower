using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Building : MonoBehaviour
{    public TeamType GetTeam() => TeamType.Building;

    [Header("Settings")]
    public BuildingSO buildingSO;
    protected float lastAttackTime = 0f;

    [Header("House Data")]
    protected string buildingName;
    protected Sprite buildingImage;
    protected BuildingType buildingType;
    protected float productionTime;

    [Header("Elements")]
    protected Animator animator;
    public Slider productionSlider;
    SpriteRenderer characterSpriteRenderer;
    private Color originalColor;
    private Vector2 originalScale;
    public Vector2 scaleReduction = new Vector3(0.9f, 0.9f, 1f);

    protected bool isProducing = false;

    public virtual void Start()
    {
        animator = GetComponent<Animator>();

        characterSpriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = characterSpriteRenderer.color;
        originalScale = transform.localScale;

        buildingImage = buildingSO.houseImage;
        buildingName = buildingSO.name;
        buildingType = buildingSO.type;
        productionTime = buildingSO.productionTime;
    }
    public void StartProduction()
    {
        if (!isProducing)
        {
            Debug.Log("ÇALIÞTI1");
            StartCoroutine(Produce());
        }
    }

    private IEnumerator Produce()
    {
        isProducing = true;
        float timer = 0f;

        while (timer < productionTime)
        {
            timer += Time.deltaTime;
            if (productionSlider != null)
                productionSlider.value = timer / productionTime;
            yield return null;
        }

        isProducing = false;       
        OnProductionComplete();
        Debug.Log("ÇALIÞTI2");

    }


    protected abstract void OnProductionComplete();
}
