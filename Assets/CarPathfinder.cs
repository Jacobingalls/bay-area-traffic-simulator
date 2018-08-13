using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


public class CarPathfinder : MonoBehaviour
{
    static Vector3 surfaceOffset = new Vector3(0.0f, 0.1f, 0.0f);

    List<Location> path;
    bool shouldUpdatePath;
    public RoadTile originalStart, originalEnd, startTile, endTile;

    float progressOnCurrentSegment = 0.0f;
    int segment;
    public RoadManager roadManager;
    bool needsAMove = false;

    float refreshTime = 0;

    public bool done = false;
    float timeOnRoad = 0.0f;

    Material myMaterial;

    public void planAndGo() {
        var t = new Thread(() => {
            segment = 1;
            path = startTile.findPathToLocation(endTile.location);
            needsAMove = true;
            if(path == null) {
                done=true;
            }
        });
        t.Start();
    }

    private void Update()
    {   
        timeOnRoad += Time.deltaTime;

        if (needsAMove) {
            move();
            needsAMove = false;
        }

        if (startTile != null && endTile != null && path != null) {
            if (segment >= path.Count) { return; }

            var previousLoc = path[segment - 1];
            var previous = roadManager.tiles[previousLoc.row, previousLoc.col];

            var loc = path[segment];
            var current = roadManager.tiles[loc.row, loc.col];

            var dir = DirectionOfTravel.Right;
            if (path[segment - 1].row > path[segment].row) {
                dir = DirectionOfTravel.Up;
            } else if (path[segment - 1].row < path[segment].row) {
                dir = DirectionOfTravel.Down;
            } else if (path[segment - 1].col < path[segment].col) {
                dir = DirectionOfTravel.Left;
            } else {
                dir = DirectionOfTravel.Right;
            }

            var speed = 1.0f;
            switch (dir) {
                case DirectionOfTravel.Up:
                case DirectionOfTravel.Down:
                    speed = current.verticalRoad.speed() > previous.verticalRoad.speed() ? previous.verticalRoad.speed() : current.verticalRoad.speed();
                    break;
                case DirectionOfTravel.Left:
                case DirectionOfTravel.Right:
                    speed = current.horizontalRoad.speed() > previous.horizontalRoad.speed() ? previous.horizontalRoad.speed() : current.horizontalRoad.speed();
                    break;
            }

            switch (dir) {
                case DirectionOfTravel.Right:
                    transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    break;
                case DirectionOfTravel.Up:
                    transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                    break;
                case DirectionOfTravel.Left:
                    transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                    break;
                case DirectionOfTravel.Down:
                    transform.rotation = Quaternion.Euler(0.0f, 270.0f, 0.0f);
                    break;
            }

            if(roadManager.data.debugCarTraffic) {
                gameObject.GetComponent<Renderer>().material = roadManager.data.green;
            }

            progressOnCurrentSegment += Time.deltaTime * 0.5f * speed;

            // Our position is based on our index.
            var queue = getOurQueue(segment - 1);
            var i = 0;
            if (queue != null) {
                foreach(var obj in queue) {
                    if (obj.Equals(this)) {
                        break;
                    }

                    i ++;
                }
            }

            // While wating on a light we need to queue up.
            var maxProgress = (float)(6 - i) / 6.0f;
            if (progressOnCurrentSegment > maxProgress) {
                progressOnCurrentSegment = maxProgress;
                if(roadManager.data.debugCarTraffic) {
                    gameObject.GetComponent<Renderer>().material = roadManager.data.red;
                }
            }

            if (progressOnCurrentSegment >= 1) {
                if(roadManager.data.debugCarTraffic) {
                    gameObject.GetComponent<Renderer>().material = roadManager.data.yellow;
                }
                if (segment + 1 >= path.Count) {
                    
                    var l = path[segment];
                    var c = roadManager.tiles[l.row, l.col];

                    gameObject.GetComponent<MeshRenderer>().enabled = false;
                    done = true;

                    if(queue != null && queue.Count > 0 && queue.Peek().Equals(this)) {
                        queue.Dequeue(); 
                    }
                }
                

                progressOnCurrentSegment = 1.0f;
            }

            previousLoc = path[segment - 1];
            var nextLoc = path[segment];

            // Lets do a spline!
            var realPos = GameManager.TerrainManagerInstance.LerpWorldSpacePositionBetweenLocations(previousLoc, nextLoc, progressOnCurrentSegment) + surfaceOffset;
            gameObject.transform.position = realPos;



            // refreshTime += Time.deltaTime;
            // if (refreshTime >= 10)
            // {
            //     startTile = current;
            //     planAndGo();
            // }
        }

        if (done) {
            Destroy(gameObject);
        }
    }

    

    public Queue<CarPathfinder> getOurQueue(int seg) {
        if (path == null || seg + 1 >= path.Count) { return null; }

        var loc = path[seg];
        var current = roadManager.tiles[loc.row, loc.col];

        Queue<CarPathfinder> queue = null;

        // Going to move up
        if (path[seg].row > path[seg + 1].row)
        {
            queue = current.getNeighborRoadTile(DirectionOfTravel.Up).upQueue;
        }

        // Going to move down
        else if (path[seg].row < path[seg + 1].row)
        {
            queue = current.getNeighborRoadTile(DirectionOfTravel.Down).downQueue;
        }

        // Going to move left
        else if (path[seg].col < path[seg + 1].col)
        {
            queue = current.getNeighborRoadTile(DirectionOfTravel.Left).leftQueue;
        }

        // Going to move right
        else
        {
            queue = current.getNeighborRoadTile(DirectionOfTravel.Right).rightQueue;
        }

        return queue;
    }

    public bool canMove() {
        if (progressOnCurrentSegment < .8) { return false;}

        var queue = getOurQueue(segment);

        if (queue == null) {
            return false;
        }

        return queue.Count < 5;
    }

    public void move() {
        var queue = getOurQueue(segment);

        if (queue == null) {
            return;
        }

        foreach(var obj in queue) {
            if (obj.Equals(this)) {
                return;
            }
        }

        queue.Enqueue(this);

        segment++;
        progressOnCurrentSegment = 0;
    }

    public List<Location> GetPath() {
        return path;
    }

    public Location GetCurrentLocation() {
        return path[segment];
    }

    public void RefreshColor() {
        gameObject.GetComponent<MeshRenderer>().material = myMaterial;
    }

    public void SetColor() {
        myMaterial = roadManager.data.carColors[Random.Range(0, roadManager.data.carColors.Length)];
        gameObject.GetComponent<MeshRenderer>().material = myMaterial;
    }

    public void OnLeftClick() {
        GameManager.GUIManagerInstance.SelectCar(this);
    }
}