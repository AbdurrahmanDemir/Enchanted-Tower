using System.Collections.Generic;
using UnityEngine;

public class RandomTargetSelector : ITowerTargetSelector
{
    public GameObject SelectTarget(List<GameObject> potentialTargets)
    {
        if (potentialTargets.Count == 0) return null;
        int index = Random.Range(0, potentialTargets.Count);
        return potentialTargets[index];
    }
}
