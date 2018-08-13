using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum Size : byte { Small, Medium, Large }
//public enum Density : byte { Rural, SubUrban, Urban }
//public enum TitleType: byte { Nature, Water, Housing, Commerce, Offices, Industrial}

// Road is an edge, it has a start, end and size. They can be unidirectional or bidirectional
public class Road {
    public Size size;
    public bool up_left;    // True if road goes up or left
    public bool down_right; // True if road goes down or right


    // Returns an inverted version of the size. Where the heigher the size of the road, the lower the cost. Yay.
    public int cost() {
        switch (size) {
            case Size.Medium:
                return 50;
            case Size.Large:
                return 1;
            default:
                return 1000;
        }
    }

    public float speed() {
        switch (size)
        {
            case Size.Medium:
                return 5.0f;
            case Size.Large:
                return 10.0f;
            default:
                return 1f; // Small and invalid
        }
    }
}


public enum DirectionOfTravel: byte {Up, Left, Down, Right}

// Tile is a cell on the game board. Roads interconnect tiles
public class RoadTile {
    public RoadManager roadManager;

    // Where it is on the map
    public Location location;

    // The roads that are cutting across
    public Road verticalRoad;
    public Road horizontalRoad;

    // Denote if there should be powerlines drawn instead of roads
    public bool verticalPower;
    public bool horizontalPower;

    // What type of tile is this, how dense?
    public TileType type;
    public Density density;

    public Queue<CarPathfinder> upQueue, downQueue, leftQueue, rightQueue;
    DirectionOfTravel phase = DirectionOfTravel.Up;

    float totalSeconds = 5.0f;
    float currentSeconds = 0.0f;

    public RoadTile() {
        upQueue = new Queue<CarPathfinder>();
        downQueue = new Queue<CarPathfinder>();
        leftQueue = new Queue<CarPathfinder>();
        rightQueue = new Queue<CarPathfinder>();
    }

    public void update(float timedelta) {

        // Exit early if we are not actually a traffic light.
        if (horizontalRoad == null) {
            phase = DirectionOfTravel.Left;
        } else if (verticalRoad == null) {
            phase = DirectionOfTravel.Up;
        }

        if (horizontalRoad != null && horizontalRoad.size == Size.Large) {
            releaseACarInDirection(DirectionOfTravel.Left);
            releaseACarInDirection(DirectionOfTravel.Right);
        }

        if (verticalRoad != null && verticalRoad.size == Size.Large) {
            releaseACarInDirection(DirectionOfTravel.Up);
            releaseACarInDirection(DirectionOfTravel.Down);
        }

        // Check to see if we need to change the light.
        currentSeconds += timedelta;
        if (currentSeconds > totalSeconds) {
            currentSeconds = 0;
            phase = (DirectionOfTravel) (((byte)phase + 1) % 4);
        }


        releaseACarInDirection(phase);
        releaseACarInDirection(((DirectionOfTravel) (((byte) phase + 2) % 4)));
    }

    private void releaseACarInDirection(DirectionOfTravel dir) {
        // Debug.
        var a = new Vector3(location.col * 4 + 2, 8, location.row * 4 + 2);
        var b = new Vector3(location.col * 4 + 2, 8, location.row * 4 + 3);

        switch (dir) {
            case DirectionOfTravel.Up:
                if(roadManager.data.drawDebug) { 
                    Debug.DrawLine(a, b, Color.magenta, 0.5f);
                }

                if (upQueue.Count > 0) {
                    if (upQueue.Peek().canMove()) {
                        upQueue.Peek().move();
                        upQueue.Dequeue();
                    }
                } else {
                    // Debug.Log("Nothing in queue UP");
                    currentSeconds += 0.1f; // Help the clock along.
                }

                break;
            case DirectionOfTravel.Down:
                a = new Vector3(location.col * 4 + 2, 8, location.row * 4 + 2);
                b = new Vector3(location.col * 4 + 2, 8, location.row * 4 + 1);
                if(roadManager.data.drawDebug) { 
                    Debug.DrawLine(a, b, Color.magenta, 0.5f);
                }

                if (downQueue.Count > 0) {
                    if (downQueue.Peek().canMove())
                    {
                        downQueue.Peek().move();
                        downQueue.Dequeue();
                    }
                }
                else {
                    // Debug.Log("Nothing in queue DOWN");
                    currentSeconds += 0.1f; // Help the clock along.
                }

                break;
            case DirectionOfTravel.Left:
                a = new Vector3(location.col * 4 + 1, 8, location.row * 4 + 2);
                b = new Vector3(location.col * 4 + 2, 8, location.row * 4 + 2);
                if(roadManager.data.drawDebug) { 
                    Debug.DrawLine(a, b, Color.magenta, 0.5f);
                }
                
                if (leftQueue.Count > 0) {
                    if (leftQueue.Peek().canMove())
                    {
                        leftQueue.Peek().move();
                        leftQueue.Dequeue();
                    }
                }
                else {
                    // Debug.Log("Nothing in queue LEFT");
                    currentSeconds += 0.1f; // Help the clock along.
                }

                break;
            case DirectionOfTravel.Right:
                a = new Vector3(location.col * 4 + 3, 8, location.row * 4 + 2);
                b = new Vector3(location.col * 4 + 2, 8, location.row * 4 + 2);
                if(roadManager.data.drawDebug) { 
                    Debug.DrawLine(a, b, Color.magenta, 0.5f);
                }
                if (rightQueue.Count > 0) {
                    if (rightQueue.Peek().canMove())
                    {
                        rightQueue.Peek().move();
                        rightQueue.Dequeue();
                    }
                }
                else {
                    // Debug.Log("Nothing in queue RIGHT");
                    currentSeconds += 0.1f; // Help the clock along.
                }

                break;

        }
    }

