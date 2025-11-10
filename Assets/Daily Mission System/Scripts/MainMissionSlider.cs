using System;
using UnityEngine;
using UnityEngine.UI;
//using Coffee.UIExtensions;
using System.Collections;
using TMPro;
using Tabsil.ModernUISystem;
using System.Collections.Generic;

namespace Tabsil.DailyMissions
{
    using Sijil;

    public class MainMissionSlider : MonoBehaviour
    {
        [Header(" Elements ")]
        [SerializeField] private Slider slider;
        [SerializeField] private UISliderItem itemPrefab;
        [SerializeField] private UISliderItem diamondItemPrefab;
        [SerializeField] private RectTransform itemsParent;
        [SerializeField] private UIRewardPopup rewardPopup;
        private TextMeshProUGUI xpText;

        [Header(" Settings ")]
        [SerializeField] private Sprite currencyIcon;
        private List<UISliderItem> sliderItems = new List<UISliderItem>();

        [Header(" Data ")]
        [SerializeField] private RewardGroupData data;
        private int lastRewardIndex;
        private bool[] itemsOpened;
        private const string lastRewardIndexKey = "MissionLastRewardIndex";
        private const string itemsOpenedKey     = "MissionItemsOpened";

        [Header(" Actions ")]
        //public static Action<UIParticleAttractor> attractorInitialized;
        public static Action<RewardEntryData[]> rewarded;

        private void Awake()
        {
            MissionManager.xpUpdated    += OnXpUpdated;
            MissionManager.reset        += ResetSelf;
        }

        private void OnDestroy()
        {
            MissionManager.xpUpdated    -= OnXpUpdated;
            MissionManager.reset        -= ResetSelf;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        IEnumerator Start()
        {
            yield return null;

            lastRewardIndex = 0;
            itemsOpened = new bool[data.RewardMilestoneDatas.Length];

            Load();
            
            Init();
        }

        private void Init()
        {
            GenerateSliderItems();
            InitSlider();
            UpdateVisuals(MissionManager.instance.Xp);

            AnimateUnopenedItems();
        }

        private void GenerateSliderItems()
        {
            itemsParent.Clear();
            sliderItems.Clear();

            UISliderItem diamondItem = Instantiate(diamondItemPrefab, itemsParent);
            diamondItem.Configure(currencyIcon, 0.ToString());

            xpText = diamondItem.Text;

            //attractorInitialized?.Invoke(diamondItem.GetComponent<UIParticleAttractor>());

            for (int i = 0; i < data.RewardMilestoneDatas.Length; i++)
            {
                RewardMilestoneData milestoneData = data.RewardMilestoneDatas[i];

                UISliderItem itemInstance = Instantiate(itemPrefab, itemsParent);
                itemInstance.Configure(milestoneData.icon, milestoneData.requiredXp.ToString());

                int _i = i;
                itemInstance.Button.onClick.AddListener(() => HandleSliderItemPressed(_i));

                sliderItems.Add(itemInstance);
            }

            PlaceItems();
        }

        private void HandleSliderItemPressed(int index)
        {
            // Check if we can reward the player the reward he has clicked
            bool canOpen = lastRewardIndex > index;
            bool isOpened = itemsOpened[index];

            if (!canOpen || isOpened)
                return;

            OpenReward(index);
        }

        private void OpenReward(int index)
        {
            itemsOpened[index] = true;

            // Stop animating the slider item
            //itemsParent.GetChild(index + 1).GetComponent<UISliderItem>().StopAnimation();
            sliderItems[index].StopAnimation();

            // Configure the Reward Popup Before Opening it 
            UIRewardPopup popup = PopupManager.Show(rewardPopup);
            popup.Configure(data.RewardMilestoneDatas[index].rewards);

            // Open the Reward Popup & Reward
            Save();

            rewarded?.Invoke(data.RewardMilestoneDatas[index].rewards);
        }

        private void PlaceItems()
        {
            float width = itemsParent.rect.width;
            float spacing = width / (itemsParent.childCount - 1);

            Vector2 startPosition = (Vector2)itemsParent.position - Vector2.right * width / 2;

            for (int i = 0; i < itemsParent.childCount; i++)
                itemsParent.GetChild(i).position = startPosition + spacing * i * Vector2.right;
        }

        private void InitSlider()
        {
            slider.minValue = 0;
            slider.maxValue = data.RewardMilestoneDatas[data.RewardMilestoneDatas.Length - 1].requiredXp;

            slider.value = 0;
        }

        private void CheckForRewards()
        {
            if (lastRewardIndex > data.RewardMilestoneDatas.Length - 1)
                return;

            if (slider.value >= data.RewardMilestoneDatas[lastRewardIndex].requiredXp)
                EnableReward();
        }

        private void EnableReward()
        {
            sliderItems[lastRewardIndex].Animate();
            
            lastRewardIndex++;
        }

        private void OnXpUpdated(int xp)
        {
            UpdateVisuals(xp);
            CheckForRewards();
        }

        private void UpdateVisuals(int xp)
        {
            slider.value = xp;
            xpText.text = xp.ToString();
        }

        private void AnimateUnopenedItems()
        {
            for (int i = 0; i < sliderItems.Count; i++)
                if (!itemsOpened[i] && slider.value >= data.RewardMilestoneDatas[i].requiredXp)
                    sliderItems[i].Animate();
        }

        private void Load()
        {
            if (Sijil.TryLoad(this, lastRewardIndexKey, out object _lastRewardIndex))
                lastRewardIndex = (int)_lastRewardIndex;

            if (Sijil.TryLoad(this, itemsOpenedKey, out object _itemsOpened))
                itemsOpened = (bool[])_itemsOpened;
        }

        private void Save()
        {
            Sijil.Save(this, lastRewardIndexKey, lastRewardIndex);
            Sijil.Save(this, itemsOpenedKey, itemsOpened);
        }

        private void ResetSelf()
        {
            Sijil.Remove(this, lastRewardIndexKey);
            Sijil.Remove(this, itemsOpenedKey);

            StartCoroutine("Start");
        }
    }
}