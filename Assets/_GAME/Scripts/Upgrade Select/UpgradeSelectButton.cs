using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSelectButton : MonoBehaviour
{
    [SerializeField] private Image cardBackground;
    [SerializeField] private TextMeshProUGUI upgradeNameText;
    [SerializeField] private Image upgradeIconImage;
    [SerializeField] private TextMeshProUGUI upgradeDescText;
    public Button selectButton;
    
    

    public void Config(Sprite bg, Sprite icon, string name, string description)
    {
        cardBackground.sprite = bg;
        upgradeNameText.text=name;
        upgradeIconImage.sprite= icon;
        upgradeDescText.text= description;
    }
    public Button GetButton()
    {
        return selectButton;
    }
}
