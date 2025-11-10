using System.Collections.Generic;
using UnityEngine;

namespace Tabsil.DailyMissions
{ 
    public class UIMissionManager : MonoBehaviour
    {
        [Header(" Elements ")]
        [SerializeField] private UIMissionContainer missionContainerPrefab;
        [SerializeField] private Transform missionContainersParent;

        List<UIMissionContainer> activeMissionContainers = new List<UIMissionContainer>();


        public void Init(Mission[] activeMissions)
        {
            missionContainersParent.Clear();
            activeMissionContainers.Clear();

            for (int i = 0; i < activeMissions.Length; i++)
            {
                UIMissionContainer containerInstance = Instantiate(missionContainerPrefab, missionContainersParent);

                int _i = i;
                containerInstance.Configure(activeMissions[i], () => ClaimMission(_i));

                activeMissionContainers.Add(containerInstance);
            }

            Reorder();
        }

        private void Reorder()
        {
            for (int i = 0; i < activeMissionContainers.Count; i++)
                if (activeMissionContainers[i].IsClaimed)
                    activeMissionContainers[i].transform.SetAsLastSibling();           
        }

        private void ClaimMission(int index)
        {
            activeMissionContainers[index].Claim();
            activeMissionContainers[index].transform.SetAsLastSibling();

            MissionManager.instance.HandleMissionClaimed(index);
        }

        public void UpdateMission(int index)
        {
            activeMissionContainers[index].UpdateVisuals();
        }
    }
}