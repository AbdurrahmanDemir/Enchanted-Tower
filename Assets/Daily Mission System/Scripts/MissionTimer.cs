using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace Tabsil.DailyMissions
{
    using Sijil;

    public class MissionTimer : MonoBehaviour
    {
        [Header(" Elements ")]
        [SerializeField] private TextMeshProUGUI timerText;
        private MissionManager missionManager;

        [Header(" Data ")]
        private DateTime startTime;
        private DateTime endTime;
        private const string startTimeKey   = "DailyMissionStartTime";
        private const string endTimeKey     = "DailyMissionEndTime";

        public bool TimerIsActive { get; private set; }

        private TimeSpan TimeLeft 
        { 
            get { return endTime - DateTime.UtcNow; } 
            set { } 
        }

        public void Init(MissionManager missionManager)
        {
            this.missionManager = missionManager;

            LoadTimes();

            if (startTime == DateTime.MinValue || endTime == DateTime.MinValue)
                StartTimer();
            else
                ConfigureTimer();
        }

        public void StartTimer()
        {
            startTime = DateTime.UtcNow;
            endTime = startTime + TimeSpan.FromDays(1);

            SaveTimes();

            UpdateTimerText();

            StartCoroutine("TimerCoroutine");
            
            TimerIsActive = true;
        }

        public void ConfigureTimer()
        {
            if (endTime.CompareTo(DateTime.UtcNow) <= 0)
            {
                DayEnd();
                return;
            }

            TimerIsActive = true;

            UpdateTimerText();

            StartCoroutine("TimerCoroutine");
        }

        IEnumerator TimerCoroutine()
        {
            while(true)
            {
                yield return new WaitForSeconds(10);

                SaveTimes();

                // Check for day end
                if (endTime.CompareTo(DateTime.UtcNow) <= 0)
                {
                    DayEnd();
                    yield break;
                }

                UpdateTimerText();
            }
        }

        [NaughtyAttributes.Button]
        private void AddOneHour()
        {
            AddHours(1);
        }

        public void AddHours(int hours)
        {
            endTime -= TimeSpan.FromHours(hours);
            SaveTimes();

            // Check for day end
            if (endTime.CompareTo(DateTime.UtcNow) <= 0)
            {
                DayEnd();
                return;
            }

            UpdateTimerText();
        }   
        
        private void DayEnd()
        {
            StopAllCoroutines();
            missionManager.ResetMissions();

            TimerIsActive = false;
        }

        private void SaveTimes()
        {
            Sijil.Save(this, startTimeKey, startTime.ToString());
            Sijil.Save(this, endTimeKey, endTime.ToString());
        }

        private void LoadTimes()
        {
            if(Sijil.TryLoad(this, startTimeKey, out object _startTime))
            {
                string startTimeString = (string)_startTime;

                if (!DateTime.TryParse(startTimeString, out startTime))
                    Debug.LogWarning("Failed to parse the start time");
            }

            if (Sijil.TryLoad(this, endTimeKey, out object _endTime))
            {
                string endTimeString = (string)_endTime;

                if (!DateTime.TryParse(endTimeString, out endTime))
                    Debug.LogWarning("Failed to parse the end time");
            }
        }

        public void ResetSelf()
        {
            Sijil.Remove(this, startTimeKey);
            Sijil.Remove(this, endTimeKey);

            StartTimer();
        }

        private void UpdateTimerText()
        {
            timerText.text = CustomTimeSpanToString(TimeLeft);
        }

        public static string CustomTimeSpanToString(TimeSpan timeLeft)
        {
            int days = timeLeft.Days;
            int hours = timeLeft.Hours;
            int minutes = timeLeft.Minutes;

            string daysString = days > 0 ? days + "d" : "";
            string separation = hours > 0 ? " " : "";
            string hoursString = hours > 0 ? hours + "h" : "";

            bool showMinutesCondition = (days == 0 && hours >= 0) || (days >= 0 && hours == 0);

            string separationBis = showMinutesCondition ? " " : "";
            string minutesString = showMinutesCondition ? minutes + "min" : "";

            return daysString + separation + hoursString + separationBis + minutesString;
        }

        private void OnApplicationPause(bool pause)
        {
            if(pause)
                SaveTimes();
        }

        public void ResetData()
        {

        }
    }
}