using System.Collections.Generic;
using UnityEngine;

public interface ITowerTargetSelector
{
    GameObject SelectTarget(List<GameObject> potentialTargets);
}
