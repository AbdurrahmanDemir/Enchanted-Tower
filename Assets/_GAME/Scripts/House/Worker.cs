using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Worker : MonoBehaviour
{
    public static Worker instance;

    [Header("Elements")]
    [SerializeField] private int idleWorkerCount;
    [SerializeField] private TextMeshProUGUI idleWorkerCountText;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        AddIdleWorker(5);
    }
    public void AddIdleWorker(int value)
    {
        idleWorkerCount += value;
        UpdateWorkerText();

    }
    public bool TryPurchaseIdleWorker(int price)
    {
        if (price <= idleWorkerCount)
        {
            idleWorkerCount -= price;
            UpdateWorkerText();
            return true;
        }
        else
        {
            PopUpController.instance.OpenPopUp("NOT ENOUGH WORKER");
        }
        return false;
    }

    private void UpdateWorkerText()
    {
        idleWorkerCountText.text = idleWorkerCount.ToString();
    }
}
