using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;

public class BombTowerBullet : MonoBehaviour
{
    public TowerData TowerData;
    public GameObject target;
    [HideInInspector] public ObjectPool<GameObject> pool;

    private bool isReleased = false;
    private float speed = 4f;
    private float reachThreshold = 0.1f;

    [SerializeField] private float explosionRadius = 1.5f;
    [SerializeField] private LayerMask targetMask;

    public static Action<Vector2> onBombParticle;

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
            DealAreaDamage();
            ReleaseBullet();
        }
    }

    private void DealAreaDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, targetMask);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                if (damageable.GetTeam() == TeamType.Hero)
                {
                    damageable.TakeDamage(TowerData.damage);
                    onBombParticle?.Invoke(hit.gameObject.transform.position);

                }
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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
#endif
}
