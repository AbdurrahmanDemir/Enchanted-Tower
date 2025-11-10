using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArcherTower : Tower
{
    [Header("Settings")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletTransform;

    [Header("Action")]
    public static Action<BulletData> OnBulletRequested;

    protected override void Attack(GameObject target)
    {
        OnBulletRequested?.Invoke(new BulletData
        {
            spawnPosition = bulletTransform.position,
            target = target,
            dataSO = towerData,
            firePoint = bulletTransform
        });
    }
}
