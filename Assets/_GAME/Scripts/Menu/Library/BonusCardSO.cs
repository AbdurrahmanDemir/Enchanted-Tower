using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Bonus Card", menuName = "Bonus")]

public class BonusCardSO : ScriptableObject
{
    public string cardName;
    public Sprite heroIcon;
    [TextArea] public string cardDescription;
}
