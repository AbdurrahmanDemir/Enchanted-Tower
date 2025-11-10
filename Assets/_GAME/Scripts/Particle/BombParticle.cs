using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class BombParticle : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private GameObject bombParticlePrefabs;

    [Header("Pooling")]
    private ObjectPool<GameObject> bombParticlePool;

    private void Awake()
    {
        BombTowerBullet.onBombParticle += BloodParticleCallBack;
        Dynamite.onBombParticle += BloodParticleCallBack;
    }
    private void OnDestroy()
    {
        BombTowerBullet.onBombParticle -= BloodParticleCallBack;
        Dynamite.onBombParticle -= BloodParticleCallBack;

    }


    private void Start()
    {
        bombParticlePool = new ObjectPool<GameObject>(CreateFunction,
                                                      ActionOnGet,
                                                      ActionOnRelease,
                                                      ActionOnDestroy);
    }

    private GameObject CreateFunction()
    {
        return Instantiate(bombParticlePrefabs) as GameObject;
    }
    private void ActionOnGet(GameObject particle)
    {
        particle.SetActive(true);
    }
    private void ActionOnRelease(GameObject particle)
    {
        particle.SetActive(false);
    }
    private void ActionOnDestroy(GameObject particle)
    {
        Destroy(particle);
    }

    private void BloodParticleCallBack(Vector2 createPosition)
    {
        GameObject bloodPaticleInstance = bombParticlePool.Get();

        bloodPaticleInstance.transform.position = createPosition;

        DOTween.Sequence()
            .AppendInterval(3)
            .AppendCallback(() => bombParticlePool.Release(bloodPaticleInstance));
    }
}
