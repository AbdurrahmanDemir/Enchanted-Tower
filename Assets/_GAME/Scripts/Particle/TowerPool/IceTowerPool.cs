using UnityEngine;
using UnityEngine.Pool;

public class IceTowerPool : MonoBehaviour
{
    [SerializeField] private GameObject iceMageBulletPrefab;
    public ObjectPool<GameObject> iceMageBulletPool;

    private void Awake()
    {
        IceTower.OnBulletRequested += SpawnBullet;
    }

    private void OnDestroy()
    {
        IceTower.OnBulletRequested -= SpawnBullet;
    }

    private void Start()
    {
        iceMageBulletPool = new ObjectPool<GameObject>(
            CreateBullet,
            OnGet,
            OnRelease,
            OnDestroyBullet
        );
    }

    private GameObject CreateBullet()
    {
        return Instantiate(iceMageBulletPrefab);
    }

    private void OnGet(GameObject obj)
    {
        obj.SetActive(true);
    }

    private void OnRelease(GameObject obj)
    {
        obj.GetComponent<IceTowerBullet>().ResetBullet();
        obj.transform.SetParent(null);
        obj.transform.position = Vector3.zero;
        obj.SetActive(false);
    }

    private void OnDestroyBullet(GameObject obj)
    {
        Destroy(obj);
    }

    private void SpawnBullet(BulletData data)
    {
        GameObject bulletInstance = iceMageBulletPool.Get();

        if (bulletInstance.activeInHierarchy)
        {
            iceMageBulletPool.Release(bulletInstance);
            bulletInstance = iceMageBulletPool.Get();
        }

        bulletInstance.transform.SetParent(data.firePoint);
        bulletInstance.transform.position = data.spawnPosition;

        var controller = bulletInstance.GetComponent<IceTowerBullet>();
        controller.target = data.target;
        controller.TowerData = data.dataSO as TowerData;
        controller.pool = iceMageBulletPool;
    }
}
