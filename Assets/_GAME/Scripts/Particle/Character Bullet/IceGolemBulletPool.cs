using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class IceGolemBulletPool : MonoBehaviour
{
    [SerializeField] private GameObject iceGolemBulletPrefab;
    public ObjectPool<GameObject> iceGolemBulletPool;

    private void Awake()
    {
        IceGolemHero.OnBulletRequested += SpawnBullet;
    }

    private void OnDestroy()
    {
        IceGolemHero.OnBulletRequested -= SpawnBullet;
    }

    private void Start()
    {
        iceGolemBulletPool = new ObjectPool<GameObject>(
            CreateBullet,
            OnGet,
            OnRelease,
            OnDestroy
        );
    }

    private GameObject CreateBullet()
    {
        return Instantiate(iceGolemBulletPrefab);
    }

    private void OnGet(GameObject obj)
    {
        obj.SetActive(true);
    }

    private void OnRelease(GameObject obj)
    {
        obj.GetComponent<IceGolemBulletController>().ResetBullet();
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
        GameObject bullet = iceGolemBulletPool.Get();

        bullet.transform.SetParent(data.firePoint);
        bullet.transform.position = data.spawnPosition;

        var controller = bullet.GetComponent<IceGolemBulletController>();
        controller.target = data.target;
        controller.target.transform.position = data.target.transform.position;
        controller.heroSO = data.dataSO as HeroSO;
        controller.pool = iceGolemBulletPool;

    }
}