    public bool Occupied {
        get {
            return verticalRoad != null || horizontalRoad != null;
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
        switch (directionOfTravel)
        {
            
            case DirectionOfTravel.Up:
                if (location.row + 1 >= roadManager.tiles.GetLength(0)) { return null; }
                return roadManager.tiles[location.row + 1, location.col];
            case DirectionOfTravel.Down:
                if (location.row <= 0) { return null; }
                return roadManager.tiles[location.row - 1, location.col];

            case DirectionOfTravel.Left:
                if (location.col <= 0) { return null; }
                return roadManager.tiles[location.row, location.col - 1];
            case DirectionOfTravel.Right:
                if (location.col + 1 >= roadManager.tiles.GetLength(1)) { return null; }
                return roadManager.tiles[location.row, location.col + 1];

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
                // If we have a vertical road, and they have a vertical road going up
                if (verticalRoad != null && verticalRoad.up_left && nextRoadTile.verticalRoad != null && nextRoadTile.verticalRoad.up_left) {
                    var cost = 0 ;//roadManager.tiles[tile.location.row, tile.location.col].upQueue.Count;
                    return verticalRoad.cost()/2 + nextRoadTile.verticalRoad.cost()/2 + cost;
                } else if (verticalRoad != null && verticalRoad.up_left && nextRoadTile.horizontalRoad != null) {
                    var cost = 0 ;//roadManager.tiles[tile.location.row, tile.location.col].upQueue.Count;
                    return verticalRoad.cost() + cost;
                }

                break;
            case DirectionOfTravel.Down:
                if (verticalRoad != null && verticalRoad.down_right && nextRoadTile.verticalRoad != null && nextRoadTile.verticalRoad.down_right) {
                    return verticalRoad.cost()/2 + nextRoadTile.verticalRoad.cost()/2;
                } else if (verticalRoad != null && verticalRoad.down_right && nextRoadTile.horizontalRoad != null) {
                    var cost = 0 ;//roadManager.tiles[tile.location.row, tile.location.col].upQueue.Count;
                    return verticalRoad.cost() + cost;
                }

                break;
            case DirectionOfTravel.Left:
                if (horizontalRoad != null && horizontalRoad.up_left && nextRoadTile.horizontalRoad != null && nextRoadTile.horizontalRoad.up_left) {
                    return horizontalRoad.cost()/2 + nextRoadTile.horizontalRoad.cost()/2;
                } else if (horizontalRoad != null && horizontalRoad.up_left && nextRoadTile.verticalRoad != null) {
                    var cost = 0 ;//roadManager.tiles[tile.location.row, tile.location.col].upQueue.Count;
                    return horizontalRoad.cost() + cost;
                }

                break;
            case DirectionOfTravel.Right:
                if (horizontalRoad != null && horizontalRoad.down_right && nextRoadTile.horizontalRoad != null && nextRoadTile.horizontalRoad.down_right) {
                    return horizontalRoad.cost()/2 + nextRoadTile.horizontalRoad.cost()/2;
                } else if (horizontalRoad != null && horizontalRoad.down_right && nextRoadTile.verticalRoad != null) {
                    var cost = 0 ;//roadManager.tiles[tile.location.row, tile.location.col].upQueue.Count;
                    return horizontalRoad.cost() + cost;
                }

                break;
        }

        return null;
    }

    static public int Heuristic(Location a, Location b)
    {
        return ((int) ((Math.Abs(a.row - b.row) + Math.Abs(a.col - b.col)))) * 100; // Assume small roads.
    }

    /*
     * Perform A* search to get to the requested location
     */
    public List<Location> findPathToLocation(Location goal) {

        // Setup A*
        var locationsToCheck = new PriorityQueue<Location>();
        locationsToCheck.Enqueue(location, 0);

        var cameFrom = new Dictionary<Location, Location>(); // cameFrom[a] = b. I got to a by being in b.
        var costSoFar = new Dictionary<Location, int>(); // The cost for location (until a better one comes around)
        costSoFar[location] = 0;
        cameFrom[location] = location;

        // Itterate in A*
        while (locationsToCheck.Count > 0) {

            // Take the first value
            var current = locationsToCheck.Dequeue();
            var currentRoadTile = roadManager.tiles[current.row, current.col];

            // If this location is where we want to go, then we need to exit so that we can recompute our path
            if (current.Equals(goal)) {
                break;
            }

            // We can now check the cardinal directions.
            foreach (var dir in getNeighbors()) {
                var neighbor = currentRoadTile.getNeighborRoadTile(dir);
                if (neighbor != null) {

                    var cost = costOfTravelInDirectionOfTravel(dir, neighbor);
                    if (cost != null)
                    {
                        var newCost = costSoFar[current] + cost;
                        if (newCost != null && (!costSoFar.ContainsKey(neighbor.location) || newCost < costSoFar[neighbor.location]))
                        {
                            costSoFar[neighbor.location] = newCost ?? 0;
                            var priority = newCost + Heuristic(neighbor.location, goal);
                            locationsToCheck.Enqueue(neighbor.location, priority ?? 0);
                            cameFrom[neighbor.location] = current;
                        }
                    }
                }
            }
        }

        // We may have fallen through, without finding the goal. So, lets make sure that the goal was found.
        if (!cameFrom.ContainsKey(goal)) {
            return null;
        }

        // backtrack
        var backtrackingCurrent = goal;
        var reversePath = new List<Location>();
        while (!backtrackingCurrent.Equals(location)) {
            reversePath.Add(backtrackingCurrent);
            backtrackingCurrent = cameFrom[backtrackingCurrent];
        }
        reversePath.Add(location);

        // return the path (in the right direction)
        reversePath.Reverse();
        return reversePath;
    }
}

public struct Location {
    public int row;
    public int col;

