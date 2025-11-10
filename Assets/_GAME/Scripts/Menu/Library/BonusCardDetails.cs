using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusCardDetails : MonoBehaviour
{
    [Header("Card")]
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private Image cardIconImage;
    [SerializeField] private TextMeshProUGUI cardText;
  

    public void Config(string name, Sprite icon,string cardDetails)
    {
        cardNameText.text = name;
        cardIconImage.sprite = icon;
        cardText.text = cardDetails;
    
    
    }

    public void TogglePanel()
    {
        if (gameObject.activeSelf)
        {
            gameObject.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
            DOTween.Kill(gameObject);
            Destroy(gameObject, 5f);
        }
        else
        {
            gameObject.SetActive(true);
            gameObject.transform.localScale = Vector3.zero;
            gameObject.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }
    }

}
