using System.Collections.Generic;
using UnityEngine;

public class ClosestTargetSelector : ITowerTargetSelector
{
    public GameObject SelectTarget(List<GameObject> potentialTargets)
    {
        GameObject closest = null;
        float minDist = Mathf.Infinity;

        foreach (var t in potentialTargets)
        {
            float dist = Vector2.Distance(t.transform.position, Vector2.zero); 
            if (dist < minDist)
            {
                minDist = dist;
                closest = t;
            }
        }
        return closest;
    }
}
