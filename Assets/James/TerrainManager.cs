using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TileType {
	Grass
}

public class RoadOffsets {
	public Vector3 topLeftCenterOffset;
	public Vector3 topRightCenterOffset;
	public Vector3 bottomLeftCenterOffset;
	public Vector3 bottomRightCenterOffset;

	public Vector3 topLeftWestOffset;
	public Vector3 topRightWestOffset;
	public Vector3 bottomLeftWestOffset;
	public Vector3 bottomRightWestOffset;

	public Vector3 topLeftEastOffset;
	public Vector3 topRightEastOffset;
	public Vector3 bottomLeftEastOffset;
	public Vector3 bottomRightEastOffset;

	public Vector3 topLeftNorthOffset;
	public Vector3 topRightNorthOffset;
	public Vector3 bottomLeftNorthOffset;
	public Vector3 bottomRightNorthOffset;

	public Vector3 topLeftSouthOffset;
	public Vector3 topRightSouthOffset;
	public Vector3 bottomLeftSouthOffset;
	public Vector3 bottomRightSouthOffset;

	public Vector2 topLeftUv; 
	public Vector2 topRightUv; 
	public Vector2 bottomLeftUv; 
	public Vector2 bottomRightUv; 

	public RoadOffsets(float roadWidth, float worldSpaceMultiplier, int myRoadTier, int numRoadTiers) {
		float roadWidthOverTwo = roadWidth / 2.0f;

		topLeftCenterOffset = new Vector3(0.5f - roadWidthOverTwo, 0.0f, 0.5f + roadWidthOverTwo) * worldSpaceMultiplier;
		topRightCenterOffset = new Vector3(0.5f + roadWidthOverTwo, 0.0f, 0.5f + roadWidthOverTwo) * worldSpaceMultiplier;
		bottomLeftCenterOffset = new Vector3(0.5f - roadWidthOverTwo, 0.0f, 0.5f - roadWidthOverTwo) * worldSpaceMultiplier;
		bottomRightCenterOffset = new Vector3(0.5f + roadWidthOverTwo, 0.0f, 0.5f - roadWidthOverTwo) * worldSpaceMultiplier;

		topLeftWestOffset = new Vector3(0.250f, 0.0f, 0.5f + roadWidthOverTwo) * worldSpaceMultiplier;
		topRightWestOffset = new Vector3(0.5f - roadWidthOverTwo, 0.0f, 0.5f + roadWidthOverTwo) * worldSpaceMultiplier;
		bottomLeftWestOffset = new Vector3(0.250f, 0.0f, 0.5f - roadWidthOverTwo) * worldSpaceMultiplier;
		bottomRightWestOffset = new Vector3(0.5f - roadWidthOverTwo, 0.0f, 0.5f - roadWidthOverTwo) * worldSpaceMultiplier;

		topLeftEastOffset = new Vector3(0.5f + roadWidthOverTwo, 0.0f, 0.5f + roadWidthOverTwo) * worldSpaceMultiplier;
		topRightEastOffset = new Vector3(0.750f, 0.0f, 0.5f + roadWidthOverTwo) * worldSpaceMultiplier;
		bottomLeftEastOffset = new Vector3(0.5f + roadWidthOverTwo, 0.0f, 0.5f - roadWidthOverTwo) * worldSpaceMultiplier;
		bottomRightEastOffset = new Vector3(0.750f, 0.0f, 0.5f - roadWidthOverTwo) * worldSpaceMultiplier;

		topLeftNorthOffset = new Vector3(0.5f - roadWidthOverTwo, 0.0f, 0.750f) * worldSpaceMultiplier;
		topRightNorthOffset = new Vector3(0.5f + roadWidthOverTwo, 0.0f, 0.750f) * worldSpaceMultiplier;
		bottomLeftNorthOffset = new Vector3(0.5f - roadWidthOverTwo, 0.0f, 0.5f + roadWidthOverTwo) * worldSpaceMultiplier;
		bottomRightNorthOffset = new Vector3(0.5f + roadWidthOverTwo, 0.0f, 0.5f + roadWidthOverTwo) * worldSpaceMultiplier;

		topLeftSouthOffset = new Vector3(0.5f - roadWidthOverTwo, 0.0f, 0.5f - roadWidthOverTwo) * worldSpaceMultiplier;
		topRightSouthOffset = new Vector3(0.5f + roadWidthOverTwo, 0.0f, 0.5f - roadWidthOverTwo) * worldSpaceMultiplier;
		bottomLeftSouthOffset = new Vector3(0.5f - roadWidthOverTwo, 0.0f, 0.250f) * worldSpaceMultiplier;
		bottomRightSouthOffset = new Vector3(0.5f + roadWidthOverTwo, 0.0f, 0.250f) * worldSpaceMultiplier;

		topLeftUv = new Vector2((float)myRoadTier / numRoadTiers + 0.01f, ((float)myRoadTier+1) / numRoadTiers - 0.01f);
		topRightUv = new Vector2(((float)myRoadTier + 1) / numRoadTiers - 0.01f, ((float)myRoadTier + 1) / numRoadTiers - 0.01f);
		bottomLeftUv = new Vector2((float)myRoadTier / numRoadTiers + 0.01f, (float)myRoadTier / numRoadTiers + 0.01f);
		bottomRightUv = new Vector2(((float)myRoadTier + 1) / numRoadTiers - 0.01f, (float)myRoadTier / numRoadTiers - 0.01f);
	}
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
	private GameObject roads;
	private GameObject terrain;

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
		if(terrain != null) {
			Destroy(terrain);
		} 
		terrain = new GameObject();
		
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

