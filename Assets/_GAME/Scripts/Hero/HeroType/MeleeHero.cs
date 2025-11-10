using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MeleeHero : Hero
{
    protected override void PerformSingleTargetAttack(GameObject target)
    {
        Debug.Log($"{gameObject.name} is attacking {target.name} with single target attack for {heroSO.GetCurrentDamage()} damage!");
        if (target.CompareTag("Enemy"))
        {
            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                if (damageable.GetTeam() == TeamType.Enemy)
                    damageable.TakeDamage(heroSO.GetCurrentDamage());
            }
        }
        else if (target.CompareTag("EnemyTower"))
        {
            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                if (damageable.GetTeam() == TeamType.Enemy)
                    damageable.TakeDamage(heroSO.GetCurrentDamage());
            }
        }
            //target.GetComponent<EnemyTowerController>().TakeDamage(heroSO.GetCurrentDamage());
    }

    protected override void PerformAreaAttack()
    {
        // Alan hasarý uygulanmaz
    }
}
