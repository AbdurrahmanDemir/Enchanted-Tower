using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ArcherTowerPool : MonoBehaviour
{
    [SerializeField] private GameObject archerTowerBullet;
    public ObjectPool<GameObject> archerTowerBulletPool;

    private void Awake()
    {
        ArcherTower.OnBulletRequested += SpawnBullet;
    }

    private void OnDestroy()
    {
        ArcherTower.OnBulletRequested -= SpawnBullet;
    }

    private void Start()
    {
        archerTowerBulletPool = new ObjectPool<GameObject>(
            CreateAngelBullet,
            OnGet,
            OnRelease,
            OnDestroy
        );
    }

    private GameObject CreateAngelBullet()
    {
        return Instantiate(archerTowerBullet);
    }

    private void OnGet(GameObject obj)
    {
        obj.SetActive(true);
    }

    private void OnRelease(GameObject obj)
    {
        obj.GetComponent<ArcherTowerBullet>().ResetBullet();
        obj.transform.SetParent(null);
        obj.transform.position = Vector3.zero;
        obj.SetActive(false);
    }

    private void OnDestroy(GameObject obj)
    {
        Destroy(obj);
    }

    private void SpawnBullet(BulletData data)
    {
        GameObject bulletInstance = archerTowerBulletPool.Get();

        if (bulletInstance == null)
        {
            Debug.LogError("archerTowerBullet instance is null.");
            return;
        }

        if (bulletInstance.activeInHierarchy)
        {
            Debug.LogWarning("archerTowerBullet already active! Releasing and retrying.");
            archerTowerBulletPool.Release(bulletInstance);
            bulletInstance = archerTowerBulletPool.Get();
        }

        bulletInstance.transform.SetParent(data.firePoint);
        bulletInstance.transform.position = data.spawnPosition;

        var controller = bulletInstance.GetComponent<ArcherTowerBullet>();
        if (controller == null)
        {
            Debug.LogError("archerTowerBullet missing from prefab!");
            return;
        }

        controller.target = data.target;
        controller.target.transform.position = data.target.transform.position;
        controller.TowerData = data.dataSO as TowerData;
        controller.pool = archerTowerBulletPool;
    }
}
