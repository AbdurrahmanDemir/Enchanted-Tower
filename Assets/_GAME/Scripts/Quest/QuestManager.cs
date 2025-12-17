using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private Quest[] quests;
    private Dictionary<int, Quest> uncompletedQuestDictionary = new Dictionary<int, Quest>();

    [Header("Elements")]
    [SerializeField] private QuestContainer questContainerPrefab;
    [SerializeField] private Transform questContainerParent;

    [Header("KingQuest")]
    private int kingQuestCount;

    private void Awake()
    {
        QuestContainer.onRewardClaimed += QuestRewardClaimedCallback;
    }

    private void OnDestroy()
    {
        QuestContainer.onRewardClaimed -= QuestRewardClaimedCallback;
    }

    private void Start()
    {
        CreateQuestContainers();
    }

    private void QuestRewardClaimedCallback(int questIndex)
    {
        SetQuestComplete(questIndex);
        ResetQuestProgress(questIndex); 

        int reward = quests[questIndex].reward;
        DataManager.instance.AddGold(reward);
        kingQuestCount++;
        PlayerPrefs.SetInt("KingQuest", kingQuestCount);
        UpdateQuest();
    }

    private void UpdateQuest()
    {
        foreach (Transform child in questContainerParent)
        {
            Destroy(child.gameObject);
        }

        CreateQuestContainers();
    }

    public void CreateQuestContainers()
    {
        StoreUncompletedMissions();

        foreach (KeyValuePair<int, Quest> questData in uncompletedQuestDictionary)
        {
            CreateQuestContainer(questData);
        }
    }

    private void StoreUncompletedMissions()
    {
        uncompletedQuestDictionary.Clear();

        for (int i = 0; i < quests.Length; i++)
        {
            if (IsQuestComplete(i))
                continue;

            Quest quest = quests[i];
            quest.progress = GetQuestProgress(i);

            uncompletedQuestDictionary.Add(i, quest);

            if (uncompletedQuestDictionary.Count >= 5)
                break;
        }
    }

    private void CreateQuestContainer(KeyValuePair<int, Quest> questData)
    {
        QuestContainer instance = Instantiate(questContainerPrefab, questContainerParent);

        string title = GetQuestTitle(questData.Value);
        string rewardString = questData.Value.reward.ToString();
        float progress = questData.Value.progress;

        instance.Configure(title, rewardString, progress, questData.Key);
    }

    private string GetQuestTitle(Quest quest)
    {
        switch (quest.Type)
        {
            case QuestType.Kill:
                return "Kill " + quest.target + " enemies";
            case QuestType.OpenChest:
                return "Open " + quest.target + " chest";
            case QuestType.GameWin:
                return "Complete " + quest.target + " level";
            case QuestType.CardUpgrade:
                return "Upgrade " + quest.target + " card";
            case QuestType.LevelPlayed:
                return "Play " + quest.target + " level";
            default:
                return "Unknown Quest";
        }
    }

    public void UpdateQuestProgress(int questIndex, float newProgress)
    {
        newProgress = Mathf.Clamp01(newProgress);

        SaveQuestProgress(questIndex, newProgress);

        Quest quest = quests[questIndex];
        quest.progress = newProgress;
        quests[questIndex] = quest;

        if (uncompletedQuestDictionary.ContainsKey(questIndex))
        {
            uncompletedQuestDictionary[questIndex] = quest;
        }

        UpdateQuestContainerUI(questIndex, newProgress);
    }

    private void UpdateQuestContainerUI(int questIndex, float progress)
    {
        if (questContainerParent == null) return;

        foreach (Transform child in questContainerParent)
        {
            QuestContainer container = child.GetComponent<QuestContainer>();
            if (container != null && container.GetKey() == questIndex)
            {
                container.UpdateProgress(progress);
                break;
            }
        }
    }

    public void KingQuest()
    {
        if (PlayerPrefs.GetInt("KingQuest", 0)>=5)
        {
            DataManager.instance.AddGold(1500);
            DataManager.instance.AddEnergy(20);
            DataManager.instance.AddHeroToken(300);
            kingQuestCount = 0;
            PlayerPrefs.SetInt("KingQuest", kingQuestCount);

        }
        else
        {
            PopUpController.instance.OpenPopUp("INSUFFICIENT MISSION WERE COMPLETED.");
        }
    }

    public Dictionary<int, Quest> GetCurrentQuest()
    {
        return uncompletedQuestDictionary;
    }

    private float GetQuestProgress(int questIndex)
    {
        return PlayerPrefs.GetFloat("QuestProgress" + questIndex, 0f);
    }

    private void SaveQuestProgress(int key, float progress)
    {
        PlayerPrefs.SetFloat("QuestProgress" + key, progress);
        PlayerPrefs.Save();
    }

    private void ResetQuestProgress(int questIndex)
    {
        PlayerPrefs.SetFloat("QuestProgress" + questIndex, 0f);
        PlayerPrefs.Save();
    }

    private void SetQuestComplete(int questIndex)
    {
        PlayerPrefs.SetInt("Quest" + questIndex, 1);
        PlayerPrefs.Save();
    }

    private bool IsQuestComplete(int questIndex)
    {
        return PlayerPrefs.GetInt("Quest" + questIndex, 0) == 1;
    }
}

public enum QuestType { Kill, OpenChest, CardUpgrade, GameWin, LevelPlayed }

[System.Serializable]
public struct Quest
{
    public QuestType Type;
    public int target;
    public int reward;
    public float progress;
}