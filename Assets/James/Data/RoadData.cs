using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RoadData : UpdateableData {
    [Header("Road Manager State")]
	public bool enableCarSim;
    public bool drawDebug;
	public GameObject carModel;
    public Texture2D roadMap;

}
