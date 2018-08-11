using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Size : byte { Small, Medium, Large }
public enum Density : byte { Rural, SubUrban, Urban }
public enum TitleType: byte { Nature, Water, Housing, Commerce, Offices, Industrial}

// Road is an edge, it has a start, end and size. They can be unidirectional or bidirectional
public class Road {
    public Size size;
    public bool up_left;    // True if road goes up or left
    public bool down_right; // True if road goes down or right


    // Returns an inverted version of the size. Where the heigher the size of the road, the lower the cost. Yay.
    public int cost() {
        switch (size) {
            case Size.Medium:
                return 3;
            case Size.Large:
                return 1;
            default:
                return 5; // Small and invalid
        }
    }
}


public enum DirectionOfTravel: byte {Up, Down, Left, Right}

// Tile is a cell on the game board. Roads interconnect tiles
public class RoadTile : MonoBehaviour {
    // Where it is on the map
    public Location location;

    // The roads that are cutting across
    public Road verticalRoad;
    public Road horizontalRoad;

    // Denote if there should be powerlines drawn instead of roads
    public bool verticalPower;
    public bool horizontalPower;

    // What type of tile is this, how dense?
    public TitleType type;
    public Density density;


	public void Update() {
        var pos = gameObject.transform.position;
                            
        // Vertical Road Debug
        if (verticalRoad != null) {
            
            if (verticalRoad.up_left) { Debug.DrawLine(new Vector3(pos.x, pos.y + 1, pos.z), new Vector3(pos.x, pos.y + 1, pos.z + 0.5f), Color.red); }
            if (verticalRoad.down_right) { Debug.DrawLine(new Vector3(pos.x, pos.y + 1, pos.z), new Vector3(pos.x, pos.y + 1, pos.z - 0.5f), Color.black); }
        }

        // Horizontal Road Debug
        if (verticalRoad != null)
        {
            if (horizontalRoad.up_left) { Debug.DrawLine(new Vector3(pos.x, pos.y + 1, pos.z), new Vector3(pos.x + 0.5f, pos.y + 1, pos.z), Color.blue); }
            if (horizontalRoad.down_right) { Debug.DrawLine(new Vector3(pos.x, pos.y + 1, pos.z), new Vector3(pos.x - 0.5f, pos.y + 1, pos.z), Color.white); }
        }
	}

    public List<DirectionOfTravel> getNeighbors() {
        var list = new List<DirectionOfTravel>();
        if (getNeighborRoadTile(DirectionOfTravel.Up) != null) { list.Add(DirectionOfTravel.Up); }
        if (getNeighborRoadTile(DirectionOfTravel.Down) != null) { list.Add(DirectionOfTravel.Down); }
        if (getNeighborRoadTile(DirectionOfTravel.Left) != null) { list.Add(DirectionOfTravel.Left); }
        if (getNeighborRoadTile(DirectionOfTravel.Right) != null) { list.Add(DirectionOfTravel.Right); }

        return list;
    }

	/*
     * We want to get a RoadTile relative to this one.
     * 
     * Returns: Nullable RoadTile
     */
	public RoadTile getNeighborRoadTile(DirectionOfTravel directionOfTravel) {
        var gm = gameObject.GetComponent<RoadManager>();


        switch (directionOfTravel)
        {
            
            case DirectionOfTravel.Up:
                if (location.col > gm.tiles.GetLength(0)) { return null; }
                return gm.tiles[location.row + 1, location.col];
            case DirectionOfTravel.Down:
                if (location.col <= 0) { return null; }
                return gm.tiles[location.row - 1, location.col];

            case DirectionOfTravel.Left:
                if (location.col <= 0) { return null; }
                return gm.tiles[location.row, location.col - 1];
            case DirectionOfTravel.Right:
                if (location.col > gm.tiles.GetLength(1)) { return null; }
                return gm.tiles[location.row, location.col + 1];

            default:
                return null;
        }
    }


