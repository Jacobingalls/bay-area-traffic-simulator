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
            progressOnCurrentSegment += Time.deltaTime * 0.50f;

            if (progressOnCurrentSegment >= 1) {
                progressOnCurrentSegment = 0.0f;

                if (segment + 1 >= path.Count) {
                    progressOnCurrentSegment = 0f;
                    startTile = endTile;
                    path = null;
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