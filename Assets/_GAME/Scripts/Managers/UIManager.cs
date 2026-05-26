using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameAnalyticsSDK;


public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private HeadquartersController headquartersController;
    [SerializeField] private LevelMapManager levelMapManager;

    [Header("Elements")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameWinPanel;
    [SerializeField] private GameObject gameLosePanel;
    [SerializeField] private GameObject menuBar;
    [Header("Level")]
    [SerializeField] private Transform enemyParent;
    [SerializeField] private Transform heroParent;
    [Header("Game Win/Lose Panel Settings")]
    [SerializeField] private TextMeshProUGUI winArenaText;
    [SerializeField] private TextMeshProUGUI winGoldText;
    [SerializeField] private TextMeshProUGUI winBonusGoldText;
    [SerializeField] private TextMeshProUGUI winEnemyCountText;
    [SerializeField] private TextMeshProUGUI winXPText;
    [SerializeField] private TextMeshProUGUI loseArenaText;
    [SerializeField] private TextMeshProUGUI loseGoldText;
    [SerializeField] private TextMeshProUGUI loseBonusGoldText;
    [SerializeField] private TextMeshProUGUI loseEnemyCountText;
    [SerializeField] private TextMeshProUGUI loseXPText;
    [Header("Game State")]
    public static Action gameOver;
    [Header("Quest Action")]
    public static Action gameWinCount;
    [Header("Chest Elements")]
    [SerializeField] private GameObject WoodenChestButtonAds;
    [SerializeField] private GameObject WoodenChestButtonFree;
    [SerializeField] private GameObject SilverChestButtonAds;
    [SerializeField] private GameObject SilverChestButtonFree;

    [SerializeField] InterstitialAdController interstitialAdController;
    [SerializeField] RewardedAdController rewardedAdController;
    private void Start()
    {
        GameUIStageChanged(UIGameStage.Menu);
    }

    private void Awake()
    {
        HeadquartersController.onGameLose += GameLosePanel;
    }
    private void OnDestroy()
    {
        HeadquartersController.onGameLose -= GameLosePanel;
    }
    public void GameLosePanel()
    {
        Time.timeScale = 1;
        GameUIStageChanged(UIGameStage.GameLose);

        gameLosePanel.transform.localScale = Vector3.zero;
        gameLosePanel.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);

        int waveIndex = PlayerPrefs.GetInt("WaveIndex", 0);
        loseArenaText.text = (waveIndex - 1).ToString();

        int enemyCount = GameManager.enemyCount;
        int rewardedGold = enemyCount * 5;

        loseEnemyCountText.text = "";
        loseBonusGoldText.text = "";
        loseGoldText.text = "";

        DOTween.To(() => 0, x => loseEnemyCountText.text = "Number of enemies killed: " + x.ToString(), enemyCount, 1f);
        DOTween.To(() => 0, x => loseBonusGoldText.text = x.ToString(), rewardedGold, 1f).SetDelay(0.5f);
        DOTween.To(() => 0, x => loseGoldText.text = x.ToString(), 0, 0.5f).SetDelay(1f);
        DOTween.To(() => 0, x => loseXPText.text = x.ToString(), 0, 0.5f).SetDelay(1f);


        DataManager.instance.AddGold(rewardedGold);
        DataManager.instance.AddXP(0);

        // GameAnalytics: Level Failed Event
        int playingEpisode = PlayerPrefs.GetInt("PlayingEpisode", 0);
        int playingLevel = PlayerPrefs.GetInt("PlayingLevel", 0);
        GameAnalytics.NewProgressionEvent(
            GAProgressionStatus.Fail,
            "Episode" + playingEpisode,
            "Level" + playingLevel
        );

        gameOver?.Invoke();

    }



    public void GameLoseButton()
    {
        StopWaveSystem();

        ClearAllEntities();

        ResetGameState();

        GameUIStageChanged(UIGameStage.Menu);

        headquartersController.ResetTower();

        gameManager.PowerUpReset();

        WoodenChestButtonAds.SetActive(true);
        WoodenChestButtonFree.SetActive(true);
        SilverChestButtonAds.SetActive(true);
        SilverChestButtonFree.SetActive(true);

        // İlk 2 seviyede reklam gösterme
        int playingLevel = PlayerPrefs.GetInt("PlayingLevel", 0);
        if (playingLevel > 2 && AdManager.Instance != null && AdManager.Instance.ShouldShowAds())
        {
            interstitialAdController.ShowInterstitialAd();
        }

    }

    public void GameWinPanel()
    {
        Time.timeScale = 1;
        GameUIStageChanged(UIGameStage.GameWin);
        gameWinPanel.transform.localScale = Vector3.zero;
        gameWinPanel.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);

        int waveIndex = PlayerPrefs.GetInt("WaveIndex", 0);
        winArenaText.text = waveIndex.ToString();

        int enemyCount = GameManager.enemyCount;
        int rewardedGold = enemyCount * 5;
        //int baseGold = gameManager.arenaWinReward[waveIndex];
        //int totalGold = rewardedGold + baseGold;

        winEnemyCountText.text = "";
        winBonusGoldText.text = "";
        winGoldText.text = "";

        DOTween.To(() => 0, x => winEnemyCountText.text = "Number of enemies killed: " + x.ToString(), enemyCount, 1f);
        DOTween.To(() => 0, x => winBonusGoldText.text = x.ToString(), rewardedGold, 1f).SetDelay(0.5f);
        DOTween.To(() => 0, x => winGoldText.text = x.ToString(), 100, 1f).SetDelay(1f);
        DOTween.To(() => 0, x => winXPText.text = x.ToString(), 10, 1f).SetDelay(1f);



        DataManager.instance.AddGold(/*totalGold*/ 100);
        DataManager.instance.AddXP(10);

        // GameAnalytics: Level Reward Resource Event
        int playingEpisode = PlayerPrefs.GetInt("PlayingEpisode", 0);
        int playingLevel = PlayerPrefs.GetInt("PlayingLevel", 0);
        GameAnalytics.NewResourceEvent(
            GAResourceFlowType.Source,
            "Gold",
            rewardedGold,
            "LevelReward",
            "Episode" + playingEpisode + "_Level" + playingLevel
        );

        gameOver?.Invoke();
        gameWinCount?.Invoke();

    }

    public void GameWinButton2X()
    {
        StopWaveSystem();

        ClearAllEntities();

        ResetGameState();

        GameUIStageChanged(UIGameStage.Menu);

        headquartersController.ResetTower();

        gameManager.PowerUpReset();

        DataManager.instance.AddGold(100);


        WoodenChestButtonAds.SetActive(true);
        WoodenChestButtonFree.SetActive(true);
        SilverChestButtonAds.SetActive(true);
        SilverChestButtonFree.SetActive(true);

        if (AdManager.Instance != null && AdManager.Instance.ShouldShowAds())
        {
            rewardedAdController.ShowRewardedAd();
        }


    }

    public void GameWinButton()
    {
        StopWaveSystem();

        ClearAllEntities();

        ResetGameState();

        GameUIStageChanged(UIGameStage.Menu);

        headquartersController.ResetTower();

        gameManager.PowerUpReset();


        WoodenChestButtonAds.SetActive(true);
        SilverChestButtonAds.SetActive(true);

        int playingLevel = PlayerPrefs.GetInt("PlayingLevel", 0);
        if (playingLevel > 2 && AdManager.Instance != null && AdManager.Instance.ShouldShowAds())
        {
            interstitialAdController.ShowInterstitialAd();
        }

        // SceneManager.LoadScene
    }

    public void GameUIStageChanged(UIGameStage stage)
    {
        switch (stage)
        {
            case UIGameStage.Menu:
                menuPanel.SetActive(true);
                gamePanel.SetActive(false);
                gameWinPanel.SetActive(false);
                gameLosePanel.SetActive(false);
                menuBar.SetActive(true);
                
                if (levelMapManager != null)
                {
                    levelMapManager.OpenLevelMap();
                }
                break;
            case UIGameStage.Game:
                menuPanel.SetActive(false);
                gamePanel.SetActive(true);
                gameWinPanel.SetActive(false);
                gameLosePanel.SetActive(false);
                menuBar.SetActive(false);
                PlacementController pc = FindObjectOfType<PlacementController>(true);
                if (pc != null)
                {
                    pc.RefreshPurchasedHeroes();
                }
                break;
            case UIGameStage.GameWin:
                menuPanel.SetActive(false);
                gamePanel.SetActive(false);
                gameWinPanel.SetActive(true);
                gameLosePanel.SetActive(false);


                break;
            case UIGameStage.GameLose:
                menuPanel.SetActive(false);
                gamePanel.SetActive(false);
                gameWinPanel.SetActive(false);
                gameLosePanel.SetActive(true);
                break;

            default:
                break;
        }

    }

    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
        panel.transform.localScale = Vector3.zero;  
        panel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);  
    }

    public void ClosePanel(GameObject panel)
    {
        panel.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => panel.SetActive(false));
    }
    public void DiscordLink()
    {
        Application.OpenURL("https://discord.gg/f2vDJdq6d5");
        if (!PlayerPrefs.HasKey("discordGift"))
        {
            DataManager.instance.AddGold(500);
            PlayerPrefs.SetInt("discordGift", 1);
        }
    }

    private void StopWaveSystem()
    {
        if (WaveManager.instance != null)
        {
            WaveManager.instance.CancelInvoke();
            
            WaveManager.instance.StopAllWaves();
        }
    }
    private async void ClearAllEntities()
    {
        if (enemyParent != null)
        {
            foreach (Transform child in enemyParent)
            {
                Destroy(child.gameObject);
            }
        }

        if (heroParent != null)
        {
            foreach (Transform child in heroParent)
            {
                Destroy(child.gameObject);
            }
        }

        if (EnemyBaseManager.instance != null)
        {
            await EnemyBaseManager.instance.UnloadCurrentLevel();
        }
    }


    private void ResetGameState()
    {
        Time.timeScale = 1f;

        GameManager.enemyCount = 0;

        DOTween.KillAll();
    }
}
public enum UIGameStage
{
    Menu,
    Game,
    GameWin,
    GameLose
}
