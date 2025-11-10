using UnityEngine;
using System.Collections.Generic;
using System;
//using Coffee.UIExtensions;

namespace Tabsil.DailyMissions
{
    using Sijil;
    using System.Collections;

    [RequireComponent(typeof(UIMissionManager))]
    [RequireComponent(typeof(MissionTimer))]
    public class MissionManager : MonoBehaviour
    {
        public static MissionManager instance;

        [Header(" Components ")]
        private UIMissionManager ui;
        private MissionTimer timer;

        [Header(" Data ")]
        [SerializeField] private MissionData[] missionDatas;
        List<Mission> activeMissions = new List<Mission>();

        private int xp;
        public int Xp => xp;

        [Header(" Actions ")]
        public static Action<int> xpUpdated;
        public static Action reset;

        [Header(" Effects ")]
        [SerializeField] private ParticleSystem diamondParticles;
        [SerializeField] private Transform particlesParent;
        //private UIParticleAttractor diamondParticleAttractor;

        [Header(" Save / Load ")]
        private bool shouldSave;
        private int[] amounts;
        private bool[] claimedStates;
        private const string amountsKey         = "MissionDataAmounts";
        private const string claimedStatesKey   = "MissionClaimedStates";
        private const string xpKey              = "MissionXp";

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            ui = GetComponent<UIMissionManager>();
            timer = GetComponent<MissionTimer>();

            Mission.updated                         += OnMissionUpdated;
            Mission.completed                       += OnMissionCompleted;

            //MainMissionSlider.attractorInitialized  += OnAttractorInitialized;

            StartCoroutine("SaveCoroutine");
        }


        private void OnDestroy()
        {
            Mission.updated                         -= OnMissionUpdated;
            Mission.completed                       -= OnMissionCompleted;

            //MainMissionSlider.attractorInitialized  -= OnAttractorInitialized;
        }

        //private void OnAttractorInitialized(UIParticleAttractor attractor)
        //{
        //    diamondParticleAttractor = attractor;
        //    diamondParticleAttractor.onAttracted.AddListener(OnDiamondParticleAttracted);
        //}

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Load();

            for (int i = 0; i < missionDatas.Length; i++)
                activeMissions.Add(new Mission(missionDatas[i], amounts[i], claimedStates[i]));

            ui.Init(activeMissions.ToArray());
            timer.Init(this);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
                Increment(EMissionType.Clicks, 1);

            else if (Input.GetKeyDown(KeyCode.UpArrow))
                Increment(EMissionType.PressUp, 1);

            else if (Input.GetKeyDown(KeyCode.DownArrow))
                Increment(EMissionType.PressDown, 1);
        }

        private void OnMissionUpdated(Mission mission)
        {
            ui.UpdateMission(activeMissions.IndexOf(mission));
        }

        private void OnMissionCompleted(Mission mission)
        {

        }

        public void HandleMissionClaimed(int index)
        {
            Mission mission = activeMissions[index];
            mission.Claim();

            int particleCount = mission.Data.RewardXp;

            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            ParticleSystem diamondParticlesInstance = Instantiate(diamondParticles, screenCenter, Quaternion.identity, particlesParent);

            //diamondParticleAttractor.AddParticleSystem(diamondParticlesInstance);

            diamondParticlesInstance.emission.SetBurst(0, new ParticleSystem.Burst(0, particleCount));
            diamondParticlesInstance.Play();

            Save();
        }

        private void OnDiamondParticleAttracted()
        {
            xp++;
            xpUpdated?.Invoke(xp);

            shouldSave = true;
        }

        public void ResetMissions()
        {
            amounts = new int[missionDatas.Length];
            claimedStates = new bool[missionDatas.Length];
            xp = 0;

            Sijil.Remove(this, amountsKey);
            Sijil.Remove(this, claimedStatesKey);
            Sijil.Remove(this, xpKey);

            activeMissions.Clear();

            for (int i = 0; i < missionDatas.Length; i++)
                activeMissions.Add(new Mission(missionDatas[i]));

            ui.Init(activeMissions.ToArray());

            timer.ResetSelf();

            reset?.Invoke();
        }

        public static void Increment(EMissionType missionType, int addend)
        {
            bool incremented = false;

            for (int i = 0; i < instance.activeMissions.Count; i++)
            {
                if (instance.activeMissions[i].IsComplete || instance.activeMissions[i].IsClaimed)
                    continue;

                if (instance.activeMissions[i].Type == missionType)
                    instance.activeMissions[i].Amount += addend;

                incremented = true;
            }

            if (incremented)
                instance.Save();
        }
        
        private void Load()
        {
            amounts = new int[missionDatas.Length];
            claimedStates = new bool[missionDatas.Length];

            if (Sijil.TryLoad(this, amountsKey, out object _amounts))
                amounts = (int[])_amounts;

            if (Sijil.TryLoad(this, claimedStatesKey, out object _claimedStates))
                claimedStates = (bool[])_claimedStates;

            if (Sijil.TryLoad(this, xpKey, out object _xp))
                xp = (int)_xp;
        }

        private void Save()
        {
            Debug.Log("Saving...");

            for (int i = 0; i < activeMissions.Count; i++)
            {
                amounts[i] = activeMissions[i].Amount;
                claimedStates[i] = activeMissions[i].IsClaimed;
            }

            Sijil.Save(this, amountsKey, amounts);
            Sijil.Save(this, claimedStatesKey, claimedStates);
            Sijil.Save(this, xpKey, xp);
        }

        IEnumerator SaveCoroutine()
        {
            while(true)
            {
                yield return new WaitForSeconds(5);

                if (shouldSave)
                {
                    Save();
                    shouldSave = false;
                }
            }
        }
    }
}