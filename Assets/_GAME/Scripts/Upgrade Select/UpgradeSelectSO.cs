using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Select", menuName = "New Item")]

public class UpgradeSelectSO : ScriptableObject
{
    [Header("Upgrade Info")]
    public UpgradeType upgradeType;
    public Sprite upgradeIcon;
    public Sprite upgradeBg;
    public string upgradeName;
    [TextArea] public string upgradeDescription;
    public int amount;
}

public enum UpgradeType
{
    AddGold=0,
    AddCapacity=1,
    AddUpgradeToken=2,
    DamageUpgrade=5,
    HealthUpgrade=6
}
