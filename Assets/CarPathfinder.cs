using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


public class CarPathfinder : MonoBehaviour
{
    List<Location> path;
    bool shouldUpdatePath;
    public RoadTile startTile, endTile;

    float progressOnCurrentSegment = 0.0f;
    int segment;
    public GameObject roadManager;
    bool needsAMove = false;


    public void planAndGo() {
        var t = new Thread(() => {
            segment = 1;
            path = startTile.findPathToLocation(endTile.location);
            needsAMove = true;
        });
        t.Start();
    }

    Vector3 toWorldSpace(int row, int col, int height)
    {
        return new Vector3((col * 4) + 2, height, (row * 4) + 2);
    }

    Vector3 toWorldSpace(Location loc, int height)
    {
        return toWorldSpace(loc.row, loc.col, height);
    }

    private void Update()
    {   
        if (needsAMove) {
            move();
            needsAMove = false;
        }

        if (startTile != null && endTile != null && path != null) {
            if (segment >= path.Count) { return; }

            var loc = path[segment];
            var current = roadManager.GetComponent<RoadManager>().tiles[loc.row, loc.col];

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
                    speed = current.verticalRoad.speed();
                    break;
                case DirectionOfTravel.Left:
                case DirectionOfTravel.Right:
                    speed = current.horizontalRoad.speed();
                    break;
            }




            progressOnCurrentSegment += Time.deltaTime * 0.50f * speed;

            if (progressOnCurrentSegment >= 1) {
                progressOnCurrentSegment = 0.0f;

                if (segment + 1 >= path.Count) {
                    progressOnCurrentSegment = 0f;
                    var temp = endTile;
                    endTile = startTile;
                    startTile = temp;
                    path = null;
                    planAndGo();
                    return;
                } else {
                    //segment++;

                    // Saturate for now, we are waiting for OK from light.
                    progressOnCurrentSegment = 1.0f;
                }

            }

            var previousLoc = path[segment - 1];
            var nextLoc = path[segment];

            // Lets do a spline!
            var realPos = Vector3.Lerp(toWorldSpace(previousLoc, 4), toWorldSpace(nextLoc, 4), progressOnCurrentSegment);
            gameObject.transform.position = realPos;


        }


    }


    public bool canMove() {
        if (segment + 1 >= path.Count) { return true; }

        if (progressOnCurrentSegment < .8) { return false;}

        var loc = path[segment];
        var current = roadManager.GetComponent<RoadManager>().tiles[loc.row, loc.col];

        // Going to move up
        if (path[segment].row > path[segment + 1].row)
        {
            return current.getNeighborRoadTile(DirectionOfTravel.Up).upQueue.Count < 5;
        }

        // Going to move down
        else if (path[segment].row < path[segment + 1].row)
        {
            return current.getNeighborRoadTile(DirectionOfTravel.Down).downQueue.Count < 5;
        }

        // Going to move left
        else if (path[segment].col < path[segment + 1].col)
        {
            return current.getNeighborRoadTile(DirectionOfTravel.Left).leftQueue.Count < 5;
        }

        // Going to move right
        else
        {
            return current.getNeighborRoadTile(DirectionOfTravel.Right).rightQueue.Count < 5;
        }
    }

    public void move() {
        if (segment + 1 >= path.Count) { return; }

        var loc = path[segment];
        var current = roadManager.GetComponent<RoadManager>().tiles[loc.row, loc.col];

        // Going to move up
        if (path[segment].row > path[segment + 1].row)
        {
            current.getNeighborRoadTile(DirectionOfTravel.Up).upQueue.Enqueue(this);
        }

        // Going to move down
        else if (path[segment].row < path[segment + 1].row)
        {
            current.getNeighborRoadTile(DirectionOfTravel.Down).downQueue.Enqueue(this);
        }

        // Going to move left
        else if (path[segment].col < path[segment + 1].col)
        {
            current.getNeighborRoadTile(DirectionOfTravel.Left).leftQueue.Enqueue(this);
        }

        // Going to move right
        else
        {
            current.getNeighborRoadTile(DirectionOfTravel.Right).rightQueue.Enqueue(this);
        }

        segment++;
        progressOnCurrentSegment = 0;
    }
}