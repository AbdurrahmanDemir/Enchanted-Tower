using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class LevelMapManager : MonoBehaviour
{

    [SerializeField] private EnemyBaseManager enemyBaseManager;
    [SerializeField] private UIManager uiManager;
    [Header("Elements")]
    [SerializeField] private int levelEpisodeIndex;
    [Header("Settings")]
    [SerializeField] private LevelEpisode[] levelEpisodes;

    [Header("Level Details Panel")]
    [SerializeField] private GameObject levelDetailsPanel;
    [SerializeField] private TextMeshProUGUI levelName;
    [SerializeField] private TextMeshProUGUI levelDate;
    [SerializeField] private TextMeshProUGUI levelType;
    [SerializeField] private TextMeshProUGUI levelDescription;
    [SerializeField] private Button levelPlayButton;

    private void Start()
    {
        levelEpisodeIndex = PlayerPrefs.GetInt("LevelEpisodeIndex", 0);
        LevelMapButtonUpdate();

        int currentLevel = GetCurrentLevelForEpisode(levelEpisodeIndex);
        Debug.Log($"Episode {levelEpisodeIndex} - Current Level: {currentLevel}");
    }

    public void LevelMapButtonUpdate()
    {
        for (int episodeIndex = 0; episodeIndex < levelEpisodes.Length; episodeIndex++)
        {
            levelEpisodes[episodeIndex].episodeLevelMap.SetActive(false);
        }

        levelEpisodes[levelEpisodeIndex].episodeLevelMap.SetActive(true);

        int currentLevel = GetCurrentLevelForEpisode(levelEpisodeIndex);

        for (int i = 0; i < levelEpisodes[levelEpisodeIndex].episodeDetails.Length; i++)
        {
            bool isActive = (i < currentLevel);
            levelEpisodes[levelEpisodeIndex].levelButton[i].SetActive(true);

            levelEpisodes[levelEpisodeIndex].levelButtonText[i].text = (i + 1).ToString();

            Image buttonImage = levelEpisodes[levelEpisodeIndex].levelButton[i].GetComponent<Image>();
            if (buttonImage != null)
            {
                if (isActive)
                {
                    buttonImage.color = Color.white;
                }
                else
                {
                    buttonImage.color = Color.gray;
                }
            }

            Button button = levelEpisodes[levelEpisodeIndex].levelButton[i].GetComponent<Button>();
            button.onClick.RemoveAllListeners();

            if (isActive)
            {
                int capturedIndex = i;
                button.onClick.AddListener(() => LevelDetailsPanel(capturedIndex));
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        }
    }

    private int GetCurrentLevelForEpisode(int episodeIndex)
    {
        string key = $"Episode_{episodeIndex}_CurrentLevel";
        return PlayerPrefs.GetInt(key, 1);
    }

    public void SetCurrentLevelForEpisode(int episodeIndex, int level)
    {
        string key = $"Episode_{episodeIndex}_CurrentLevel";
        PlayerPrefs.SetInt(key, level);
        PlayerPrefs.Save();
    }

    public bool IsEpisodeUnlocked(int episodeIndex)
    {
        if (episodeIndex == 0) return true; 

        int previousEpisodeMaxLevel = GetCurrentLevelForEpisode(episodeIndex - 1);
        int previousEpisodeTotalLevels = levelEpisodes[episodeIndex - 1].episodeDetails.Length;

        return previousEpisodeMaxLevel > previousEpisodeTotalLevels;
    }

    public void SelectEpisode(int episodeIndex)
    {
        if (IsEpisodeUnlocked(episodeIndex))
        {
            levelEpisodeIndex = episodeIndex;
            PlayerPrefs.SetInt("LevelEpisodeIndex", levelEpisodeIndex);
            PlayerPrefs.Save();
            LevelMapButtonUpdate();
        }
        else
        {
            Debug.Log($"Episode {episodeIndex} henüz açýlmamýþ!");
        }
    }

    public void LevelDetailsPanel(int index)
    {
        OpenPanel(levelDetailsPanel);
        levelName.text = "LEVEL: " + levelEpisodes[levelEpisodeIndex].episodeDetails[index].episodeData.baseName;
        levelDate.text = levelEpisodes[levelEpisodeIndex].episodeDetails[index].episodeData.episodeDate;
        levelType.text = "Level Type: " + levelEpisodes[levelEpisodeIndex].episodeDetails[index].episodeData.levelType.ToString();
        levelDescription.text = levelEpisodes[levelEpisodeIndex].episodeDetails[index].episodeData.cardDescription;
        levelPlayButton.onClick.RemoveAllListeners();
        levelPlayButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt("PlayingEpisode", levelEpisodeIndex);
            PlayerPrefs.SetInt("PlayingLevel", index);
            PlayerPrefs.Save();

            int realAssetIndex = CalculateAssetIndex(levelEpisodeIndex, index);
            enemyBaseManager.LoadLevel(realAssetIndex);
            uiManager.GameUIStageChanged(UIGameStage.Game);
            levelEpisodes[levelEpisodeIndex].episodeLevelMap.SetActive(false);
        });
    }

    private int CalculateAssetIndex(int episodeIndex, int levelIndex)
    {
        int totalIndex = 0;

        for (int i = 0; i < episodeIndex; i++)
        {
            totalIndex += levelEpisodes[i].episodeDetails.Length;
        }

        totalIndex += levelIndex;

        Debug.Log($"Episode {episodeIndex}, Level {levelIndex} -> Asset Index: {totalIndex}");
        return totalIndex;
    }

    public int GetEpisodeLevelCount(int episodeIndex)
    {
        if (episodeIndex >= 0 && episodeIndex < levelEpisodes.Length)
        {
            return levelEpisodes[episodeIndex].episodeDetails.Length;
        }
        return 0;
    }

    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }
}

[System.Serializable]
public struct LevelEpisode
{
    [Header("Episode Info")]
    public string episodeName;
    public int episodeNumber;
    public EpisodeDetails[] episodeDetails;

    [Header("Episode Elements")]
    public GameObject episodeLevelMap;
    public GameObject episodeArena;
    public GameObject[] levelButton;
    public TextMeshProUGUI[] levelButtonText;
}

[System.Serializable]
public struct EpisodeDetails
{
    public GridEnemyBaseData episodeData;
}