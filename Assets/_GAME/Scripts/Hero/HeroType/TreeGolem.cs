using System;
using UnityEngine;

public class TreeGolem : Hero
{
    [SerializeField] private Transform bulletSpawnPoint;

    public static Action<BulletData> OnBulletRequested;

    private void Update()
    {
        if (heroStopped) return;

        GameObject target = FindClosestInjuredHero();
        if (target == null)
        {
            animator.Play("idle");
            return;
        }

        float distance = Vector2.Distance(transform.position, target.transform.position);
        if (distance <= heroSO.range)
        {
            Attack(target);
        }
        else
        {
            MoveTowardsTarget(target.transform.position);
            animator.Play("run");
        }
    }

    protected override void PerformAreaAttack()
    {
        // Sýhhiyeci alan hasarý yok
    }

    protected override void PerformSingleTargetAttack(GameObject target)
    {
        OnBulletRequested?.Invoke(new BulletData
        {
            spawnPosition = bulletSpawnPoint.position,
            target = target,
            dataSO = heroSO,
            firePoint = bulletSpawnPoint
        });
    }

    private GameObject FindClosestInjuredHero()
    {
        GameObject closest = null;
        float closestDistance = Mathf.Infinity;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 100f, targetLayerMask);
        foreach (var hit in hits)
        {
            Hero hero = hit.GetComponent<Hero>();
            if (hero != null && !hero.IsFullHealth())
            {
                float dist = Vector2.Distance(transform.position, hero.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closest = hero.gameObject;
                }
            }
        }

        return closest;
    }
}
