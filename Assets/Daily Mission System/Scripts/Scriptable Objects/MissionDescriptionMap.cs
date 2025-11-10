using UnityEngine;
using System;

namespace Tabsil.DailyMissions
{
    [CreateAssetMenu(fileName = "Mission Description Map", menuName = "Scriptable Objects/Tabsil/Daily Missions/Mission Description Map", order = -9999999)]
    public class MissionDescriptionMap : ScriptableObject
    {
        [Header(" Data ")]
        [SerializeField] private MissionDescriptionData[] data;
        public MissionDescriptionData[] Data => data;

        public string GetDescription(EMissionType missionType, int target)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].type == missionType)
                    return string.Format(data[i].description, target);
            }

            Debug.LogWarning("No description found for this mission type : " + missionType);
            return "";
        }
    }

    [Serializable]
    public struct MissionDescriptionData
    {
        public EMissionType type;
        public string description;
    }
}