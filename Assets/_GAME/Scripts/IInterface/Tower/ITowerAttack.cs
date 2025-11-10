using UnityEngine;

public interface ITowerAttack
{
    void Attack(GameObject target, Transform firePoint, int damage);
}
