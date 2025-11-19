using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [Header(" Data ")]
    [SerializeField] private int gold;
    [SerializeField] private int xp;
    [SerializeField] private int energy;
    [SerializeField] private int heroUpgradeToken;
    [Header(" Text ")]
    [SerializeField] private TextMeshProUGUI[] GoldText;
    [SerializeField] private TextMeshProUGUI[] XpText;
    [SerializeField] private TextMeshProUGUI[] EnergyText;
    [SerializeField] private TextMeshProUGUI[] HeroUpgradeTokenText;

    private GameObject popUp;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        LoadData();

    }


    public bool TryPurchaseGold(int price)
    {
        if (price <= gold)
        {
            gold -= price;
            SaveData();
            UpdateGoldText();
            return true;
        }
        else
        {
            PopUpController.instance.OpenPopUp("NOT ENOUGH GOLD");

        }
        return false;
    }

    public void AddGold(int value)
    {
        gold += value;
        UpdateGoldText();
        SaveData();
    }

    public void AddXP(int value)
    {
        xp += value;
        UpdateXPText();
        SaveData();
    }

    private void UpdateGoldText()
    {
        for (int i = 0; i < GoldText.Length; i++)
        {
            GoldText[i].text= gold.ToString();
        }
    }

    private void UpdateXPText()
    {
        for (int i = 0; i < XpText.Length; i++)
        {
            XpText[i].text = xp.ToString();
        }

    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("Gold", gold);
        PlayerPrefs.SetInt("XP", xp);
        PlayerPrefs.SetInt("Energy", energy);
        PlayerPrefs.SetInt("HeroUpgradeToken", heroUpgradeToken);
    }

    private void LoadData()
    {
        if (PlayerPrefs.HasKey("Gold"))
        {
            gold = PlayerPrefs.GetInt("Gold");
            xp = PlayerPrefs.GetInt("XP", 0);
            energy = PlayerPrefs.GetInt("Energy", energy);
            heroUpgradeToken = PlayerPrefs.GetInt("HeroUpgradeToken", heroUpgradeToken);
        }
        else
        {
            AddGold(200);
            AddEnergy(5);
            AddHeroToken(100);
        }
        

        Debug.Log("GOLD" + gold + "XP" + xp);

        SaveData();
        UpdateGoldText();
        UpdateXPText();
        UpdateEnergyText();
        UpdateHeroTokenText();
    }

    // Energy

    public bool TryPurchaseEnergy(int price)
    {
        if (price <= energy)
        {
            energy -= price;
            SaveData();
            UpdateEnergyText();
            return true;
        }
        else
        {
            PopUpController.instance.OpenPopUp("NOT ENOUGH ENERGY");
        }
        return false;
    }

    private void UpdateEnergyText()
    {
        for (int i = 0; i < EnergyText.Length; i++)
        {
            EnergyText[i].text = energy.ToString();
        }
    }

    public void AddEnergy(int value)
    {
        energy += value;
        UpdateEnergyText();
        SaveData();
    }

    //Hero Token

    public bool TryPurchaseHeroUpgradeToken(int price)
    {
        if (price <= heroUpgradeToken)
        {
            heroUpgradeToken -= price;
            SaveData();
            UpdateHeroTokenText();
            return true;
        }
        else
        {
            PopUpController.instance.OpenPopUp("NOT ENOUGH HERO UPGRADE TOKEN");
        }
        return false;
    }
    private void UpdateHeroTokenText()
    {
        for (int i = 0; i < HeroUpgradeTokenText.Length; i++)
        {
            HeroUpgradeTokenText[i].text = heroUpgradeToken.ToString();
        }
    }
    public void AddHeroToken(int value)
    {
        heroUpgradeToken += value;
        UpdateHeroTokenText();
        SaveData();
    }
    public int GetGoldCount()
    {
        return gold;
    }
}
