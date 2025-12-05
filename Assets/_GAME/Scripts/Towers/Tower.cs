using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public abstract class Tower : MonoBehaviour, IDamageable
{
    public TeamType GetTeam() => TeamType.Building;
    [Header("General Settings")]
    public TowerData towerData;
    public LayerMask targetMask;
    public Transform firePoint;

    protected float lastAttackTime;

    [Header("Tower")]
    protected int health;

    [Header("Elements")]
    protected Animator animator;
    private Slider healthSlider;
    SpriteRenderer characterSpriteRenderer;
    private Color originalColor;
    private Vector2 originalScale;
    public Vector2 scaleReduction = new Vector3(0.9f, 0.9f, 1f);

    public static Action<Vector2> onDead;

    private void Start()
    {
        animator = GetComponent<Animator>();

        characterSpriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = characterSpriteRenderer.color;
        originalScale = transform.localScale;



        healthSlider = GetComponentInChildren<Slider>();
        health = towerData.health;
        healthSlider.maxValue = towerData.health;
        healthSlider.value = health;
        if (SceneManager.GetActiveScene().name == "PixelGame")
            EnemyBaseManager.instance.RegisterObject(gameObject.name);

    }
    protected virtual void Update()
    {
        GameObject target = FindTarget();

        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.transform.position);
        if (distance <= towerData.range && Time.time - lastAttackTime >= towerData.attackCooldown)
        {
            lastAttackTime = Time.time;
            Attack(target);
        }
    }

    protected abstract void Attack(GameObject target);

    protected GameObject FindTarget()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, towerData.range, targetMask);

        if (targets.Length == 0)
            return null;

        switch (towerData.targetingType)
        {
            case TargetPriorityType.Closest:
                return FindClosest(targets);
            case TargetPriorityType.Random:
                return targets[Random.Range(0, targets.Length)].gameObject;
            case TargetPriorityType.HighestHealth:
                return FindHighestHealth(targets);
            default:
                return null;
        }
    }

    private GameObject FindClosest(Collider2D[] targets)
    {
        float closestDistance = Mathf.Infinity;
        GameObject closest = null;

        foreach (var col in targets)
        {
            float dist = Vector2.Distance(transform.position, col.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = col.gameObject;
            }
        }

        return closest;
    }

    private GameObject FindHighestHealth(Collider2D[] targets)
    {
        int maxHealth = -1;
        GameObject highest = null;

        foreach (var col in targets)
        {
            if (col.TryGetComponent<Enemy>(out var enemy))
            {
                if (enemy.health > maxHealth)
                {
                    maxHealth = enemy.health;
                    highest = enemy.gameObject;
                }
            }
        }

        return highest;
    }
    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        healthSlider.value = health;

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
            if (SceneManager.GetActiveScene().name == "PixelGame")
                EnemyBaseManager.instance.UnRegisterObject();

            onDead?.Invoke(transform.position);
            Debug.Log("tower öldü");
            Destroy(gameObject);


        }
    }
    public int GetCurrentHealth()
    {
        return health;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, towerData.range);
    }
}
