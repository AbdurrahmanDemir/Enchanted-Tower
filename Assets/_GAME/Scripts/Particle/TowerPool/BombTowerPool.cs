using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BombTowerPool : MonoBehaviour
{
    [SerializeField] private GameObject bombTowerBullet;
    public ObjectPool<GameObject> bombTowerBulletPool;

    private void Awake()
    {
        BombTower.OnBulletRequested += SpawnBullet;
    }

    private void OnDestroy()
    {
        BombTower.OnBulletRequested -= SpawnBullet;
    }

    private void Start()
    {
        bombTowerBulletPool = new ObjectPool<GameObject>(
            CreateAngelBullet,
            OnGet,
            OnRelease,
            OnDestroy
        );
    }

    private GameObject CreateAngelBullet()
    {
        return Instantiate(bombTowerBullet);
    }

    private void OnGet(GameObject obj)
    {
        obj.SetActive(true);
    }

    private void OnRelease(GameObject obj)
    {
        obj.GetComponent<BombTowerBullet>().ResetBullet();
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
        GameObject bulletInstance = bombTowerBulletPool.Get();

        if (bulletInstance == null)
        {
            Debug.LogError("archerTowerBullet instance is null.");
            return;
        }

        if (bulletInstance.activeInHierarchy)
        {
            Debug.LogWarning("archerTowerBullet already active! Releasing and retrying.");
            bombTowerBulletPool.Release(bulletInstance);
            bulletInstance = bombTowerBulletPool.Get();
        }

        bulletInstance.transform.SetParent(data.firePoint);
        bulletInstance.transform.position = data.spawnPosition;

        var controller = bulletInstance.GetComponent<BombTowerBullet>();
        if (controller == null)
        {
            Debug.LogError("archerTowerBullet missing from prefab!");
            return;
        }

        controller.target = data.target;
        controller.target.transform.position = data.target.transform.position;
        controller.TowerData = data.dataSO as TowerData;
        controller.pool = bombTowerBulletPool;
    }
}
