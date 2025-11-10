using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class IceGolemHero : Hero
{
    [Header("Settings")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletTransform;

    [Header("Action")]
    public static Action<BulletData> OnBulletRequested;
    public static Action<Vector2> onIceParticle;


    bool onDamage;
    protected override void PerformSingleTargetAttack(GameObject target)
    {
        OnBulletRequested?.Invoke(new BulletData
        {
            spawnPosition = bulletTransform.position,
            target = target,
            dataSO = heroSO,
            firePoint = bulletTransform
        });
        onIceParticle?.Invoke(target.transform.position);

        StartCoroutine(DamageOn(target));
    }
    IEnumerator DamageOn(GameObject enemy)
    {
        if (enemy == null) yield break; 

        var enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript == null) yield break;

        IceMageSO iceMageSO = heroSO as IceMageSO;

        float cooldown = enemyScript.enemySO.cooldown;
        float moveSpeed = enemyScript.enemySO.moveSpeed;

        yield return new WaitForSeconds(iceMageSO.freezeDuration);

        onDamage = false;

        if (enemy != null && enemyScript != null)
        {
            enemyScript.cooldown = cooldown;
            enemyScript.moveSpeed = moveSpeed;
            Debug.Log($"Enemy cooldown: {enemyScript.cooldown} move speed: {enemyScript.moveSpeed}");
        }
    }

    protected override void PerformAreaAttack()
    {
        // Alan hasarý uygulanmaz
    }
}
