using System.Collections.Generic;
using UnityEngine;

public class HighestHealthTargetSelector : ITowerTargetSelector
{
    public GameObject SelectTarget(List<GameObject> potentialTargets)
    {
        GameObject strongest = null;
        int maxHealth = -1;

        foreach (var t in potentialTargets)
        {
            if (t.TryGetComponent<IDamageable>(out var damageable))
            {
                int hp = damageable.GetCurrentHealth();
                if (hp > maxHealth)
                {
                    maxHealth = hp;
                    strongest = t;
                }
            }
        }

        return strongest;
    }
}
