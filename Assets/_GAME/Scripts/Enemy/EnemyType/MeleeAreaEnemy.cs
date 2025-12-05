using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MeleeAreaEnemy : Enemy
{
    protected override void PerformSingleTargetAttack(GameObject target)
    {
        
    }

    protected override void PerformAreaAttack()
    {
        Collider2D[] targetsInRange = Physics2D.OverlapCircleAll(transform.position, enemySO.range, targetLayerMask);
        foreach (var targetx in targetsInRange)
        {
            Debug.Log($"{gameObject.name} is attacking {targetx.gameObject.name} with area of effect attack for {enemySO.damage} damage!");
            animator.Play("attack");

            if (targetx.CompareTag("Hero"))
            {
                if (targetx.TryGetComponent<IDamageable>(out var damageable))
                {
                    if (damageable.GetTeam() == TeamType.Hero)
                        damageable.TakeDamage(enemySO.damage);
                }

            }
            else if (targetx.CompareTag("Tower"))
            {
                if (targetx.TryGetComponent<IDamageable>(out var damageable))
                {
                    if (damageable.GetTeam() == TeamType.Building)
                        damageable.TakeDamage(enemySO.damage);
                }

            }
        }

    }
}
