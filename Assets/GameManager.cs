﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class TimePeriod {
	private int hours;
	private int minutes;
}

public class GameManager : MonoBehaviour {
	public Light sun;
	public TerrainData terrainData;
	public RoadData roadData;

	public BuildingData BuildingData;

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

	private InputManager _inputManager;
	public InputManager InputManager {
		get {
			return _inputManager;
		}
	}

	private GUIManager _GUIManager;
	public GUIManager GUIManager {
		get {
			return _GUIManager;
		}
	}

    private BuildingManager _buildingManager;
    public BuildingManager BuildingManager {
        get {
            return _buildingManager;
        }
    }

	public const int HOURS_PER_DAY = 24; 
	public const int MINUTES_PER_HOUR = 60; 

	[Range(0, HOURS_PER_DAY - 1)]
	public float startingHour = 8;
	[Range(0, MINUTES_PER_HOUR - 1)]
	public float startingMinute = 0;
	private float timeOfDay = 0.0f; // from 0 to 1
    public float secondsPerDay = 5.0f * 60.0f;

	private float currentTime = 0.0f;

	// returns value between 0 and 1
	public float GetTimeOfDay() {
		return timeOfDay;
	}

	public int GetHour() {
		return (int)Mathf.Floor(timeOfDay * HOURS_PER_DAY);
	}

	public int GetMinute() {
		return (int)Mathf.Floor(((timeOfDay * HOURS_PER_DAY) - GetHour()) * MINUTES_PER_HOUR);
	}

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
		_inputManager = gameObject.AddComponent<InputManager>();
		_GUIManager = GameObject.Find("Canvas").GetComponent<GUIManager>();
		_buildingManager = gameObject.AddComponent<BuildingManager>();

		_terrainManager.data = terrainData;
		_roadManager.data = roadData;
		_buildingManager.data = BuildingData;

		_terrainManager.Initialize();
		_roadManager.Initialize(_terrainManager);
		_terrainManager.GenerateMeshes(_roadManager);
		_buildingManager.Initialize(_terrainManager);

		currentTime = secondsPerDay * ((startingHour/(float)HOURS_PER_DAY) + ((startingMinute / (float)MINUTES_PER_HOUR) / HOURS_PER_DAY));
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

	public static InputManager InputManagerInstance {
		get {
			if(_instance != null) {
				return _instance._inputManager;
			}
			return null;
		}
	}

	public static GUIManager GUIManagerInstance {
		get {
			if(_instance != null) {
				return _instance._GUIManager;
			}
			return null;
		}
	}

    public static BuildingManager BuildingManagerInstance {
        get { 
            if(_instance != null) {
                return _instance._buildingManager;
            }
            return null;
        }
    }
}
