using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class SkeletonBulletController : MonoBehaviour
{
    [Header("Bullet Settings")]
    public EnemySO enemySO;
    public GameObject target;
    public Vector2 targetPosition;
    public float moveSpeed = 4f;
    [HideInInspector] public ObjectPool<GameObject> pool;

    private bool isReleased = false;
    private bool isInitialized = false;
    private Tween autoReleaseTween;
    private Collider2D bulletCollider;

    private void Awake()
    {
        bulletCollider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        isReleased = false;
        isInitialized = false;

        if (bulletCollider != null)
            bulletCollider.enabled = true;

        StartAutoReleaseTween();
    }

    private void OnDisable()
    {
        autoReleaseTween?.Kill();
        autoReleaseTween = null;

        if (bulletCollider != null)
            bulletCollider.enabled = false;
    }

    private void Update()
    {
        if (isReleased) return;

        if (target == null || !target.activeInHierarchy)
        {
            SafeRelease();
            return;
        }

        if (!isInitialized)
        {
            targetPosition = target.transform.position;
            isInitialized = true;
        }

        MoveTowardsTarget(targetPosition);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReleased) return;
        HandleCollision(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isReleased) return;
        HandleCollision(collision.gameObject);
    }

    private void HandleCollision(GameObject hitObj)
    {
        if (isReleased || hitObj == null) return;

        bool shouldRelease = false;

        if (hitObj.CompareTag("Hero"))
        {
            if (hitObj.TryGetComponent<IDamageable>(out var damageable) && damageable.GetTeam() == TeamType.Hero)
            {
                damageable.TakeDamage(enemySO.damage);
            }
            shouldRelease = true;
        }       
        else if (hitObj.CompareTag("Wall") || hitObj.CompareTag("Obstacle"))
        {
            shouldRelease = true;
        }

        if (shouldRelease)
        {
            SafeRelease();
        }
    }

    private void MoveTowardsTarget(Vector2 targetPos)
    {
        if (isReleased) return;

        Vector2 currentPos = transform.position;
        Vector2 newPos = Vector2.MoveTowards(currentPos, targetPos, moveSpeed * Time.deltaTime);
        transform.position = newPos;

        if (Vector2.Distance(newPos, targetPos) < 0.1f)
        {
            SafeRelease();
        }
    }

    private void SafeRelease()
    {
        if (isReleased) return;

        isReleased = true;

        if (bulletCollider != null)
            bulletCollider.enabled = false;

        if (autoReleaseTween != null)
        {
            autoReleaseTween.Kill();
            autoReleaseTween = null;
        }

        if (pool != null)
        {
            try
            {
                pool.Release(gameObject);
            }
            catch (System.InvalidOperationException e)
            {
                Debug.LogError($"Pool release error for {gameObject.name}: {e.Message}");
                Destroy(gameObject);
            }
        }
    }

    private void StartAutoReleaseTween()
    {
        if (autoReleaseTween != null)
        {
            autoReleaseTween.Kill();
            autoReleaseTween = null;
        }

        autoReleaseTween = DOTween.Sequence()
            .AppendInterval(3f)
            .AppendCallback(() => {
                if (!isReleased) 
                {
                    SafeRelease();
                }
            });
    }

    public void ResetBullet()
    {
        isReleased = false;
        isInitialized = false;
        target = null;
        targetPosition = Vector2.zero;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        // Collider'ý tekrar aktif et
        if (bulletCollider != null)
            bulletCollider.enabled = true;
    }
}