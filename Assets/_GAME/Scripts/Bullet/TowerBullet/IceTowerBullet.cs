using System;
using UnityEngine;
using UnityEngine.Pool;

public class IceTowerBullet : MonoBehaviour
{
    public TowerData TowerData;
    public GameObject target;
    [HideInInspector] public ObjectPool<GameObject> pool;

    private bool isReleased = false;
    private float speed = 5f;
    private float reachThreshold = 0.1f;

    [SerializeField] private float freezeDuration = 2f;

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
            DealDamageAndFreeze();
            ReleaseBullet();
        }
    }

    private void DealDamageAndFreeze()
    {
        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            if (damageable.GetTeam() == TeamType.Hero)
            {
                damageable.TakeDamage(TowerData.damage);

                if (target.TryGetComponent<Hero>(out var enemy))
                {
                    enemy.Freeze(freezeDuration);
                    onBombParticle?.Invoke(enemy.gameObject.transform.position);

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
}
