using NavMeshPlus.Components;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class EnemyBaseManager : MonoBehaviour
{
    public static EnemyBaseManager instance;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private LevelMapManager levelMapManager;

    [Header("Addressable Level References")]
    public AssetReference[] levelAssetReferences;

    [Header("Loading UI")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private UnityEngine.UI.Slider loadingSlider;

    [Header("Assign")]
    public Transform levelSpawnRoot;

    private GameObject currentLevelObj;
    private AsyncOperationHandle<GameObject> currentLevelHandle;
    public int aliveCount = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        CheckAssetReferences();

        if (levelMapManager == null)
            levelMapManager = FindObjectOfType<LevelMapManager>();
    }

    private void CheckAssetReferences()
    {
        Debug.Log($"Toplam AssetReference say?s?: {levelAssetReferences.Length}");

        for (int i = 0; i < levelAssetReferences.Length; i++)
        {
            if (levelAssetReferences[i] == null)
            {
                Debug.LogError($"AssetReference {i} NULL!");
            }
            else if (!levelAssetReferences[i].RuntimeKeyIsValid())
            {
                Debug.LogError($"AssetReference {i} RuntimeKey ge?ersiz!");
            }
            else
            {
                Debug.Log($"AssetReference {i} OK: {levelAssetReferences[i].AssetGUID}");
            }
        }
    }

    public async void LoadLevel(int index)
    {
        if (index < 0 || index >= levelAssetReferences.Length)
        {
            Debug.LogError($"Ge?ersiz level index: {index}. Toplam level say?s?: {levelAssetReferences.Length}");
            return;
        }

        if (levelAssetReferences[index] == null)
        {
            Debug.LogError($"Level AssetReference null: {index}");
            return;
        }

        Debug.Log($"Level y?kleniyor: {index}");

        await UnloadCurrentLevel();

        try
        {
            loadingPanel.SetActive(true);
            loadingSlider.value = 0f;

            currentLevelHandle = levelAssetReferences[index].LoadAssetAsync<GameObject>();

            while (!currentLevelHandle.IsDone)
            {
                loadingSlider.value = currentLevelHandle.PercentComplete;
                await System.Threading.Tasks.Task.Yield();
            }

            GameObject levelPrefab = await currentLevelHandle.Task;

            if (currentLevelHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Level prefab ba?ar?yla y?klendi: {levelPrefab.name}");

                currentLevelObj = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity, levelSpawnRoot);

                if (currentLevelObj == null)
                {
                    Debug.LogError("Level objesi olu?turulamad?!");
                    loadingPanel.SetActive(false);
                    return;
                }

                Debug.Log($"Level objesi olu?turuldu: {currentLevelObj.name}");

                var navMeshSurface = currentLevelObj.GetComponentInChildren<NavMeshSurface>();
                if (navMeshSurface != null)
                {
                    Debug.Log("NavMeshSurface bulundu, build ediliyor...");
                    navMeshSurface.BuildNavMesh();
                }
                else
                {
                    Debug.LogWarning("NavMeshSurface bulunamad?!");
                }

                StartCoroutine(SpawnEnemiesSafely());
            }
            else
            {
                Debug.LogError($"Level y?kleme ba?ar?s?z: {currentLevelHandle.Status}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Level y?klenirken hata: {e.Message}");
        }
        finally
        {
            loadingPanel.SetActive(false);
        }
    }

    private IEnumerator SpawnEnemiesSafely()
    {
        yield return null;

        NavMeshAgent[] agents = currentLevelObj.GetComponentsInChildren<NavMeshAgent>(true);
        Debug.Log($"Bulunan NavMeshAgent say?s?: {agents.Length}");

        foreach (var agent in agents)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(agent.transform.position, out hit, 1f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
                Debug.Log($"Agent spawn edildi: {agent.name}");
            }
            else
            {
                Debug.LogWarning("NavMeshAgent spawn pozisyonu NavMesh ?zerinde de?il: " + agent.name);
            }
        }

        Debug.Log($"Toplam alive count: {aliveCount}");
    }

    public async System.Threading.Tasks.Task UnloadCurrentLevel()
    {
        if (currentLevelObj != null)
        {
            Debug.Log("Mevcut level destroy ediliyor...");
            Destroy(currentLevelObj);
            currentLevelObj = null;
        }

        if (currentLevelHandle.IsValid())
        {
            Addressables.Release(currentLevelHandle);
        }

        await System.Threading.Tasks.Task.Yield();
    }

    public void RegisterObject(string name)
    {
        aliveCount++;
        Debug.Log($"Object registered. Alive count: {aliveCount} - {name}");
    }

    public void UnRegisterObject()
    {
        aliveCount--;
        Debug.Log($"Object unregistered. Alive count: {aliveCount}");

        if (aliveCount <= 0)
        {
            Debug.Log("T?m d??manlar ?ld?, oyun kazan?ld?!");

            int playingEpisode = PlayerPrefs.GetInt("PlayingEpisode", 0);
            int playingLevel = PlayerPrefs.GetInt("PlayingLevel", 0);

            Debug.Log($"Tamamlanan Episode: {playingEpisode}, Level: {playingLevel}");

            int newLevel = playingLevel + 2;
            levelMapManager.SetCurrentLevelForEpisode(playingEpisode, newLevel);

            int totalLevelsInEpisode = GetTotalLevelsInEpisode(playingEpisode);

            if (playingLevel >= totalLevelsInEpisode - 1)
            {
                Debug.Log($"Episode {playingEpisode} tamamland?! Yeni episode a??l?yor...");

                int newEpisodeIndex = playingEpisode + 1;
                PlayerPrefs.SetInt("LevelEpisodeIndex", newEpisodeIndex);

                levelMapManager.SetCurrentLevelForEpisode(newEpisodeIndex, 1);

                PlayerPrefs.Save();

                Debug.Log($"Yeni episode a??ld?: {newEpisodeIndex}");
            }

            uiManager.GameWinPanel();
        }
    }

    private int GetTotalLevelsInEpisode(int episodeIndex)
    {

        if (levelMapManager != null)
        {
            return levelMapManager.GetEpisodeLevelCount(episodeIndex);
        }

        return 6;
    }

    private void OnDestroy()
    {
        if (currentLevelHandle.IsValid())
        {
            Addressables.Release(currentLevelHandle);
        }
    }
}