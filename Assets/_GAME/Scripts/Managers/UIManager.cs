using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;


public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;   

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
    [SerializeField] private TextMeshProUGUI loseArenaText;
    [SerializeField] private TextMeshProUGUI loseGoldText;
    [SerializeField] private TextMeshProUGUI loseBonusGoldText;
    [SerializeField] private TextMeshProUGUI loseEnemyCountText;

  

    private void Start()
    {
        GameUIStageChanged(UIGameStage.Menu);
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

        DataManager.instance.AddGold(rewardedGold);
        DataManager.instance.AddXP(5);

    }



    public void GameLoseButton()
    {
        GameUIStageChanged(UIGameStage.Menu);

        //towerController.ResetTower();
        gameManager.PowerUpReset();

        SceneManager.LoadScene(0);
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
        DOTween.To(() => 0, x => winGoldText.text = x.ToString(), 100 /*baseGold*/, 1f).SetDelay(1f);


        DataManager.instance.AddGold(/*totalGold*/ 100);
        DataManager.instance.AddXP(20);

    }

    public void GameWinButton()
    {
        //towerController.ResetTower();
        gameManager.PowerUpReset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);


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

}
public enum UIGameStage
{
    Menu,
    Game,
    GameWin,
    GameLose
}
