using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FreePassRewardPrefabs : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI rewardAmountText;
    [SerializeField] private GameObject checkIcon;
    [SerializeField] private GameObject adIcon;
    [SerializeField] private Button claimButton;



    public void Config(Sprite type, Sprite icon, int amount, bool state)
    {
        backgroundImage.sprite = type;
        iconImage.sprite = icon;
        rewardAmountText.text = amount.ToString();
        if (state)
            checkIcon.SetActive(true);
        else
            checkIcon.SetActive(false);
    }
    public GameObject CheckIcon()
    {
        return checkIcon;
    }
    public GameObject AdIcon()
    {
        return adIcon;
    }

    public Button GetClaimButton()
    {
        return claimButton;
    }

}
