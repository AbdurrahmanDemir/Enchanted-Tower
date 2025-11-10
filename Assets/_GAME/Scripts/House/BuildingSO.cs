using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Creat Building")]

public class BuildingSO : ScriptableObject
{
    public string houseName;
    public Sprite houseImage;
    public BuildingType type;
    public float productionTime;

}
public enum BuildingType
{
    ResourceBuilding,
    MilitaryBuilding,
    DefenseBuilding

}
