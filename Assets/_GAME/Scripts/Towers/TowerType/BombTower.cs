using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTower : Tower
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
