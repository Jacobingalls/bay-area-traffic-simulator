using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BuildingTile {

}

public class Special {

}

public enum WaterLevel {
    NO_THREAT,
    IS_IT_RAINING,
    SLIME,
    FLOODED
} 

public class Cell {
    public RoadTile road;
    public BuildingTile building;
    public Special special;
    public Location location;
    public WaterLevel waterLevel;
}

public class GameManager : MonoBehaviour {
	public Light sun;
	public TerrainData terrainData;
	public RoadData roadData;

	private TerrainManager _terrainManager;
	public TerrainManager TerrainManager {
		get {
			return _terrainManager;
		}
	}

	private RoadManager _roadManager;
	public RoadManager RoadManager {
		get {
			return _roadManager;
		}
	}

	public float timeOfDay = 0.0f; // from 0 to 1
    public float secondsPerDay = 5.0f * 60.0f;

	private float currentTime = 0.0f;

	void Awake()
	{
		if (_instance == null) {
			_instance = this;
		} else if (_instance != this) {
			Destroy(gameObject);    
		}
		DontDestroyOnLoad(gameObject);
	}

	void Start () {
		_terrainManager = gameObject.AddComponent<TerrainManager>();
		_roadManager = gameObject.AddComponent<RoadManager>();

		_terrainManager.data = terrainData;
		_roadManager.data = roadData;

		_terrainManager.Initialize();
		_roadManager.Initialize(_terrainManager);
		_terrainManager.GenerateMeshes(_roadManager);
	}
	
	void Update () {
		currentTime += Time.deltaTime;
		if(currentTime > secondsPerDay) {
			currentTime -= secondsPerDay;
		}
		timeOfDay = currentTime / secondsPerDay;
		sun.gameObject.transform.rotation = Quaternion.Euler(timeOfDay * 360.0f - 90.0f, -30.0f, 0.0f);
	}

	private static GameManager _instance = null;  
	public static GameManager Instance {
		get {
			return _instance;
		}
	}

	public static TerrainManager TerrainManagerInstance {
		get {
			if(_instance != null) {
				return _instance._terrainManager;
			}
			return null;
		}
	}
	public static RoadManager RoadManagerInstance {
		get {
			if(_instance != null) {
				return _instance._roadManager;
			}
			return null;
		}
	}
}
