using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class IceTowerParticle : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private GameObject iceTowerParticlePrefab;

    [Header("Pooling")]
    private ObjectPool<GameObject> iceParticlePool;

    private void Awake()
    {
        IceTowerBullet.onBombParticle += IceParticleCallBack;
    }
    private void OnDestroy()
    {
        IceTowerBullet.onBombParticle -= IceParticleCallBack;
    }


    private void Start()
    {
        iceParticlePool = new ObjectPool<GameObject>(CreateFunction,
                                                      ActionOnGet,
                                                      ActionOnRelease,
                                                      ActionOnDestroy);
    }

    private GameObject CreateFunction()
    {
        return Instantiate(iceTowerParticlePrefab) as GameObject;
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

    private void IceParticleCallBack(Vector2 createPosition)
    {
        GameObject bloodPaticleInstance = iceParticlePool.Get();

        bloodPaticleInstance.transform.position = createPosition;

        DOTween.Sequence()
            .AppendInterval(3)
            .AppendCallback(() => iceParticlePool.Release(bloodPaticleInstance));
    }
}
