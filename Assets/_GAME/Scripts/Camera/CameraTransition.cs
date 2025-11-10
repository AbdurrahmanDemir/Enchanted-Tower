using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraTransition : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private float originalOrthoSize;

    [SerializeField] private Transform targetTransform; // Hedef pozisyon (boþ bir GameObject olabilir)
    [SerializeField] private float targetOrthoSize = 3f; // Gidilecek ortographic size
    [SerializeField] private float transitionDuration = 1.5f;

    private Camera cam;
    private Coroutine currentTransition;

    void Start()
    {
        cam = GetComponent<Camera>();

        // Kameranýn ilk deðerlerini kaydet
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalOrthoSize = cam.orthographicSize;
    }

    public void MoveToTarget()
    {
        if (currentTransition != null) StopCoroutine(currentTransition);
        currentTransition = StartCoroutine(TransitionTo(
            targetTransform.position,
            targetTransform.rotation,
            targetOrthoSize
        ));
    }

    public void MoveToOriginal()
    {
        if (currentTransition != null) StopCoroutine(currentTransition);
        currentTransition = StartCoroutine(TransitionTo(
            originalPosition,
            originalRotation,
            originalOrthoSize
        ));
    }

    private IEnumerator TransitionTo(Vector3 targetPos, Quaternion targetRot, float targetSize)
    {
        float elapsed = 0f;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float startSize = cam.orthographicSize;

        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Son deðerleri kesinleþtir
        transform.position = targetPos;
        transform.rotation = targetRot;
        cam.orthographicSize = targetSize;
    }
}
