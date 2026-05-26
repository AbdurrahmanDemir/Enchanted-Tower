using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingSceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private GameObject loadingIcon;

    [Header("Settings")]
    [SerializeField] private float minimumLoadTime = 1.5f;
    [SerializeField] private string targetSceneName = "PixelGame";

    [Header("Persistent Managers")]
    [SerializeField] private GameObject iapCorePrefab;

    private void Start()
    {
        if (IAPCore.Instance == null && iapCorePrefab != null)
        {
            Instantiate(iapCorePrefab);
        }

        StartCoroutine(LoadGameScene());
    }

    private IEnumerator LoadGameScene()
    {
        float startTime = Time.time;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
        asyncLoad.allowSceneActivation = false;
        

        if (loadingIcon != null)
        {
            StartCoroutine(RotateLoadingIcon());
        }

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            if (progressBar != null)
            {
                progressBar.value = progress;
            }

            if (loadingText != null)
            {
                loadingText.text = $"Loading... {(progress * 100):F0}%";
            }

            float elapsedTime = Time.time - startTime;
            if (asyncLoad.progress >= 0.9f && elapsedTime >= minimumLoadTime)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private IEnumerator RotateLoadingIcon()
    {
        while (true)
        {
            loadingIcon.transform.Rotate(0, 0, -360 * Time.deltaTime);
            yield return null;
        }
    }
}