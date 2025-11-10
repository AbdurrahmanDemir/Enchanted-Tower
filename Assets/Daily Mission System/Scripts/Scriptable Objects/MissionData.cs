using UnityEngine;

namespace Tabsil.DailyMissions
{
    [CreateAssetMenu(fileName = "Mission Data", menuName = "Scriptable Objects/Tabsil/Daily Missions/Mission Data", order = -1000000)]
    public class MissionData : ScriptableObject
    {
        [SerializeField] private EMissionType type;
        public EMissionType Type => type;

        [SerializeField] private int target;
        public int Target => target;

        [SerializeField] private int rewardXp;
        public int RewardXp => rewardXp;

        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;
    }
}