    public override String ToString() {
        return String.Format("{0}, {1}", row, col);
    }
}

public struct FakeTuple<M, N> {
    public M Item1;
    public N Item2;

    public FakeTuple(M item1, N item2) {
        Item1 = item1;
        Item2 = item2;
    }
}

// Samelessly stolen from: https://www.redblobgames.com/pathfinding/a-star/implementation.html#csharp
public class PriorityQueue<T>
{
    // I'm using an unsorted array for this example, but ideally this
    // would be a binary heap. There's an open issue for adding a binary
    // heap to the standard C# library: https://github.com/dotnet/corefx/issues/574
    //
    // Until then, find a binary heap class:
    // * https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
    // * http://visualstudiomagazine.com/articles/2012/11/01/priority-queues-with-c.aspx
    // * http://xfleury.github.io/graphsearch.html
    // * http://stackoverflow.com/questions/102398/priority-queue-in-net

    private List<FakeTuple<T, double>> elements = new List<FakeTuple<T, double>>();

    public int Count
    {
        get { return elements.Count; }
    }

    public void Enqueue(T item, double priority)
    {
        elements.Add(new FakeTuple<T, double>(item, priority));
    }

    public T Dequeue()
    {
        int bestIndex = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].Item2 < elements[bestIndex].Item2)
            {
                bestIndex = i;
            }
        }

        T bestItem = elements[bestIndex].Item1;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }
}


public class RoadManager : MonoBehaviour {


    [HideInInspector]
    public RoadTile[,] tiles = new RoadTile[75, 50]; // row, col

    public RoadData data;

