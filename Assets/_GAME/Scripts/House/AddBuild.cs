using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddBuild : MonoBehaviour
{
    [SerializeField] private GameObject[] buildingPrefabs;
    [SerializeField] private GameObject addBuildingPanel;
    Transform emptyArea;
    Button emptyAreaButton;

    public void AddBuilding(GameObject build)
    {
        Instantiate(build,emptyArea.position,Quaternion.identity);
        ClosePanel(addBuildingPanel);
    }

    public void EmptyArea(Transform emptyArea)
    {
        OpenPanel(addBuildingPanel);
        this.emptyArea = emptyArea;
        GameObject clickedObj = EventSystem.current.currentSelectedGameObject;
        emptyAreaButton = clickedObj.GetComponent<Button>();
        emptyAreaButton.gameObject.SetActive(false);

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
[System.Serializable]
public struct BuildingInfo
{
    public string buildingName;
    public GameObject buildingPrefabs;
    public int buildingFee;

}
