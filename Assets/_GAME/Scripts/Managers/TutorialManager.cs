using DG.Tweening;
using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [Header("Elements")]
    [SerializeField] private GameObject tutorialPanel1;
    [SerializeField] private GameObject tutorialPanel2;
    [SerializeField] private GameObject tutorialPanel3;
    [SerializeField] private GameObject tutorialPanel4;
    [SerializeField] private GameObject tutorialPanel5;
    [SerializeField] private GameObject tutorialPanel6;
    [SerializeField] private GameObject tutorialPanel7;
    [SerializeField] private GameObject tutorialPanel8;
    [SerializeField] private GameObject tutorialPanel9;

    public bool finishTutorial;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (!PlayerPrefs.HasKey("Tutorial"))
        {
            OpenPanel(tutorialPanel1);
        }
        else
        {
            tutorialPanel1.SetActive(false);
            tutorialPanel2.SetActive(false);
            tutorialPanel3.SetActive(false);
            tutorialPanel4.SetActive(false);
            tutorialPanel5.SetActive(false);
            tutorialPanel6.SetActive(false);
            finishTutorial = true;
        }

        if (PlayerPrefs.HasKey("Tutorial") && !PlayerPrefs.HasKey("UpgradeTutorial"))
        {
            OpenPanel(tutorialPanel7);
        }

    }

    public void TutorailPanel2()
    {
        ClosePanel(tutorialPanel1);
        OpenPanel(tutorialPanel2);
    }

    public void TutorailPanel3()
    {
        ClosePanel(tutorialPanel2);
        OpenPanel(tutorialPanel3);
    }

    public void TutorailPanel4()
    {
        ClosePanel(tutorialPanel3);
        OpenPanel(tutorialPanel4);
        
    }
    public void TutorailPanel5()
    {
        ClosePanel(tutorialPanel4);
        OpenPanel(tutorialPanel5);
    }
    public void TutorailPanel6()
    {
        ClosePanel(tutorialPanel5);
        PlayerPrefs.SetInt("Tutorial", 1);
        finishTutorial = true;
        StartCoroutine(TutorialPanel6());
    }

    //public void TutorailPanelOff()
    //{
    //    //ClosePanel(tutorialPanel4);
    //    PlayerPrefs.SetInt("Tutorial", 1);
    //}
    IEnumerator TutorialPanel6()
    {
        OpenPanel(tutorialPanel6);
        yield return new WaitForSeconds(2f);
        ClosePanel(tutorialPanel6);
    }

    public void TutorailPanel7()
    {
        ClosePanel(tutorialPanel7);
        OpenPanel(tutorialPanel8);
    }
    public void TutorailPanel8()
    {
        ClosePanel(tutorialPanel8);
        OpenPanel(tutorialPanel9);
    }
    public void TutorailPanel9()
    {
        ClosePanel(tutorialPanel9);
        PlayerPrefs.SetInt("UpgradeTutorial", 1);

    }
    public bool GetTutorialState()
    {
        return finishTutorial;
    }

    private void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
        panel.transform.localScale = Vector3.zero;  // Ýlk baþta küçültülmüþ halde
        panel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);  // Yumuþak bir þekilde büyüme efekti
    }

    private void ClosePanel(GameObject panel)
    {
        panel.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => panel.SetActive(false));
    }
}
