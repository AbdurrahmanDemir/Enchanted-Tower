using UnityEngine;

public class SimpleProjectileAttack : ITowerAttack
{
    private GameObject projectilePrefab;

    public SimpleProjectileAttack(GameObject projectile)
    {
        projectilePrefab = projectile;
    }

    public void Attack(GameObject target, Transform firePoint, int damage)
    {
        GameObject proj = GameObject.Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
    }
}
