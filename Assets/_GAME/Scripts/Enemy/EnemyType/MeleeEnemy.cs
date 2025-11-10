using UnityEngine;

public class MeleeEnemy : Enemy
{    
    protected override void PerformSingleTargetAttack(GameObject target)
    {
        Debug.Log($"{gameObject.name} is attacking {target.name} with single target attack for {enemySO.damage} damage!");

        animator.Play("attack");
        if (target.CompareTag("Hero"))
        {
            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                if (damageable.GetTeam() == TeamType.Hero)
                    damageable.TakeDamage(enemySO.damage);
            }

        }
        else if (target.CompareTag("Tower"))
        {
            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                if (damageable.GetTeam() == TeamType.Hero)
                    damageable.TakeDamage(enemySO.damage);
            }
        }
    }

    protected override void PerformAreaAttack()
    {
        // Alan hasarý uygulanmaz
    }
}
