using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Density: byte { Rural, SubUrban, Urban }
public enum BuildingType: byte { Residential, Commerce, Industrial}

public class BuildingManager : MonoBehaviour {

    [HideInInspector]
    public BuildingTile[,] tiles = new BuildingTile[75, 50];
	public BuildingData data;

    public void Initialize(TerrainManager terrainManager) {
        
		// Load in cities from a texture map. 
		for(var row = 0; row < tiles.GetLength(0); row ++) {
			for(var col = 0; col < tiles.GetLength(1); col ++) {
				var pxl = data.zoningMap.GetPixel(col, row);
				BuildingType type = BuildingType.Residential;
				if (pxl.r > 0) { // this is a residential tile
					type = BuildingType.Residential;
				} else if (pxl.g > 0) { // this is a commercial tile
					type = BuildingType.Commerce;
				} else if (pxl.b > 0) { // this is a industiral tile
					type = BuildingType.Industrial;
				} else {
					continue;
				}

				var density = (Density)((pxl.r + pxl.g + pxl.b) * 2);

				var tile = new BuildingTile();
				tile.buildingManager = this;
				tile.density = density;
				tile.type = type;
				tile.location = new Location();
				tile.location.row = row;
				tile.location.col = col;
				tile.height = terrainManager.GetWorldHeightAtLocation(tile.location);
				tiles[row, col] = tile;
			}
		}
    }

	// Use this for initialization
	void Start () {
		
	}
	
	bool makeBuildings = true;
	List<GameObject> buildings;

	// Update is called once per frame
	void Update () {
		if (makeBuildings) {
			buildings = new List<GameObject>();
			makeBuildings = false;
			for (var row = 0; row < tiles.GetLength(0); row ++) {
				for(var col = 0; col < tiles.GetLength(1); col++) {
					var tile = tiles[row, col];
					if (tile == null) {continue;}
					
					var tl_building = Instantiate(data.building);
					tl_building.transform.position = new Vector3(4 * col + 1f, tile.GetHeight(), 4 * row + 3f );
					tl_building.GetComponent<Renderer>().material = tile.GetMaterial();
					buildings.Add(tl_building);

					var tr_building = Instantiate(data.building);
					tr_building.transform.position = new Vector3(4 * col + 3f, tile.GetHeight(), 4 * row +3f );
					tr_building.GetComponent<Renderer>().material = tile.GetMaterial();
					buildings.Add(tr_building);

					var bl_building = Instantiate(data.building);
					bl_building.transform.position = new Vector3(4 * col + 1f, tile.GetHeight(), 4 * row +1f );
					bl_building.GetComponent<Renderer>().material = tile.GetMaterial();
					buildings.Add(bl_building);

					var br_building = Instantiate(data.building);
					br_building.transform.position = new Vector3(4 * col + 3f, tile.GetHeight(), 4 * row +1f );
					br_building.GetComponent<Renderer>().material = tile.GetMaterial();
					buildings.Add(br_building);

				}
			}
		}

		for (var row = 0; row < tiles.GetLength(0); row ++) {
			for(var col = 0; col < tiles.GetLength(1); col++) {
				var tile = tiles[row, col];
				if(tile != null) {
					tile.maybeMakeACar(buildings);
				}		
			}
		}
	}
}