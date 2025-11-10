using UnityEngine;

namespace Tabsil.DailyMissions
{
    [CreateAssetMenu(fileName = "Reward Sprite Map", menuName = "Scriptable Objects/Tabsil/Daily Missions/Reward Sprite Map", order = -9999999)]
    public class RewardSpriteMap : ScriptableObject
    {
        [Header(" Data ")]
        [SerializeField] private RewardSpriteData[] data;
        public RewardSpriteData[] Data => data;

        public Sprite GetSprite(ERewardType rewardType)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].type == rewardType)
                    return data[i].sprite;
            }

            Debug.LogWarning("No sprite found for this reward type : " + rewardType);
            return null;
        }
    }

    [System.Serializable]
    public struct RewardSpriteData
    {
        public ERewardType type;
        public Sprite sprite;
    }
}