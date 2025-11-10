using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TreeGolemBulletPool : MonoBehaviour
{
    [SerializeField] private GameObject treeGolemBulletPrefab;
    public ObjectPool<GameObject> treeGolemBulletPool;

    private void Awake()
    {
        TreeGolem.OnBulletRequested += SpawnBullet;
    }

    private void OnDestroy()
    {
        TreeGolem.OnBulletRequested -= SpawnBullet;
    }

    private void Start()
    {
        treeGolemBulletPool = new ObjectPool<GameObject>(
            CreateAngelBullet,
            OnGet,
            OnRelease,
            OnDestroy
        );
    }

    private GameObject CreateAngelBullet()
    {
        return Instantiate(treeGolemBulletPrefab);
    }

    private void OnGet(GameObject obj)
    {
        obj.SetActive(true);
    }

    private void OnRelease(GameObject obj)
    {
        obj.GetComponent<TreeGolemBulletController>().ResetBullet();
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
        GameObject bulletInstance = treeGolemBulletPool.Get();

        if (bulletInstance == null)
        {
            Debug.LogError("TreeGolem bullet instance is null.");
            return;
        }

        if (bulletInstance.activeInHierarchy)
        {
            Debug.LogWarning("TreeGolem bullet already active! Releasing and retrying.");
            treeGolemBulletPool.Release(bulletInstance);
            bulletInstance = treeGolemBulletPool.Get();
        }

        bulletInstance.transform.SetParent(data.firePoint);
        bulletInstance.transform.position = data.spawnPosition;

        var controller = bulletInstance.GetComponent<TreeGolemBulletController>();
        if (controller == null)
        {
            Debug.LogError("TreeGolem missing from prefab!");
            return;
        }

        controller.target = data.target;
        controller.targetPosition = data.target.transform.position;
        controller.heroSO = data.dataSO as HeroSO;
        controller.pool = treeGolemBulletPool;
    }

}
