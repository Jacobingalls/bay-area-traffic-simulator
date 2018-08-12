using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


public class CarPathfinder : MonoBehaviour
{
    List<Location> path;
    bool shouldUpdatePath;
    RoadTile startTile, endTile;

    float progressOnCurrentSegment = 0.0f;
    int segment;
    public GameObject roadManager;



    void determineNextDestinationAndPath() {
        System.Random rnd = new System.Random();
        var rm = roadManager.GetComponent<RoadManager>();
        endTile = rm.tiles[rnd.Next() % 100, rnd.Next() % 100];
        segment = 1;

        var t = new Thread(() => {
            path = startTile.findPathToLocation(endTile.location);
        });
        t.Start();
    }

    private void drawDebugLines() {
        var rm = roadManager.GetComponent<RoadManager>();
        var linePos = new Vector3(startTile.location.row * 4, 2, startTile.location.col * 4);
        if (path != null)
        {
            foreach (var loc in path)
            {
                var newLinePos = new Vector3(loc.row * 4, 2, loc.col * 4);
                Debug.DrawLine(linePos, newLinePos, Color.white, 100);
                linePos = newLinePos;
            }
        }

        var startPostition = new Vector3(startTile.location.row * 4, 2, startTile.location.col * 4);
        Debug.DrawLine(new Vector3(startPostition.x * 4 - 2f, 2, startPostition.z * 4 - 2f), new Vector3(startPostition.x * 4 + 2f, 2, startPostition.z * 4 + 2f), Color.green, 100);
        Debug.DrawLine(new Vector3(startPostition.x * 4 + 2f, 2, startPostition.z * 4 - 2f), new Vector3(startPostition.x * 4 - 2f, 2, startPostition.z * 4 + 2f), Color.green, 100);

        var endPosition = new Vector3(endTile.location.row, 2, endTile.location.col);
        Debug.DrawLine(new Vector3(endPosition.x * 4 - 2f, 2, endPosition.z * 4 - 2f), new Vector3(endPosition.x * 4 + 2f, 2, endPosition.z * 4 + 2f), Color.red, 100);
        Debug.DrawLine(new Vector3(endPosition.x * 4 + 2f, 2, endPosition.z * 4 - 2f), new Vector3(endPosition.x * 4- 2f, 2, endPosition.z * 4 + 2f), Color.red, 100);
    }

    private void Update()
    {
        System.Random rnd = new System.Random();
        var rm = roadManager.GetComponent<RoadManager>();
        if (startTile == null) {
            startTile = rm.tiles[rnd.Next() % 100, rnd.Next() % 100];
            endTile = startTile;
            gameObject.transform.position = new Vector3(startTile.location.row * 4, 2, startTile.location.col * 4);
        }

        //drawDebugLines();

        if (startTile == endTile) {
            determineNextDestinationAndPath();
        }


        if (path != null) {
            progressOnCurrentSegment += Time.deltaTime * 0.50f;

            if (progressOnCurrentSegment >= 1) {
                progressOnCurrentSegment = 0.0f;

                if (segment + 1 >= path.Count) {
                    progressOnCurrentSegment = 0f;
                    startTile = endTile;
                } else {
                    segment++;
                }

            }

            var previousLoc = path[segment - 1];
            var nextLoc = path[segment];

            // Lets do a spline!
            var realPos = Vector3.Lerp(new Vector3(previousLoc.row * 4, 3, previousLoc.col * 4), new Vector3(nextLoc.row * 4, 3, nextLoc.col * 4), progressOnCurrentSegment);
            gameObject.transform.position = realPos;
        }


    }
}