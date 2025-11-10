using System;
using UnityEngine;

public class RangeHero : Hero
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
        OnBulletRequested?.Invoke(new BulletData
        {
            spawnPosition = bulletTransform.position,
            target = target,
            dataSO = heroSO,
            firePoint = bulletTransform
        });
    }
}
