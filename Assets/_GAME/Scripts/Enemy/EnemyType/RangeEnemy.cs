using TMPro;
using UnityEngine;
using System;

public class RangeEnemy : Enemy
{
    [Header("Settings")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletTransform;

    [Header("Action")]
    public static Action<BulletData> OnBulletRequested;

    protected override void PerformAreaAttack()
    {

    }

    protected override void PerformSingleTargetAttack(GameObject target)
    {

        animator.Play("attack");
        OnBulletRequested?.Invoke(new BulletData
        {
            spawnPosition = bulletTransform.position,
            target = target,
            dataSO = enemySO,
            firePoint = bulletTransform
        });
    }
}
