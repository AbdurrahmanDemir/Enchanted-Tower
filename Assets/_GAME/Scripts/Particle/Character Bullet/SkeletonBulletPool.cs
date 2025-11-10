using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SkeletonBulletPool : MonoBehaviour
{
    [SerializeField] private GameObject skeletonBulletPrefab;
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxSize = 50;

    public ObjectPool<GameObject> skeletonBulletPool;
    private HashSet<GameObject> activeBullets = new HashSet<GameObject>();

    private void Awake()
    {
        RangeEnemy.OnBulletRequested += SpawnBullet;
    }

    private void OnDestroy()
    {
        RangeEnemy.OnBulletRequested -= SpawnBullet;

        foreach (var bullet in activeBullets)
        {
            if (bullet != null)
            {
                Destroy(bullet);
            }
        }
        activeBullets.Clear();
    }

    private void Start()
    {
        skeletonBulletPool = new ObjectPool<GameObject>(
            CreateBullet,
            OnGet,
            OnRelease,
            OnDestroy,
            true, 
            defaultCapacity,
            maxSize
        );
    }

    private GameObject CreateBullet()
    {
        GameObject newBullet = Instantiate(skeletonBulletPrefab);

        var controller = newBullet.GetComponent<SkeletonBulletController>();
        if (controller != null)
        {
            controller.pool = skeletonBulletPool;
        }

        return newBullet;
    }

    private void OnGet(GameObject obj)
    {
        if (obj == null) return;

        activeBullets.Add(obj);

        obj.SetActive(true);

        var controller = obj.GetComponent<SkeletonBulletController>();
        if (controller != null)
        {
            controller.ResetBullet();
        }
    }

    private void OnRelease(GameObject obj)
    {
        if (obj == null) return;

        activeBullets.Remove(obj);

        var controller = obj.GetComponent<SkeletonBulletController>();
        if (controller != null)
        {
            controller.ResetBullet();
        }

        obj.transform.SetParent(null);
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;

        obj.SetActive(false);
    }

    private void OnDestroy(GameObject obj)
    {
        if (obj != null)
        {
            activeBullets.Remove(obj);
            Destroy(obj);
        }
    }

    private void SpawnBullet(BulletData data)
    {
        if (data.target == null)
        {
            Debug.LogWarning("Bullet spawn target is null!");
            return;
        }

        if (skeletonBulletPool == null)
        {
            Debug.LogError("Skeleton bullet pool is not initialized!");
            return;
        }

        GameObject bulletInstance = null;

        try
        {
            bulletInstance = skeletonBulletPool.Get();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error getting bullet from pool: {e.Message}");
            return;
        }

        if (bulletInstance == null)
        {
            Debug.LogError("Failed to get bullet instance from pool!");
            return;
        }

        bulletInstance.transform.SetParent(null); 
        bulletInstance.transform.position = data.spawnPosition;
        bulletInstance.transform.rotation = Quaternion.identity;

        var controller = bulletInstance.GetComponent<SkeletonBulletController>();
        if (controller == null)
        {
            Debug.LogError("SkeletonBulletController missing from prefab!");
            skeletonBulletPool.Release(bulletInstance);
            return;
        }

        controller.target = data.target;
        controller.targetPosition = data.target.transform.position;
        controller.enemySO = data.dataSO as EnemySO;
        controller.pool = skeletonBulletPool;
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Update()
    {
        if (Application.isEditor)
        {
            int nullCount = 0;
            foreach (var bullet in activeBullets)
            {
                if (bullet == null)
                    nullCount++;
            }

            if (nullCount > 0)
            {
                Debug.LogWarning($"Active bullets list contains {nullCount} null references!");
            }
        }
    }
}
