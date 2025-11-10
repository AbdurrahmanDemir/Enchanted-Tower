using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmoredKnightHouse : Building
{
    [Header("Barbar Settings")]
    public GameObject heroPrefab; 
    public Transform spawnPoint;

    public WorkerController workerController;

    private void Start()
    {
        base.Start();

        if (workerController.TryPurchaseWorker(0))
            StartProduction();
    }

    protected override void OnProductionComplete()
    {
        if (heroPrefab != null && spawnPoint != null)
        {
            Instantiate(heroPrefab, spawnPoint.position, Quaternion.identity);
        }
        if (productionSlider != null)
            productionSlider.value = 0;

        if (workerController.TryPurchaseWorker(1))
            StartProduction();
    }
 

}
