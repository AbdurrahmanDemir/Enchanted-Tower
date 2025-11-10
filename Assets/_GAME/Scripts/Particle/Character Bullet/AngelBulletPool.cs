using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AngelBulletPool : MonoBehaviour
{
    [SerializeField] private GameObject angelBulletPrefab;
    public ObjectPool<GameObject> angelBulletPool;

    private void Awake()
    {
        RangeHero.OnBulletRequested += SpawnBullet;
    }

    private void OnDestroy()
    {
        RangeHero.OnBulletRequested -= SpawnBullet;
    }

    private void Start()
    {
        angelBulletPool = new ObjectPool<GameObject>(
            CreateAngelBullet,
            OnGet,
            OnRelease,
            OnDestroy
        );
    }

    private GameObject CreateAngelBullet()
    {
        return Instantiate(angelBulletPrefab);
    }

    private void OnGet(GameObject obj)
    {
        obj.SetActive(true);
    }

    private void OnRelease(GameObject obj)
    {
        obj.GetComponent<AngelBulletController>().ResetBullet();
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
        GameObject bulletInstance = angelBulletPool.Get();

        if (bulletInstance == null)
        {
            Debug.LogError("Angel bullet instance is null.");
            return;
        }

        if (bulletInstance.activeInHierarchy)
        {
            Debug.LogWarning("Angel bullet already active! Releasing and retrying.");
            angelBulletPool.Release(bulletInstance);
            bulletInstance = angelBulletPool.Get();
        }

        bulletInstance.transform.SetParent(data.firePoint);
        bulletInstance.transform.position = data.spawnPosition;

        var controller = bulletInstance.GetComponent<AngelBulletController>();
        if (controller == null)
        {
            Debug.LogError("AngelBulletController missing from prefab!");
            return;
        }

        controller.target = data.target;
        controller.targetPosition = data.target.transform.position;
        controller.heroSO = data.dataSO as HeroSO;
        controller.pool = angelBulletPool;
    }

}