    /*
     * Starting from the tile we are in, can we go in cardinal direction. If so, we return the cost. As determined by the cost function.
     * 
     * If we have a road then, we check to see if it goes in the right direction
     * Otherwise, we return true if our direct neighbor has a road in any direction
     * 
     */
    public int? costOfTravelInDirectionOfTravel(DirectionOfTravel directionOfTravel, RoadTile nextRoadTile) {
        switch (directionOfTravel) {
            case DirectionOfTravel.Up:
            case DirectionOfTravel.Down:
                // If we have a road going up, and the other tile has a road going up.
                if (verticalRoad != null && ((verticalRoad.up_left && directionOfTravel == DirectionOfTravel.Up) || (verticalRoad.down_right && directionOfTravel == DirectionOfTravel.Down))) {
                    return verticalRoad.cost();
                }

                break;

            case DirectionOfTravel.Left:
            case DirectionOfTravel.Right:
                // If we have a road going up, and the other tile has a road going up.
                if (horizontalRoad != null && ((horizontalRoad.up_left && directionOfTravel == DirectionOfTravel.Left) || (horizontalRoad.down_right && directionOfTravel == DirectionOfTravel.Right))) {
                    return horizontalRoad.cost();
                }

                break;
        }

        if (nextRoadTile.horizontalRoad != null || nextRoadTile.verticalRoad != null) {
            return 10; // The cost of navigating your car though the sidewalk.
        } else {
            return null;
        }
    }

    static public int Heuristic(Location a, Location b)
    {
        return (int) ((Math.Abs(a.row - b.row) + Math.Abs(a.col - b.col)) * 5); // Assume small roads.
    }

    /*
     * Perform A* search to get to the requested location
     */
    public Location[] findPathToLocation(Location goal) {

        // Setup A*
        var locationsToCheck = new SortedList<int, Location>(new ReverseComparer());
        locationsToCheck.Add(0, goal);

        var cameFrom = new Dictionary<Location, Location>(); // cameFrom[a] = b. I got to a by being in b.
        var costSoFar = new Dictionary<Location, int>(); // The cost for location (until a better one comes around)


        // Itterate in A*
        while (locationsToCheck.Count > 0) {
            // Take the first value
            var current = locationsToCheck[0];
            locationsToCheck.RemoveAt(0);

            // If this location is where we want to go, then we need to exit so that we can recompute our path
            if (current.Equals(goal)) {
                break;
            }

            // We can now check the cardinal directions.
            foreach (var dir in getNeighbors()) {
                var neighbor = getNeighborRoadTile(dir);
                var cost = costOfTravelInDirectionOfTravel(dir, neighbor);
                if (cost != null) {
                    var newCost = costSoFar[current] + cost;
                    if (newCost != null && (!costSoFar.ContainsKey(neighbor.location) || newCost < costSoFar[neighbor.location])) {
                        costSoFar[neighbor.location] = newCost ?? 0;
                        var priority = newCost + Heuristic(neighbor.location, goal);
                        locationsToCheck.Add(priority ?? 0, neighbor.location);
                        cameFrom[neighbor.location] = current;
                    }
                }
            }
        }



        return new Location[0];
    }
}

public class Location {
    public int row;
    public int col;

}

class ReverseComparer : IComparer<int>
{
    public int Compare(int x, int y)
    {
        return -x.CompareTo(y);
    }
}


public class RoadManager : MonoBehaviour {

    public GameObject prefab;
    public RoadTile[,] tiles = new RoadTile[100, 100]; // row, col

	// Use this for initialization
	void Start () {
        System.Random rnd = new System.Random();

        for (int row = 0; row < 100; row ++) {
            for (int col = 0; col < 100; col++)
            {
                var go = Instantiate(prefab);
                go.transform.position = new Vector3(row, 0.0f, col);

                // Make the road tile
                var tile = go.AddComponent<RoadTile>();
                tile.location = new Location();
                tile.location.col = col;
                tile.location.row = row;

                // Make the roads. Right now, lets randomly seed
                var up    = rnd.NextDouble() >= 0.75f;
                var down  = rnd.NextDouble() >= 0.75f;
                var left  = rnd.NextDouble() >= 0.75f;
                var right = rnd.NextDouble() >= 0.75f;
                if (up || down) {
                    tile.verticalRoad = new Road();
                    tile.verticalRoad.size = Size.Medium;
                    tile.verticalRoad.up_left = up;
                    tile.verticalRoad.down_right = down;
                }

                if (left || right) {
                    tile.horizontalRoad = new Road();
                    tile.horizontalRoad.size = Size.Medium;
                    tile.horizontalRoad.up_left = up;
                    tile.horizontalRoad.down_right = down;
                }


                // Put the tile int the array
                tiles[row, col] = tile;
            } 
        }
	}
	
	// Update is called once per frame
	void Update () {
		


	}
}
