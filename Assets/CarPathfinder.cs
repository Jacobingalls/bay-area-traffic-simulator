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



    public void planAndGo() {
        var t = new Thread(() => {
            segment = 1;
            path = startTile.findPathToLocation(endTile.location);
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
                    segment++;
                }

            }

            var previousLoc = path[segment - 1];
            var nextLoc = path[segment];

            // Lets do a spline!
            var realPos = Vector3.Lerp(toWorldSpace(previousLoc, 4), toWorldSpace(nextLoc, 4), progressOnCurrentSegment);
            gameObject.transform.position = realPos;


        }


    }
}