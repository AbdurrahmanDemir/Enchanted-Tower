using UnityEngine;

namespace Tabsil.DailyMissions
{
    [CreateAssetMenu(fileName = "Reward Group Data", menuName = "Scriptable Objects/Tabsil/Daily Missions/Reward Group Data", order = -1000000)]
    public class RewardGroupData : ScriptableObject
    {
        [Header(" Data ")]
        [SerializeField] private RewardMilestoneData[] rewardMilestoneDatas;
        public RewardMilestoneData[] RewardMilestoneDatas => rewardMilestoneDatas;
    }

    [System.Serializable]
    public struct RewardMilestoneData
    {
        public Sprite icon;
        public RewardEntryData[] rewards;
        public int requiredXp;
    }

    [System.Serializable]
    public struct RewardEntryData
    {
        public ERewardType type;
        public int amount;
    }
}