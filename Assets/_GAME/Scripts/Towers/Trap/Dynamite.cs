using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamite : MonoBehaviour
{
    public static Action<Vector2> onBombParticle;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hero"))
        {
            if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
            {
                if (damageable.GetTeam() == TeamType.Hero)
                    damageable.TakeDamage(500);

                onBombParticle?.Invoke(transform.position);

                Destroy(gameObject);

            }
        }
    }

}
