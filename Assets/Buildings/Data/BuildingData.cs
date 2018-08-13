using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BuildingData : UpdateableData
{
    [Header("Building Manager State")]

    public GameObject building;
    public Material lowDensityResidential, mediumDensityResidential, highDensityResidential;
    public Material lowDensityCommercial, mediumDensityCommercial, highDensityCommercial;
    public Material lowDensityIndustrial, mediumDensityIndustrial, highDensityIndustrial;

    public Texture2D zoningMap;


    // The list of models which should be used for residential buildings.
    public GameObject[,] residentialBuildings;

    // The list of models that should be used for commercial buildings.
    public GameObject[,] commercialBuildings;

    // The list of models that should be used for industrial buildings.
    public GameObject[,] industrialBuildings;

    // A handy value for scaling building models up or down. 
    public float globalBuildingScale = 1.0f;
}
