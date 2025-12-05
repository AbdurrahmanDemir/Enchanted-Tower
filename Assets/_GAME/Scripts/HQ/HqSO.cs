using UnityEngine;

[CreateAssetMenu(fileName = "Tower", menuName = "Creat Tower")]
public class HqSO : ScriptableObject
{
    public int maxHealth;
    public int baseUpgradeCost = 1;

    public int GetCurrentHealth()
    {
        return PlayerPrefs.GetInt($"Tower_Health", maxHealth);
    }
    public int GetUpgradeCost()
    {
        return PlayerPrefs.GetInt($"Tower_UpgradeCost", baseUpgradeCost);
    }


    public void UpgradeDamage()
    {
        int upgradeCost = GetUpgradeCost();
        int upgradeHealth = GetCurrentHealth();

        int newUpgradeCost = upgradeCost * 2;
        int newUpgradeHealth = upgradeHealth + 100;

        PlayerPrefs.SetInt($"Tower_Health", newUpgradeHealth);
        PlayerPrefs.SetInt($"Tower_UpgradeCost", newUpgradeCost);
        PlayerPrefs.Save();
    }
}
