using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

	public Text timeOfDayText;

	private GameManager gameManager;

	// Use this for initialization
	void Start () {
		gameManager = GameManager.Instance;
	}

	// Update is called once per frame
	void Update () {
		int hour = gameManager.GetHour();
		string ampmString = "am";
		if(hour > GameManager.HOURS_PER_DAY / 2) {
			hour -= GameManager.HOURS_PER_DAY / 2;
			ampmString = "pm";
		}
		timeOfDayText.text = string.Format("{0}:{1}{2}", hour.ToString("D2"), gameManager.GetMinute().ToString("D2"), ampmString);
	}
}
