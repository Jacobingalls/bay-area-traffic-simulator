using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TileType {
	Grass
}

public class TerrainManager : MonoBehaviour {

	public int width = 10;

	public int height = 10;

	[Range(0.1f, 2.0f)]
	public float yScale = 1.0f / 1.2f;

	[Range(1, 20)]
	public int quantizationRange = 6;

	public Texture2D heightmap;
	public GameObject tilePrefab;
	public Material terrainMaterial;
	public TerrainData terrainData;

	private Tile[,] tiles;

	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;
	private Mesh mesh;
	
	private float maxHeight;

	int[] GetNeighborHeights(int row, int col, int[,] heights) {
		int[] neighbors = new int[4]; // E N W S

		int numRows = heights.GetLength(0);
		int numCols = heights.GetLength(1);

		int currentHeight = heights[row, col];

		neighbors[0] = currentHeight;
		neighbors[1] = currentHeight;
		neighbors[2] = currentHeight;
		neighbors[3] = currentHeight;

		if (col > 0 && row > 0 && heights[row - 1, col - 1] > currentHeight) { // North-West
			neighbors[0] = heights[row - 1, col - 1];
		}
		if (col < numCols - 1 && row > 0 && heights[row - 1, col + 1] > currentHeight) { // North-East
			neighbors[1] = heights[row - 1, col + 1];
		}
		if (row < numRows - 1 && col > 0 && heights[row + 1, col - 1] > currentHeight) { // South-West
			neighbors[2] = heights[row + 1, col - 1];
		}
		if (row < numRows - 1 && col < numCols - 1 && heights[row + 1, col + 1] > currentHeight) { // South-East
			neighbors[3] = heights[row + 1, col + 1];
		}

		return neighbors;
	}

	void OnValuesUpdates() {
		terrainData.ApplyToMaterial(terrainMaterial);
		terrainData.UpdateMeshHeights(terrainMaterial, 0.0f, maxHeight);
	}

	// Use this for initialization
	void Start () {
		maxHeight = quantizationRange * yScale;
		
		terrainData.OnValuesUpdates += OnValuesUpdates;
		terrainData.ApplyToMaterial(terrainMaterial);
		terrainData.UpdateMeshHeights(terrainMaterial, 0.0f, maxHeight);

		tiles = new Tile[height, width];
		// for (int row = 0; row < height; row++) {
		// 	for (int col = 0; col < width; col++) {
		// 		GameObject go = Instantiate(tilePrefab);
		// 				Debug.Log(heightmap.width);
		// 		tiles[row, col] = go.GetComponent<Tile>();
		// 		tiles[row, col].row = row;
		// 		tiles[row, col].col = col;
		// 		tiles[row, col].height = Mathf.RoundToInt(heightmap.GetPixel(col, row).r * 6.0f);
		// 		tiles[row, col].type = TileType.Grass;
		// 	}
		// }

		int[,] heights = new int[height, width];
		for (int row = 0; row < height; row++) {
			for (int col = 0; col < width; col++) {
				heights[row, col] = Mathf.RoundToInt(heightmap.GetPixel(col, row).r * 6.0f);
			}
		}

		meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = terrainMaterial;
		meshFilter = gameObject.AddComponent<MeshFilter>();
		mesh = meshFilter.mesh;

		mesh = new Mesh();
		
		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<int> indices = new List<int>();

		for (int row = 0; row < height; row++) {
			for (int col = 0; col < width; col++) {
				float height = Mathf.RoundToInt(heightmap.GetPixel(col, row).r * 6.0f) * yScale;
				vertices.Add(new Vector3(col, height, row));
				if (row % 2 == 0) {
					if (col % 2 == 0) {
						uvs.Add(new Vector2(0.0f, 0.0f));
					} else {
						uvs.Add(new Vector2(1.0f, 0.0f));
					}
				} else {
					if (col % 2 == 0) {
						uvs.Add(new Vector2(0.0f, 1.0f));
					} else {
						uvs.Add(new Vector2(1.0f, 1.0f));
					}
				}
			}
		}

		for (int row = 0; row < height - 1; row++) {
			for (int col = 0; col < width - 1; col++) {
				int topLeft = row * width + col;
				int topRight = row * width + col + 1;
				int bottomLeft = (row+1) * width + col;
				int bottomRight = (row+1) * width + col + 1;

				indices.Add(topLeft);
				indices.Add(bottomLeft);
				indices.Add(topRight);

				indices.Add(topRight);
				indices.Add(bottomLeft);
				indices.Add(bottomRight);
			}
		}

		mesh.vertices = vertices.ToArray();
		mesh.triangles = indices.ToArray();
		mesh.uv = uvs.ToArray();

		mesh.RecalculateNormals();

		meshFilter.mesh = mesh;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
