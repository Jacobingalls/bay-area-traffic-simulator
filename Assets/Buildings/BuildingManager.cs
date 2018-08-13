using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Density: byte { Rural, SubUrban, Urban }

public class BuildingManager : MonoBehaviour {

    [HideInInspector]
    public BuildingTile[,] tiles = new BuildingTile[75, 50];


    public void Initialize(TerrainManager terrainManager) {
        // Load in cities from a texture map. 

    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}