using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestContainer : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Button claimButton;

    private int key;

    [Header("Actions")]
    public static Action<int> onRewardClaimed;

    public void Configure(string title, string rewardString, float progress, int key)
    {
        this.key = key;
        titleText.text = title;
        coinText.text = rewardString;
        UpdateProgress(progress);
    }

    public void UpdateProgress(float value)
    {
        progressBar.value = Mathf.Clamp01(value);
        CheckIfCanClaim(value);
    }

    private void CheckIfCanClaim(float progress)
    {
        bool isComplete = progress >= 0.999f; 

        claimButton.gameObject.SetActive(isComplete);
        progressBar.gameObject.SetActive(!isComplete);
    }

    public void Claim()
    {
        onRewardClaimed?.Invoke(key);
    }

    public int GetKey()
    {
        return key;
    }
}
