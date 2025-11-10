using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MeleeAreaHero : Hero
{
    protected override void PerformSingleTargetAttack(GameObject target)
    {
    }

    protected override void PerformAreaAttack()
    {

        Collider2D[] targetsInRange = Physics2D.OverlapCircleAll(transform.position, heroSO.range, targetLayerMask);
        foreach (var targetx in targetsInRange)
        {
            Debug.Log($"{gameObject.name} is attacking {targetx.gameObject.name} with area of effect attack for {heroSO.GetCurrentDamage()} damage!");

            if (targetx.CompareTag("Enemy"))
            {
                if (targetx.TryGetComponent<IDamageable>(out var damageable))
                {
                    if (damageable.GetTeam() == TeamType.Enemy)
                        damageable.TakeDamage(heroSO.GetCurrentDamage());
                }
            }
            else if (targetx.CompareTag("EnemyTower"))
            {
                if (targetx.TryGetComponent<IDamageable>(out var damageable))
                {
                    if (damageable.GetTeam() == TeamType.Enemy)
                        damageable.TakeDamage(heroSO.GetCurrentDamage());
                }
            }
                //targetx.GetComponent<EnemyTowerController>().TakeDamage(heroSO.GetCurrentDamage());
        }
    }
}
