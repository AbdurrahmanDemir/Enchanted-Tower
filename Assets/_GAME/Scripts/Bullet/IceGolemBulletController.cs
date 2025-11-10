using System;
using UnityEngine;
using UnityEngine.Pool;

public class IceGolemBulletController : MonoBehaviour
{
    public HeroSO heroSO;
    public GameObject target;

    [HideInInspector] public ObjectPool<GameObject> pool;

    private bool isReleased = false;
    private float speed = 5f;
    private float reachThreshold = 0.1f;

    [SerializeField] private float slowDuration = 2f;    
    [SerializeField] private float slowMultiplier = 0.2f; 


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
            DealDamageAndSlow();
            ReleaseBullet();
        }
    }

    private void DealDamageAndSlow()
    {
        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            if (damageable.GetTeam() == TeamType.Enemy)
            {
                damageable.TakeDamage(heroSO.GetCurrentDamage());

                if (target.TryGetComponent<Enemy>(out var enemy))
                {
                    FreezeEnemy(enemy);
                }
            }
        }
    }

    private void FreezeEnemy(Enemy enemy)
    {
        if (enemy == null) return;

        enemy.moveSpeed = 0f;
        enemy.cooldown = enemy.enemySO.cooldown + 2f;

        if (enemy.TryGetComponent<UnityEngine.AI.NavMeshAgent>(out var agent))
        {
            agent.speed = 0f;
        }

        enemy.Invoke(nameof(enemy.ResetStats), slowDuration);
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
