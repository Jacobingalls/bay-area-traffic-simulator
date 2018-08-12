using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TileType {
	Grass
}

public class TerrainManager : MonoBehaviour {

	public TerrainData data;

	private int[,] heights;

	public int[,] GetHeights() {
		return heights;
	}

	private Tile[,] tiles;

	private MeshRenderer roadMeshRenderer;
	private MeshFilter roadMeshFilter;
	private Mesh roadMesh;
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
		data.ApplyToMaterial(data.terrainMaterial);
		data.UpdateMeshHeights(data.terrainMaterial, 0.0f, maxHeight);
	}

	public void Initialize () {
		maxHeight = data.quantizationRange * data.yScale;
		
		data.OnValuesUpdates += OnValuesUpdates;
		data.ApplyToMaterial(data.terrainMaterial);
		data.UpdateMeshHeights(data.terrainMaterial, 0.0f, maxHeight);

		heights = new int[data.height, data.width];
		for (int row = 0; row < data.height; row++) {
			for (int col = 0; col < data.width; col++) {
				heights[row, col] = Mathf.RoundToInt(data.heightmap.GetPixel(col, row).r * 6.0f);
			}
		}
	}

	public void GenerateMeshes(RoadManager roadManager) {
		GenerateTerrainMesh(roadManager);
		GenerateRoadMesh(roadManager);
	}

	void GenerateTerrainMesh(RoadManager roadManager) {
		GameObject terrain = new GameObject();
		
		terrain.transform.name = "Terrain";
		terrain.transform.parent = transform;

		var meshRenderer = terrain.AddComponent<MeshRenderer>();
		meshRenderer.material = data.terrainMaterial;
		var meshFilter = terrain.AddComponent<MeshFilter>();
		var mesh = meshFilter.mesh;

		mesh = new Mesh();
		
		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<int> indices = new List<int>();

		for (int row = 0; row < data.height; row++) {
			for (int col = 0; col < data.width; col++) {
				float height = Mathf.RoundToInt(data.heightmap.GetPixel(col, row).r * 6.0f) * data.yScale;
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

		for (int row = 0; row < data.height - 1; row++) {
			for (int col = 0; col < data.width - 1; col++) {
				int topLeft = row * data.width + col;
				int topRight = row * data.width + col + 1;
				int bottomLeft = (row+1) * data.width + col;
				int bottomRight = (row+1) * data.width + col + 1;

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

	void GenerateRoadMesh(RoadManager roadManager) {
		GameObject roads = new GameObject();

		roads.transform.name = "Roads";
		roads.transform.parent = transform;

		var meshRenderer = roads.AddComponent<MeshRenderer>();
		meshRenderer.material = data.roadMaterial;
		var meshFilter = roads.AddComponent<MeshFilter>();
		var mesh = meshFilter.mesh;

		mesh = new Mesh();
		
		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<int> indices = new List<int>();

		int height = roadManager.tiles.GetLength(0);
		int width = roadManager.tiles.GetLength(1);
		int worldSpaceMultiplier = 4;

		int index = 0;

		var roadWidth = 0.150f;
		var roadWidthOverTwo = roadWidth / 2.0f;
		var topLeftCenterOffset = new Vector3(0.5f - roadWidthOverTwo, 0.0f, 0.5f + roadWidthOverTwo) * worldSpaceMultiplier;
		var topRightCenterOffset = new Vector3(0.5f + roadWidthOverTwo, 0.0f, 0.5f + roadWidthOverTwo) * worldSpaceMultiplier;
		var bottomLeftCenterOffset = new Vector3(0.5f - roadWidthOverTwo, 0.0f, 0.5f - roadWidthOverTwo) * worldSpaceMultiplier;
		var bottomRightCenterOffset = new Vector3(0.5f + roadWidthOverTwo, 0.0f, 0.5f - roadWidthOverTwo) * worldSpaceMultiplier;

		var topLeftWestOffset = new Vector3(0.0f, 0.0f, 0.5f + roadWidthOverTwo) * worldSpaceMultiplier;
		var topRightWestOffset = new Vector3(0.5f - roadWidthOverTwo, 0.0f, 0.5f + roadWidthOverTwo) * worldSpaceMultiplier;
		var bottomLeftWestOffset = new Vector3(0.0f, 0.0f, 0.5f - roadWidthOverTwo) * worldSpaceMultiplier;
		var bottomRightWestOffset = new Vector3(0.5f - roadWidthOverTwo, 0.0f, 0.5f - roadWidthOverTwo) * worldSpaceMultiplier;

		var topLeftEastOffset = new Vector3(0.5f + roadWidthOverTwo, 0.0f, 0.5f + roadWidthOverTwo) * worldSpaceMultiplier;
		var topRightEastOffset = new Vector3(1.0f, 0.0f, 0.5f + roadWidthOverTwo) * worldSpaceMultiplier;
		var bottomLeftEastOffset = new Vector3(0.5f + roadWidthOverTwo, 0.0f, 0.5f - roadWidthOverTwo) * worldSpaceMultiplier;
		var bottomRightEastOffset = new Vector3(1.0f, 0.0f, 0.5f - roadWidthOverTwo) * worldSpaceMultiplier;

		var topLeftNorthOffset = new Vector3(0.5f - roadWidthOverTwo, 0.0f, 1.0f) * worldSpaceMultiplier;
		var topRightNorthOffset = new Vector3(0.5f + roadWidthOverTwo, 0.0f, 1.0f) * worldSpaceMultiplier;
		var bottomLeftNorthOffset = new Vector3(0.5f - roadWidthOverTwo, 0.0f, 0.5f + roadWidthOverTwo) * worldSpaceMultiplier;
		var bottomRightNorthOffset = new Vector3(0.5f + roadWidthOverTwo, 0.0f, 0.5f + roadWidthOverTwo) * worldSpaceMultiplier;

		var topLeftSouthOffset = new Vector3(0.5f - roadWidthOverTwo, 0.0f, 0.5f - roadWidthOverTwo) * worldSpaceMultiplier;
		var topRightSouthOffset = new Vector3(0.5f + roadWidthOverTwo, 0.0f, 0.5f - roadWidthOverTwo) * worldSpaceMultiplier;
		var bottomLeftSouthOffset = new Vector3(0.5f - roadWidthOverTwo, 0.0f, 0.0f) * worldSpaceMultiplier;
		var bottomRightSouthOffset = new Vector3(0.5f + roadWidthOverTwo, 0.0f, 0.0f) * worldSpaceMultiplier;

		for (int row = 0; row < height; row++) {
			for (int col = 0; col < width; col++) {
				var tile = roadManager.tiles[row, col];
				if(!tile.Occupied) {
					continue;
				}

				var worldY = heights[row * worldSpaceMultiplier, col * worldSpaceMultiplier] * data.yScale + 0.05f;
				var worldX = col * worldSpaceMultiplier;
				var worldZ = row * worldSpaceMultiplier;

				var worldSpacePos = new Vector3(worldX, worldY, worldZ);

				// START Road Center
				vertices.Add(topLeftCenterOffset + worldSpacePos);
				vertices.Add(topRightCenterOffset + worldSpacePos);
				vertices.Add(bottomLeftCenterOffset + worldSpacePos);
				vertices.Add(bottomRightCenterOffset + worldSpacePos);

				indices.Add(index + 0);
				indices.Add(index + 1);
				indices.Add(index + 2);
				indices.Add(index + 1);
				indices.Add(index + 3);
				indices.Add(index + 2);

				index += 4;
				// END Road Center

				RoadTile t = tile;
				RoadTile n = null;
				foreach (var v in tile.getNeighbors()) {
					switch(v) {
					case DirectionOfTravel.Right:
						n = roadManager.tiles[row, col + 1];
						if((n.horizontalRoad == null || !n.horizontalRoad.up_left) && (t.horizontalRoad == null || !t.horizontalRoad.down_right)) {
							continue;
						}

						vertices.Add(topLeftEastOffset + worldSpacePos);
						vertices.Add(topRightEastOffset + worldSpacePos);
						vertices.Add(bottomLeftEastOffset + worldSpacePos);
						vertices.Add(bottomRightEastOffset + worldSpacePos);

						indices.Add(index + 0);
						indices.Add(index + 1);
						indices.Add(index + 2);
						indices.Add(index + 1);
						indices.Add(index + 3);
						indices.Add(index + 2);

						index += 4;
						break;
					case DirectionOfTravel.Left:
						n = roadManager.tiles[row, col - 1];
						if((n.horizontalRoad == null || !n.horizontalRoad.down_right) && (t.horizontalRoad == null || !t.horizontalRoad.up_left)) {
							continue;
						}

						vertices.Add(topLeftWestOffset + worldSpacePos);
						vertices.Add(topRightWestOffset + worldSpacePos);
						vertices.Add(bottomLeftWestOffset + worldSpacePos);
						vertices.Add(bottomRightWestOffset + worldSpacePos);

						indices.Add(index + 0);
						indices.Add(index + 1);
						indices.Add(index + 2);
						indices.Add(index + 1);
						indices.Add(index + 3);
						indices.Add(index + 2);

						index += 4;
						break;
					case DirectionOfTravel.Up:
						n = roadManager.tiles[row + 1, col];
						if((n.verticalRoad == null || !n.verticalRoad.down_right) && (t.verticalRoad == null || !t.verticalRoad.up_left)) {
							continue;
						}

						vertices.Add(topLeftNorthOffset + worldSpacePos);
						vertices.Add(topRightNorthOffset + worldSpacePos);
						vertices.Add(bottomLeftNorthOffset + worldSpacePos);
						vertices.Add(bottomRightNorthOffset + worldSpacePos);

						indices.Add(index + 0);
						indices.Add(index + 1);
						indices.Add(index + 2);
						indices.Add(index + 1);
						indices.Add(index + 3);
						indices.Add(index + 2);

						index += 4;
						break;
					case DirectionOfTravel.Down:
						n = roadManager.tiles[row - 1, col];
						if((n.verticalRoad == null || !n.verticalRoad.up_left) && (t.verticalRoad == null || !t.verticalRoad.down_right)) {
							continue;
						}

						vertices.Add(topLeftSouthOffset + worldSpacePos);
						vertices.Add(topRightSouthOffset + worldSpacePos);
						vertices.Add(bottomLeftSouthOffset + worldSpacePos);
						vertices.Add(bottomRightSouthOffset + worldSpacePos);

						indices.Add(index + 0);
						indices.Add(index + 1);
						indices.Add(index + 2);
						indices.Add(index + 1);
						indices.Add(index + 3);
						indices.Add(index + 2);

						index += 4;
						break;
					default:
						break;
					}
				}
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
