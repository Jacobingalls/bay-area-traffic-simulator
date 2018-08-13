using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RoadData : UpdateableData {
    [Header("Road Manager State")]
	public bool enableCarSim;
    public bool simpleCarSim;
    public bool drawDebug;
    public bool debugCarTraffic;
	public GameObject carModel;
    public Texture2D roadMap;
    public Material red, green, yellow;
    public Material[] carColors;

}
