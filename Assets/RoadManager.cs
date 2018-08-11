using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour {

    public GameObject prefab;

	// Use this for initialization
	void Start () {
        for (int y = 0; y < 10; y ++) {
            for (int x = 0; x < 10; x++)
            {
                var go = Instantiate(prefab);
                go.transform.position = new Vector3(x, y, 0.0f);
            } 
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
