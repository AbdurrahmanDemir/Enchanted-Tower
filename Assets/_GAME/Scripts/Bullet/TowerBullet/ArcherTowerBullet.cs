using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ArcherTowerBullet : MonoBehaviour
{
    public TowerData TowerData;
    public GameObject target;
    [HideInInspector] public ObjectPool<GameObject> pool;

    private bool isReleased = false;
    private float speed = 4f;
    private float reachThreshold = 0.1f;

    private void Update()
    {
        if (isReleased || target == null)
        {
            ReleaseBullet();
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        float distance = Vector2.Distance(transform.position, target.transform.position);
        if (distance <= reachThreshold)
        {
            DealDamage();
            ReleaseBullet();
        }
    }

    private void DealDamage()
    {
        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            if (damageable.GetTeam() == TeamType.Enemy)
            {
                damageable.TakeDamage(TowerData.damage);
            }
        }
    }

    private void ReleaseBullet()
    {
        if (isReleased) return;
        isReleased = true;
        pool?.Release(gameObject);
    }

    public void ResetBullet()
    {
        isReleased = false;
        target = null;
        transform.position = Vector3.zero;
    }
}
