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

        for (int i = 0; i < levelAssetReferences.Length; i++)
        {
            if (levelAssetReferences[i] == null)
            {
            }
            else if (!levelAssetReferences[i].RuntimeKeyIsValid())
            {
            }
            else
            {
            }
        }
    }

    public async void LoadLevel(int index)
    {
        if (index < 0 || index >= levelAssetReferences.Length)
        {
            return;
        }

        if (levelAssetReferences[index] == null)
        {
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
              

                currentLevelObj = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity, levelSpawnRoot);

                if (currentLevelObj == null)
                {
                    loadingPanel.SetActive(false);
                    return;
                }


                var navMeshSurface = currentLevelObj.GetComponentInChildren<NavMeshSurface>();
                if (navMeshSurface != null)
                {
                    navMeshSurface.BuildNavMesh();
                }
                else
                {
                    Debug.LogWarning("NavMeshSurface bulunamadý");
                }

                StartCoroutine(SpawnEnemiesSafely());
            }
            else
            {
                Debug.LogError($"Level yükleme baþarýsýz {currentLevelHandle.Status}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Level yüklenirken hata: {e.Message}");
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

        foreach (var agent in agents)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(agent.transform.position, out hit, 1f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
            else
            {
                Debug.LogWarning("NavMeshAgent spawn pozisyonu NavMesh üzerinde deðil: " + agent.name);
            }
        }

        Debug.Log($"Toplam alive count: {aliveCount}");
    }

    public async System.Threading.Tasks.Task UnloadCurrentLevel()
    {
        if (currentLevelObj != null)
        {
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
    }

    public void UnRegisterObject()
    {
        aliveCount--;

        if (aliveCount <= 0)
        {

            int playingEpisode = PlayerPrefs.GetInt("PlayingEpisode", 0);
            int playingLevel = PlayerPrefs.GetInt("PlayingLevel", 0);

            Debug.Log($"Tamamlanan Episode: {playingEpisode}, Level: {playingLevel}");

            int newLevel = playingLevel + 2;
            levelMapManager.SetCurrentLevelForEpisode(playingEpisode, newLevel);

            int totalLevelsInEpisode = GetTotalLevelsInEpisode(playingEpisode);

            if (playingLevel >= totalLevelsInEpisode - 1)
            {

                int newEpisodeIndex = playingEpisode + 1;
                PlayerPrefs.SetInt("LevelEpisodeIndex", newEpisodeIndex);

                levelMapManager.SetCurrentLevelForEpisode(newEpisodeIndex, 1);

                PlayerPrefs.Save();

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