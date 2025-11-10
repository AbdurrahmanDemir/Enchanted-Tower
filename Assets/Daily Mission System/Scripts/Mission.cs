using UnityEngine;
using System;

namespace Tabsil.DailyMissions
{
    [System.Serializable]
    public class Mission
    {

        [Header(" Actions ")]
        public static Action<Mission> updated;
        public static Action<Mission> completed;

        private MissionData data;
        public MissionData Data => data;

        private bool isComplete;
        public bool IsComplete => isComplete;

        private bool isClaimed;
        public bool IsClaimed => isClaimed;

        private int amount;
        public int Amount
        {
            get => amount;
            set
            {
                amount = Mathf.Min(value, data.Target);

                updated?.Invoke(this);

                if (amount == data.Target)
                    Complete();
            }
        }

        public float Progress => (float)amount / data.Target;
        public string ProgressString => amount + " / " + data.Target;

        public EMissionType Type => data.Type;

        public Mission(MissionData data)
        {
            this.data = data;
        }

        public Mission(MissionData data, int amount, bool claimedState)
        {
            this.data = data;
            this.amount = amount;

            if (claimedState)
                Claim();
        }

        public void Complete()
        {
            isComplete = true;
            completed?.Invoke(this);
        }

        public void Claim()
        {
            isComplete = true;
            isClaimed = true;
        }
    }
}