	int addIndices(List<int> indices, int index) {
		indices.Add(index + 0);
		indices.Add(index + 1);
		indices.Add(index + 2);
		indices.Add(index + 1);
		indices.Add(index + 3);
		indices.Add(index + 2);
		return index + 4;
	}

	public void RegenerateRoadMeshForTest(RoadManager roadManager) {
		GenerateRoadMesh(roadManager);
	}

	void GenerateRoadMesh(RoadManager roadManager) {
		if(roads != null) {
			Destroy(roads);
		} 
		roads = new GameObject();

		roads.transform.name = "Roads";
		roads.transform.parent = transform;

		var meshRenderer = roads.AddComponent<MeshRenderer>();
		meshRenderer.material = data.roadMaterial;
		meshRenderer.receiveShadows = false;
		meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
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

		var roadWidth = 0.40f;
		var roadWidthOverTwo = roadWidth / 2.0f;
		// roads own their top and right corners for sloping behavior
   
		var northTopOffset = new Vector3(0.0f, 0.0f, .250f) * worldSpaceMultiplier;
		var eastRightOffset = new Vector3(0.250f, 0.0f, 0.0f) * worldSpaceMultiplier;
		var westLeftOffset = new Vector3(-0.250f, 0.0f, 0.0f) * worldSpaceMultiplier;
		var southBottomOffset = new Vector3(0.0f, 0.0f, -.250f) * worldSpaceMultiplier;

		RoadOffsets[] offsets = {
			new RoadOffsets(0.1f, worldSpaceMultiplier, 0, 3), // hardcoded magic numbers. 
			new RoadOffsets(0.15f, worldSpaceMultiplier, 1, 3), // WHAT ARE YOU GONNA DO ABOUT IT?
			new RoadOffsets(0.2f, worldSpaceMultiplier, 2, 3), // huh? what? yeah, thought so, chump.
		};

		Vector3 verticalOffset = Vector3.zero;

		Dictionary<RoadTile, int[]> indexLookup = new Dictionary<RoadTile, int[]>();

		var elevationDelta = data.yScale;

		//FIXME: There are discontinuities in the slope normals. 
		//		IDEA: Two pass solution. Draw internal segments first, then in second pass work backwards
		//			(from the highest row and column) to generate the segments, stitching as appropriate.
		//			Use hash table to lookup the start vertex of neighboring vertices
		const int DirectionOfTravelCenter = 4;

		for (int row = 0; row < height; row++) {
			for (int col = 0; col < width; col++) {
				var tile = roadManager.tiles[row, col];

				indexLookup[tile] = new int[5];

				if(!tile.Occupied) {
					indexLookup[tile][(int)DirectionOfTravel.Left] = -1;
					indexLookup[tile][(int)DirectionOfTravel.Right] = -1;
					indexLookup[tile][(int)DirectionOfTravel.Up] = -1;
					indexLookup[tile][(int)DirectionOfTravel.Down] = -1;
					indexLookup[tile][(int)DirectionOfTravelCenter] = -1;
					continue;
				}

				var worldY = heights[row * worldSpaceMultiplier, col * worldSpaceMultiplier] * data.yScale + 0.05f;
				var worldX = col * worldSpaceMultiplier;
				var worldZ = row * worldSpaceMultiplier;

				var worldSpacePos = new Vector3(worldX, worldY, worldZ);

				var roadSizeHorizontal = tile.horizontalRoad != null ? (int)tile.horizontalRoad.size : -1;
				var roadSizeVertical = tile.verticalRoad != null ? (int)tile.verticalRoad.size : -1;
				var roadSizeLargest = roadSizeHorizontal > roadSizeVertical ? roadSizeHorizontal : roadSizeVertical;

				int roadSizeNeighbor = -1;
				int roadSizeToUse = -1;

				// START Road Center
				indexLookup[tile][(int)DirectionOfTravelCenter] = index;
				vertices.Add(offsets[roadSizeLargest].topLeftCenterOffset + worldSpacePos);
				vertices.Add(offsets[roadSizeLargest].topRightCenterOffset + worldSpacePos);
				vertices.Add(offsets[roadSizeLargest].bottomLeftCenterOffset + worldSpacePos);
				vertices.Add(offsets[roadSizeLargest].bottomRightCenterOffset + worldSpacePos);
				uvs.Add(offsets[roadSizeLargest].topLeftUv);
				uvs.Add(offsets[roadSizeLargest].topRightUv);
				uvs.Add(offsets[roadSizeLargest].bottomLeftUv);
				uvs.Add(offsets[roadSizeLargest].bottomRightUv);

				index = addIndices(indices, index);
				// END Road Center

				RoadTile t = tile;
				RoadTile n = null;
				int curHeight = heights[row * worldSpaceMultiplier, col * worldSpaceMultiplier];
				var center = -1;
				foreach (var v in tile.getNeighbors()) {
					switch(v) {
					case DirectionOfTravel.Right:
						n = roadManager.tiles[row, col + 1];
						if((n.horizontalRoad == null || !n.horizontalRoad.up_left) && (t.horizontalRoad == null || !t.horizontalRoad.down_right)) {
							continue;
						}

						roadSizeToUse = roadSizeHorizontal;
						if (n.horizontalRoad != null) {
							roadSizeNeighbor = (int)n.horizontalRoad.size;
							roadSizeToUse = roadSizeNeighbor < roadSizeHorizontal? roadSizeNeighbor : roadSizeHorizontal;
						}

						// Add inner segment
						indexLookup[tile][(int)DirectionOfTravel.Right] = index;
						vertices.Add(offsets[roadSizeToUse].topRightEastOffset + worldSpacePos);
						vertices.Add(offsets[roadSizeToUse].bottomRightEastOffset + worldSpacePos);
						uvs.Add(offsets[roadSizeToUse].topLeftUv);
						uvs.Add(offsets[roadSizeToUse].bottomLeftUv);
						center = indexLookup[tile][(int)DirectionOfTravelCenter];
						indices.Add(center + 1);
						indices.Add(index + 0);
						indices.Add(center + 3);
						indices.Add(index + 0);
						indices.Add(index + 1);
						indices.Add(center + 3);
						index += 2;
						break;
					case DirectionOfTravel.Left:
						n = roadManager.tiles[row, col - 1];
						if((n.horizontalRoad == null || !n.horizontalRoad.down_right) && (t.horizontalRoad == null || !t.horizontalRoad.up_left)) {
							continue;
						}
						
						roadSizeToUse = roadSizeHorizontal;
						if (n.horizontalRoad != null) {
							roadSizeNeighbor = (int)n.horizontalRoad.size;
							roadSizeToUse = roadSizeNeighbor < roadSizeHorizontal? roadSizeNeighbor : roadSizeHorizontal;
						}

						// Add inner segment
						indexLookup[tile][(int)DirectionOfTravel.Left] = index;
						vertices.Add(offsets[roadSizeToUse].topLeftWestOffset + worldSpacePos);
						vertices.Add(offsets[roadSizeToUse].bottomLeftWestOffset + worldSpacePos);
						uvs.Add(offsets[roadSizeToUse].topRightUv);
						uvs.Add(offsets[roadSizeToUse].bottomRightUv);
						center = indexLookup[tile][(int)DirectionOfTravelCenter];
						indices.Add(index + 0);
						indices.Add(center + 0);
						indices.Add(index + 1);
						indices.Add(center + 0);
						indices.Add(center + 2);
						indices.Add(index + 1);
						index += 2;
						break;
					case DirectionOfTravel.Up:
						n = roadManager.tiles[row + 1, col];

						if((n.verticalRoad == null || !n.verticalRoad.down_right) && (t.verticalRoad == null || !t.verticalRoad.up_left)) {
							continue;
						}

						roadSizeToUse = roadSizeVertical;
						if (n.verticalRoad != null) {
							roadSizeNeighbor = (int)n.verticalRoad.size;
							roadSizeToUse = roadSizeNeighbor < roadSizeVertical? roadSizeNeighbor : roadSizeVertical;
						}

						// Add inner segment
						indexLookup[tile][(int)DirectionOfTravel.Up] = index;
						vertices.Add(offsets[roadSizeToUse].topLeftNorthOffset + worldSpacePos);
						vertices.Add(offsets[roadSizeToUse].topRightNorthOffset + worldSpacePos);
						uvs.Add(offsets[roadSizeToUse].bottomLeftUv);
						uvs.Add(offsets[roadSizeToUse].bottomRightUv);
						center = indexLookup[tile][(int)DirectionOfTravelCenter];
						indices.Add(index + 0);
						indices.Add(index + 1);
						indices.Add(center + 0);
						indices.Add(index + 1);
						indices.Add(center + 1);
						indices.Add(center + 0);
						index += 2;
						break;
					case DirectionOfTravel.Down:
						n = roadManager.tiles[row - 1, col];
						if((n.verticalRoad == null || !n.verticalRoad.up_left) && (t.verticalRoad == null || !t.verticalRoad.down_right)) {
							continue;
						}

						roadSizeToUse = roadSizeVertical;
						if (n.verticalRoad != null) {
							roadSizeNeighbor = (int)n.verticalRoad.size;
							roadSizeToUse = roadSizeNeighbor < roadSizeVertical? roadSizeNeighbor : roadSizeVertical;
						}

						// Add inner segment
						indexLookup[tile][(int)DirectionOfTravel.Down] = index;
						vertices.Add(offsets[roadSizeToUse].bottomLeftSouthOffset + worldSpacePos);
						vertices.Add(offsets[roadSizeToUse].bottomRightSouthOffset + worldSpacePos);
						uvs.Add(offsets[roadSizeToUse].topLeftUv);
						uvs.Add(offsets[roadSizeToUse].bottomRightUv);
						center = indexLookup[tile][(int)DirectionOfTravelCenter];
						indices.Add(center + 2);
						indices.Add(center + 3);
						indices.Add(index + 0);
						indices.Add(center + 3);
						indices.Add(index + 1);
						indices.Add(index + 0);
						index += 2;
						break;
					default:
						break;
					}
				}
			}
		}

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

				var roadSizeHorizontal = tile.horizontalRoad != null ? (int)tile.horizontalRoad.size : -1;
				var roadSizeVertical = tile.verticalRoad != null ? (int)tile.verticalRoad.size : -1;
				var roadSizeLargest = roadSizeHorizontal > roadSizeVertical ? roadSizeHorizontal : roadSizeVertical;

				int roadSizeNeighbor = -1;
				int roadSizeToUse = -1;

				RoadTile t = tile;
				RoadTile n = null;
				int nh = -1;
				int curHeight = heights[row * worldSpaceMultiplier, col * worldSpaceMultiplier];
				float nhWorldSpace = 0.0f;
				int neighborIndex = -1;
				int myIndex = -1;
				foreach (var v in tile.getNeighbors()) {
					switch(v) {
					case DirectionOfTravel.Right:
						n = roadManager.tiles[row, col + 1];
						if((n.horizontalRoad == null || !n.horizontalRoad.up_left) && (t.horizontalRoad == null || !t.horizontalRoad.down_right)) {
							continue;
						}


						// Add outer segment (potentially sloped)
						nh = heights[row * worldSpaceMultiplier, (col + 1) * worldSpaceMultiplier];
						nhWorldSpace = heights[row * worldSpaceMultiplier, (col + 1) * worldSpaceMultiplier] * data.yScale + 0.05f;
						if (nh == curHeight) {
							verticalOffset = Vector3.zero;
						} else if (nh < curHeight) {
							verticalOffset = new Vector3(0.0f, -elevationDelta, 0.0f);
						} else {
							verticalOffset = new Vector3(0.0f, elevationDelta, 0.0f);
						}

						myIndex = indexLookup[tile][(int)DirectionOfTravel.Right];
						indexLookup[tile][(int)DirectionOfTravel.Right] = index;
						
						roadSizeToUse = roadSizeHorizontal;
						if (n.horizontalRoad != null) {
							roadSizeNeighbor = (int)n.horizontalRoad.size;
							roadSizeToUse = roadSizeNeighbor < roadSizeHorizontal? roadSizeNeighbor : roadSizeHorizontal;
						}

						vertices.Add(offsets[roadSizeToUse].topRightEastOffset + worldSpacePos + eastRightOffset + verticalOffset);
						vertices.Add(offsets[roadSizeToUse].bottomRightEastOffset + worldSpacePos + eastRightOffset + verticalOffset);
						uvs.Add(offsets[roadSizeToUse].topLeftUv);
						uvs.Add(offsets[roadSizeToUse].bottomLeftUv);
						indices.Add(myIndex + 0);
						indices.Add(index + 0);
						indices.Add(myIndex + 1);
						indices.Add(index + 0);
						indices.Add(index + 1);
						indices.Add(myIndex + 1);
						index += 2;

						break;
					case DirectionOfTravel.Left:
						n = roadManager.tiles[row, col - 1];
						if((n.horizontalRoad == null || !n.horizontalRoad.down_right) 
						&& (t.horizontalRoad == null || !t.horizontalRoad.up_left)) {
							continue;
						}

						// Add outer segment (NOT sloped)
						neighborIndex = indexLookup[n][(int)DirectionOfTravel.Right];
						myIndex = indexLookup[tile][(int)DirectionOfTravel.Left];

						if(neighborIndex == -1) {
							continue;
						}

						indices.Add(neighborIndex + 0);
						indices.Add(myIndex + 0);
						indices.Add(neighborIndex + 1);
						indices.Add(neighborIndex + 1);
						indices.Add(myIndex + 0);
						indices.Add(myIndex + 1);

						break;
					case DirectionOfTravel.Up:
						n = roadManager.tiles[row + 1, col];

						if((n.verticalRoad == null || !n.verticalRoad.down_right) && (t.verticalRoad == null || !t.verticalRoad.up_left)) {
							continue;
						}

						// Add outer segment (potentially sloped)
						nh = heights[(row + 1) * worldSpaceMultiplier, col * worldSpaceMultiplier];
						nhWorldSpace = heights[(row + 1) * worldSpaceMultiplier, col * worldSpaceMultiplier] * data.yScale + 0.05f;
						if (nh == curHeight) {
							verticalOffset = Vector3.zero;
						} else if (nh < curHeight) {
							verticalOffset = new Vector3(0.0f, -elevationDelta, 0.0f);
						} else {
							verticalOffset = new Vector3(0.0f, elevationDelta, 0.0f);
						}

						myIndex = indexLookup[tile][(int)DirectionOfTravel.Up];
						indexLookup[tile][(int)DirectionOfTravel.Up] = index;

						roadSizeToUse = roadSizeVertical;
						if (n.verticalRoad != null) {
							roadSizeNeighbor = (int)n.verticalRoad.size;
							roadSizeToUse = roadSizeNeighbor < roadSizeVertical? roadSizeNeighbor : roadSizeVertical;
						}

						vertices.Add(offsets[roadSizeToUse].topLeftNorthOffset + worldSpacePos + northTopOffset + verticalOffset);
						vertices.Add(offsets[roadSizeToUse].topRightNorthOffset + worldSpacePos + northTopOffset + verticalOffset);
						uvs.Add(offsets[roadSizeToUse].topLeftUv);
						uvs.Add(offsets[roadSizeToUse].topRightUv);
						indices.Add(index + 0);
						indices.Add(index + 1);
						indices.Add(myIndex + 0);
						indices.Add(index + 1);
						indices.Add(myIndex + 1);
						indices.Add(myIndex + 0);
						index += 2;

						break;
					case DirectionOfTravel.Down:
						n = roadManager.tiles[row - 1, col];
						if((n.verticalRoad == null || !n.verticalRoad.up_left) && (t.verticalRoad == null || !t.verticalRoad.down_right)) {
							continue;
						}

						neighborIndex = indexLookup[n][(int)DirectionOfTravel.Up];
						myIndex = indexLookup[tile][(int)DirectionOfTravel.Down];

						if(neighborIndex == -1) {
							continue;
						}

						// Add outer segment (NOT sloped)
						indices.Add(myIndex + 0);
						indices.Add(myIndex + 1);
						indices.Add(neighborIndex + 0);
						indices.Add(myIndex + 1);
						indices.Add(neighborIndex + 1);
						indices.Add(neighborIndex + 0);
						
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
	
	public float GetWorldHeightAtLocation(Location loc) {
		return heights[loc.row * 4, loc.col * 4] * data.yScale;
	}

	public Vector3 LerpWorldSpacePositionBetweenLocations(Location start, Location end, float pct) {
		float startHeight = heights[start.row * 4, start.col * 4] * data.yScale;
		float endHeight = heights[end.row * 4, end.col * 4] * data.yScale;
		float height;
		if(start.col < end.col || start.row < end.row) {
			if(pct <= 0.25f) {
				height = startHeight;
			} else if (pct >= 0.50f) {
				height = endHeight;
			} else {
				height = Mathf.Lerp(startHeight, endHeight, (pct - 0.25f) / 0.25f);
			}
		} else {
			if(pct <= 0.5f) {
				height = startHeight;
			} else if (pct >= 0.75f) {
				height = endHeight;
			} else {
				height = Mathf.Lerp(startHeight, endHeight, (pct - 0.5f) / 0.25f);
			}
		}


		return Vector3.Lerp(ToWorldSpace(start, height), ToWorldSpace(end, height), pct);
	}

	public Vector3 ToWorldSpace(int row, int col, float height)
    {
        return new Vector3((col * 4) + 2, height, (row * 4) + 2);
    }

    public Vector3 ToWorldSpace(Location loc, float height)
    {
        return ToWorldSpace(loc.row, loc.col, height);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
