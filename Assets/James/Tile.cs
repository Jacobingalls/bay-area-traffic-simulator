using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public int row;
	public int col;
	public TileType type;
	public int height;

	// Use this for initialization
	void Start () {
		transform.position = new Vector3(col, height / 2.0f, row);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
