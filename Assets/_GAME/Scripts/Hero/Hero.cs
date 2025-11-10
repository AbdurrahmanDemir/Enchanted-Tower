using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public abstract class Hero : MonoBehaviour, IDamageable
{
    public TeamType GetTeam() => TeamType.Hero;

    [Header("NavMesh")]
    protected NavMeshAgent agent;
    protected GameObject currentTarget;


    [Header("Settings")]
    public HeroSO heroSO;
    protected float lastAttackTime = 0f;
    public LayerMask targetLayerMask;

    [Header("HeroSO")]
    protected string heroName;
    protected Sprite heroImage;
    protected string attackType;
    protected bool isAreaOfEffect;
    protected int damage;
    protected float range;
    protected float moveSpeed;
    protected float cooldown;
    public int health;

    [Header("Elements")]
    protected Animator animator;
     private Slider healthSlider;
    SpriteRenderer characterSpriteRenderer;
    private Color originalColor;
    private Vector2 originalScale;
    public Vector2 scaleReduction = new Vector3(0.9f, 0.9f, 1f);

    [Header("Action")]
    protected bool onThrow = false;
    public static Action OnAnyHeroHealthChanged;

  
    private void Awake()
    {
        UpgradeSelectManager.heroDamageItem += PowerUpHeroDamage;
        UpgradeSelectManager.heroHealthItem += PowerUpHeroHealth;

        UpgradeSelectManager.onPowerUpPanelOpened += OnThrowStartingCallBack;
        UpgradeSelectManager.onPowerUpPanelClosed += OnThrowEndingCallBack;

        heroName = heroSO.heroName;
        heroImage = heroSO.heroImage;
        attackType = heroSO.attackType;
        isAreaOfEffect = heroSO.isAreaOfEffect;
        damage = heroSO.GetCurrentDamage();
        range = heroSO.range;
        moveSpeed = heroSO.moveSpeed;
        cooldown = heroSO.cooldown;


        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = moveSpeed;


        //TowerController.onGameLose += OnThrowStartingCallBack;
        //EnemyTowerController.onGameWin += OnThrowStartingCallBack;
    }
    private void OnDestroy()
    {
        //Hook.onThrowStarting -= OnThrowStartingCallBack;
        //Hook.onThrowEnding -= OnThrowEndingCallBack;

        UpgradeSelectManager.heroDamageItem -= PowerUpHeroDamage;
        UpgradeSelectManager.heroHealthItem -= PowerUpHeroHealth;

        UpgradeSelectManager.onPowerUpPanelOpened -= OnThrowStartingCallBack;
        UpgradeSelectManager.onPowerUpPanelClosed -= OnThrowEndingCallBack;



        //TowerController.onGameLose -= OnThrowStartingCallBack;
        //EnemyTowerController.onGameWin -= OnThrowStartingCallBack;

    }

    private void Start()
    {
        animator = GetComponent<Animator>();

        characterSpriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = characterSpriteRenderer.color;
        originalScale = transform.localScale;

       

        healthSlider = GetComponentInChildren<Slider>();
        healthSlider.maxValue = heroSO.maxHealth;
        health = heroSO.GetCurrentHealth();
        healthSlider.value = health;
    }

    protected virtual void Update()
    {
        if (onThrow) return;

        if (currentTarget == null)
        {
            currentTarget = FindClosestTarget();
        }

        if (currentTarget == null) return;

        if (currentTarget != null)
        {
            if (currentTarget.gameObject.transform.position.x < transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, -180, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        float dist = Vector2.Distance(transform.position, currentTarget.transform.position);

        if (dist <= range)
        {
            agent.SetDestination(transform.position); 
            Attack(currentTarget);                    
            animator.Play("attack");
        }
        else
        {
            agent.SetDestination(currentTarget.transform.position);
            animator.Play("run");
        }
    }

    protected virtual void Attack(GameObject target)
    {

        if(Time.time- lastAttackTime>= heroSO.cooldown)
        {
            lastAttackTime = Time.time;


            if (heroSO.isAreaOfEffect)
            {
                PerformAreaAttack();
                animator.Play("attack");
            }
            else
            {
                PerformSingleTargetAttack(target);
                animator.Play("attack");
            }
        }
    }
    protected abstract void PerformSingleTargetAttack(GameObject target);
    protected abstract void PerformAreaAttack();

    protected GameObject FindClosestTarget()
    {
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, 100, targetLayerMask);
        foreach (var target in potentialTargets)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target.gameObject;
            }
        }

        return closestTarget;
    }
    public void MoveTowardsTarget(Vector2 targetPosition)
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }


    public virtual void TakeDamage(int damage)
    {
        health-=damage;
        healthSlider.value = health;
        OnAnyHeroHealthChanged?.Invoke();

        characterSpriteRenderer.DOKill();
        characterSpriteRenderer.DOColor(Color.red, 0.1f).OnComplete(() =>
        {
            characterSpriteRenderer.DOColor(originalColor, 0.1f);
        });

        transform.DOKill();
        transform.DOScale(originalScale * scaleReduction, 0.1f).OnComplete(() =>
        {
            transform.DOScale(originalScale, 0.1f);
        });


        if (health <= 0)
        {
            Debug.Log("hero öldü");

            //if (placementData != null && PlacementManager.instance != null)
            //{
            //    PlacementManager.instance.ReduceCapacity(placementData.size);
            //}

            Destroy(gameObject);
        }

    }
    public void PowerUpHeroHealth(int amount)
    {
        health += amount;
        healthSlider.value = health;
    }
    public void PowerUpHeroDamage(int amount)
    {
        damage += amount;
    }

    public void OnThrowStartingCallBack()
    {
        onThrow = true;
        Debug.Log("Avtipn çalýþtý" + onThrow);
    }
    public void OnThrowEndingCallBack()
    {
        onThrow = false;

        Debug.Log("Avtipn çalýþtý" + onThrow);

    }
    public bool IsFullHealth()
    {
        return health >= heroSO.maxHealth;
    }
    public int GetMissingHealth()
    {
        return heroSO.maxHealth - health;
    }

    public int GetCurrentHealth()
    {
        return health;
    }
    public PlacementHeroData placementData;

    public void Initialize(PlacementHeroData data)
    {
        placementData = data;
    }

    bool isFrozen=false;
    public void Freeze(float duration)
    {
        if (!isFrozen)
        {
            isFrozen = true;
            moveSpeed = 0f; 
            StartCoroutine(UnfreezeAfter(duration));
        }
    }

    private IEnumerator UnfreezeAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        moveSpeed = heroSO.moveSpeed;
        isFrozen = false;
    }

}
