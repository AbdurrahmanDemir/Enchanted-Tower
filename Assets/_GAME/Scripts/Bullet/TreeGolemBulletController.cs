using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class TreeGolemBulletController : MonoBehaviour
{
    public HeroSO heroSO;
    public Vector2 targetPosition;
    public GameObject target;

    [HideInInspector] public ObjectPool<GameObject> pool;
    private bool isReleased = false;
    private float moveSpeed = 4f;

    private void Start()
    {
        DOTween.Sequence()
            .AppendInterval(1f)
            .AppendCallback(() => ReleaseBullet())
            .SetTarget(this);
    }

    private void Update()
    {
        if (isReleased)
            return;

        if (target == null || IsTargetFullHealth())
        {
            ReleaseBullet();
            return;
        }

        MoveTowardsTarget(targetPosition);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReleased) return;

        if (collision.CompareTag("Hero") && collision.gameObject == target)
        {
            if (collision.TryGetComponent<Hero>(out var hero))
            {
                if (!hero.IsFullHealth())
                {
                    hero.health += 50; 
                    hero.health = Mathf.Min(hero.health, hero.heroSO.maxHealth); 
                }
            }

            ReleaseBullet();
        }
    }

    private void ReleaseBullet()
    {
        if (isReleased) return;
        isReleased = true;

        DOTween.Kill(this);

        pool?.Release(gameObject);
    }

    public void ResetBullet()
    {
        isReleased = false;
        target = null;
        targetPosition = Vector2.zero;
        transform.position = Vector3.zero;
    }

    private void MoveTowardsTarget(Vector2 pos)
    {
        transform.position = Vector2.MoveTowards(transform.position, pos, moveSpeed * Time.deltaTime);
    }

    private bool IsTargetFullHealth()
    {
        if (target != null && target.TryGetComponent<Hero>(out var hero))
        {
            return hero.IsFullHealth();
        }
        return true;
    }
}
