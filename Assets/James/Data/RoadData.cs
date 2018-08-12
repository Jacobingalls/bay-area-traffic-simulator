using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RoadData : UpdateableData {
    [Header("Road Manager State")]
	public bool enableCarSim;
	public GameObject carModel;
}
