using System.Collections.Generic;
using UnityEngine;

public class QuestTracker : MonoBehaviour
{
    private QuestManager questManager;

    private void Awake()
    {
        questManager = GetComponent<QuestManager>();

        UIManager.gameWinCount += GameWinCallBack;
        LevelMapManager.levelPlayed += LevelPlayedCallBack;
        CardList.cardUpgrade += CardUpgradeCallBack;
        Enemy.enemyDead += EnemyKillCallBack;
        ChestManager.openedChest += OpenChestCallBack;

    }

    private void OnDestroy()
    {
        UIManager.gameWinCount -= GameWinCallBack;
        LevelMapManager.levelPlayed -= LevelPlayedCallBack;
        CardList.cardUpgrade-= CardUpgradeCallBack;
        Enemy.enemyDead -= EnemyKillCallBack;
        ChestManager.openedChest -= OpenChestCallBack;
    }

    private void UpdateQuestByType(QuestType questType, int incrementAmount = 1)
    {
        Dictionary<int, Quest> quests = new Dictionary<int, Quest>(questManager.GetCurrentQuest());

        foreach (KeyValuePair<int, Quest> questData in quests)
        {
            Quest quest = questData.Value;

            if (quest.Type == questType)
            {
                int currentProgress = Mathf.RoundToInt(quest.progress * quest.target);
                currentProgress += incrementAmount;

                currentProgress = Mathf.Min(currentProgress, quest.target);

                float newProgress = (float)currentProgress / quest.target;

                questManager.UpdateQuestProgress(questData.Key, newProgress);
            }
        }
    }

    private void UpdateQuestFromPlayerPrefs(QuestType questType, string playerPrefsKey)
    {
        Dictionary<int, Quest> quests = new Dictionary<int, Quest>(questManager.GetCurrentQuest());


        foreach (KeyValuePair<int, Quest> questData in quests)
        {
            Quest quest = questData.Value;

            if (quest.Type == questType)
            {
                int totalValue = PlayerPrefs.GetInt(playerPrefsKey, 0);

                totalValue = Mathf.Min(totalValue, quest.target);

                float newProgress = (float)totalValue / quest.target;

                questManager.UpdateQuestProgress(questData.Key, newProgress);
            }
        }
    }

    private void GameWinCallBack()
    {
        UpdateQuestByType(QuestType.GameWin);
    }

    private void CardUpgradeCallBack()
    {
        UpdateQuestByType(QuestType.CardUpgrade);
    }

    private void LevelPlayedCallBack()
    {
        UpdateQuestByType(QuestType.LevelPlayed);
    }
    private void EnemyKillCallBack()
    {
        UpdateQuestByType(QuestType.Kill);
    }
    private void OpenChestCallBack()
    {
        UpdateQuestByType(QuestType.OpenChest);
    }
}