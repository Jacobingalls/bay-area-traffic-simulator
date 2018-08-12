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
        var linePos = new Vector3(startTile.transform.position.x, 2, startTile.transform.position.z);
        if (path != null)
        {
            foreach (var loc in path)
            {
                var positionOfTile = rm.tiles[loc.row, loc.col].transform.position;
                var newLinePos = new Vector3(positionOfTile.x, 2, positionOfTile.z);
                Debug.DrawLine(linePos, newLinePos, Color.white, 100);
                linePos = newLinePos;
            }
        }

        var startPostition = startTile.transform.position;
        Debug.DrawLine(new Vector3(startPostition.x - 0.5f, 2, startPostition.z - 0.5f), new Vector3(startPostition.x + 0.5f, 2, startPostition.z + 0.5f), Color.green, 100);
        Debug.DrawLine(new Vector3(startPostition.x + 0.5f, 2, startPostition.z - 0.5f), new Vector3(startPostition.x - 0.5f, 2, startPostition.z + 0.5f), Color.green, 100);

        var endPosition = endTile.transform.position;
        Debug.DrawLine(new Vector3(endPosition.x - 0.5f, 2, endPosition.z - 0.5f), new Vector3(endPosition.x + 0.5f, 2, endPosition.z + 0.5f), Color.red, 100);
        Debug.DrawLine(new Vector3(endPosition.x + 0.5f, 2, endPosition.z - 0.5f), new Vector3(endPosition.x - 0.5f, 2, endPosition.z + 0.5f), Color.red, 100);
    }

    private void Update()
    {
        System.Random rnd = new System.Random();
        var rm = roadManager.GetComponent<RoadManager>();
        if (startTile == null) {
            startTile = rm.tiles[rnd.Next() % 100, rnd.Next() % 100];
            endTile = startTile;
            gameObject.transform.position = new Vector3(startTile.transform.position.x, 3, startTile.transform.position.z);
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

            var previousPosition = rm.tiles[previousLoc.row, previousLoc.col].transform.position;
            var nextPosition = rm.tiles[nextLoc.row, nextLoc.col].transform.position;

            // Lets do a spline!
            var realPos = Vector3.Lerp(new Vector3(previousPosition.x, 3, previousPosition.z), new Vector3(nextPosition.x, 3, nextPosition.z), progressOnCurrentSegment);
            gameObject.transform.position = realPos;
        }


    }
}