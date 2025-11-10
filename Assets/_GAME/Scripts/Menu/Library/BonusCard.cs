using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private Image cardIconImage;
    public Button detailsButton;
    public int cardIndex;


    public void Config(string name, Sprite icon)
    {
        cardNameText.text = name;
        cardIconImage.sprite = icon;        
    }
}
