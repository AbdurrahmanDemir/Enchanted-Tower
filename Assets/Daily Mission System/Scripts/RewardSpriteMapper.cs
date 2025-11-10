using UnityEngine;

namespace Tabsil.DailyMissions
{
    public static class RewardSpriteMapper 
    {
        static RewardSpriteMap data;

        const string dataPath = "Reward Sprite Map";

        static RewardSpriteMapper()
            => data = Resources.Load(dataPath) as RewardSpriteMap;

        public static Sprite GetSprite(ERewardType type)
            => data.GetSprite(type);
    }
}