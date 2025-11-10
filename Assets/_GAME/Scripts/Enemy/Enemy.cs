using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    public TeamType GetTeam() => TeamType.Enemy;

    [Header("NavMesh")]
    protected NavMeshAgent agent;
    protected GameObject currentTarget;

    [Header("Settings")]
    public EnemySO enemySO;
    protected float lastAttackTime = 0f;
    public LayerMask targetLayerMask;

    [Header("EnemySO")]
    public string enemyName;
    public Sprite enemyImage;
    public string attackType;
    public int damage;
    public float range;
    public float moveSpeed;
    public int health;
    public float cooldown;
    public float detectionRange = 5f;

    [Header("Elements")]
    public Animator animator;
    private Slider healthSlider;
    private SpriteRenderer characterSpriteRenderer;
    private Color originalColor;
    private Vector2 originalScale;
    public Vector2 scaleReduction = new Vector3(0.9f, 0.9f, 1f);

    [Header("Action")]
    private bool onThrow = false;
    public static Action<Vector2> onDead;
    public static Action OnAnyEnemyHealthChanged;

    private void Awake()
    {
        UpgradeSelectManager.onPowerUpPanelOpened += OnThrowStartingCallBack;
        UpgradeSelectManager.onPowerUpPanelClosed += OnThrowEndingCallBack;
    }

    private void OnDestroy()
    {
        UpgradeSelectManager.onPowerUpPanelOpened -= OnThrowStartingCallBack;
        UpgradeSelectManager.onPowerUpPanelClosed -= OnThrowEndingCallBack;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        characterSpriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = characterSpriteRenderer.color;
        originalScale = transform.localScale;

        Initialize(enemySO);

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = moveSpeed;

        if (SceneManager.GetActiveScene().name == "PixelGame")
                EnemyBaseManager.instance.RegisterObject(gameObject.name);
    }

    public void Initialize(EnemySO so)
    {
        enemySO = so;
        enemyName = enemySO.enemyName;
        enemyImage = enemySO.enemyImage;
        attackType = enemySO.attackType;
        damage = enemySO.GetEnemyDamage();
        range = enemySO.range;
        moveSpeed = enemySO.moveSpeed;
        cooldown = enemySO.cooldown;
        health = enemySO.GetEnemyHealth();

        if (healthSlider == null)
            healthSlider = GetComponentInChildren<Slider>();

        healthSlider.maxValue = health;
        healthSlider.value = health;
    }

    private void Update()
    {
        if (onThrow) return;

        if (currentTarget == null)
        {
            currentTarget = FindTargetInDetectionRange();
        }

        if (currentTarget != null)
        {
            // Yön dönüþü
            if (currentTarget.transform.position.x < transform.position.x)
                transform.rotation = Quaternion.Euler(0, -180, 0);
            else
                transform.rotation = Quaternion.Euler(0, 0, 0);

            float distanceToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);

            if (distanceToTarget > detectionRange * 1.5f)
            {
                currentTarget = null;
                animator.Play("idle");
                agent.SetDestination(transform.position);
                return;
            }

            if (distanceToTarget <= range)
            {
                agent.SetDestination(transform.position); // dur
                Attack(currentTarget);
            }
            else
            {
                agent.SetDestination(currentTarget.transform.position);
                animator.Play("run");
            }
        }
        else
        {
            animator.Play("idle");
            agent.SetDestination(transform.position);
        }
    }

    private GameObject FindTargetInDetectionRange()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, detectionRange, targetLayerMask);
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var target in targets)
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

    protected virtual void Attack(GameObject target)
    {
        if (Time.time - lastAttackTime >= cooldown)
        {
            lastAttackTime = Time.time;

            if (enemySO.isAreaOfEffect)
                PerformAreaAttack();
            else
                PerformSingleTargetAttack(target);

            animator.Play("attack");
        }
    }

    protected abstract void PerformSingleTargetAttack(GameObject target);
    protected abstract void PerformAreaAttack();

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        healthSlider.value = health;
        OnAnyEnemyHealthChanged?.Invoke();

        characterSpriteRenderer.DOKill();
        characterSpriteRenderer.DOColor(Color.gray, 0.1f).OnComplete(() =>
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
            if (SceneManager.GetActiveScene().name == "PixelGame")
                EnemyBaseManager.instance.UnRegisterObject();

            onDead?.Invoke(transform.position);
            Destroy(gameObject);
        }
    }

    public void OnThrowStartingCallBack() => onThrow = true;
    public void OnThrowEndingCallBack() => onThrow = false;
    public int GetCurrentHealth() { return health; }
    public void ResetStats()
    {
        moveSpeed = enemySO.moveSpeed;
        cooldown = enemySO.cooldown;

        if (agent != null)
        {
            agent.speed = moveSpeed;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
#endif
}