    public void Initialize(TerrainManager tm) {
        var heights = tm.GetHeights();

        for (int row = 0; row < tiles.GetLength(0); row ++) {
            for (int col = 0; col < tiles.GetLength(1); col++)
            {
                // var go = Instantiate(prefab);
                // go.transform.position = new Vector3(row, 0.0f, col);

                // Make the road tile
                var tile = new RoadTile();
                tile.location = new Location();
                tile.location.col = col;
                tile.location.row = row;
                tile.roadManager = this;

                // Put the tile int the array
                tiles[row, col] = tile;

                if(heights[row * 4, col * 4] == 0) {
                    continue;
                }

                // Make the roads.
                var pxl = data.roadMap.GetPixel(col, row);
                var dirA = pxl.r != 0; // Right now everything is all or nothing.

                var up = false;
                if (row < tiles.GetLength(0))
                {
                    up = dirA && data.roadMap.GetPixel(col, row + 1).r > 0;
                }

                var down = false;
                if (row > 0)
                {
                    down = dirA && data.roadMap.GetPixel(col, row - 1).r > 0;
                }

                var left = false;
                if (col > 0)
                {
                    left = dirA && data.roadMap.GetPixel(col - 1, row).r > 0;
                }

                var right = false;
                if (col <= tiles.GetLength(1))
                {
                    right = dirA && data.roadMap.GetPixel(col + 1, row).r > 0;
                }

                // The red channel allows us to make the vertical more beefy, while the green is horizontal.
                var r = (int)(pxl.r * 255);
                var g = (int)(pxl.g * 255);

                var vsize = (r - 63) / 64 - 1;
                var hsize = (g - 63) / 64 - 1;

                var pos = new Vector3(col * 4.0f + 2.0f, heights[row * 4, col * 4], row * 4.0f + 2.0f);
                // var drawDebug = row %  == 0 && col % 3 == 0;
                if (up || down) {
                    tile.verticalRoad = new Road();
                    tile.verticalRoad.size = (Size) vsize;
                    tile.verticalRoad.up_left = up;
                    tile.verticalRoad.down_right = down;

                    if (up && data.drawDebug) { Debug.DrawLine(new Vector3(pos.x, pos.y, pos.z), new Vector3(pos.x, pos.y, pos.z + 0.5f), Color.red, 10000.0f); }
                    if (down && data.drawDebug) { Debug.DrawLine(new Vector3(pos.x, pos.y, pos.z), new Vector3(pos.x, pos.y, pos.z - 0.5f), Color.black, 10000.0f); }
                }

                if (left || right) {
                    tile.horizontalRoad = new Road();
                    tile.horizontalRoad.size = (Size) hsize;
                    tile.horizontalRoad.up_left = left;
                    tile.horizontalRoad.down_right = right;
                    
                    if (left && data.drawDebug) { Debug.DrawLine(new Vector3(pos.x, pos.y, pos.z), new Vector3(pos.x - 0.5f, pos.y, pos.z), Color.blue, 10000.0f); }
                    if (right && data.drawDebug) { Debug.DrawLine(new Vector3(pos.x, pos.y, pos.z), new Vector3(pos.x + 0.5f, pos.y, pos.z), Color.white, 10000.0f); }
                }
            } 
        }

        if(data.enableCarSim) {
            var twiddles = new List<int>();

            twiddles.Add(-1);
            twiddles.Add( 0);
            twiddles.Add( 1);

            foreach (var twiddle1 in twiddles)
            {
                foreach (var twiddle2 in twiddles)
                {
                    foreach (var twiddle3 in twiddles)
                    {
                        foreach (var twiddle4 in twiddles)
                        {
                            // makeACarGo(tiles[24 + twiddle1, 16 + twiddle2], tiles[20 + twiddle3, 22 + twiddle4]);
                            // makeACarGo(tiles[20 + twiddle1, 16 + twiddle2], tiles[20 + twiddle3, 18 + twiddle4]);
                            // makeACarGo(tiles[24 + twiddle1, 16 + twiddle2], tiles[19 + twiddle3, 22 + twiddle4]);
                            // makeACarGo(tiles[24 + twiddle1, 16 + twiddle2], tiles[4  + twiddle3, 40 + twiddle4]);

                            makeACarGo(tiles[20 + twiddle1, 14 + twiddle2], tiles[44  + twiddle3, 38 + twiddle4]);
                        }
                    }
                }
            }
        } 
    }

	private void Update()
	{
        for (int row = 0; row < tiles.GetLength(0); row++)
        {
            for (int col = 0; col < tiles.GetLength(1); col++)
            {
                tiles[row, col].update(Time.deltaTime);
            }
        }
	}

	void makeACarGo(RoadTile start, RoadTile end) {
        var car = Instantiate(data.carModel);
        var carPathfinder = car.GetComponent<CarPathfinder>();
        carPathfinder.roadManager = gameObject;
        carPathfinder.startTile = start;
        carPathfinder.endTile = end;

        carPathfinder.originalStart = start;
        carPathfinder.originalEnd = end;

        carPathfinder.planAndGo();
    }
}
