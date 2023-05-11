using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class RockClusterGenerator : MonoBehaviour
{
    public GameObject rockPrefab;
    public Terrain terrain;

    public void GenerateRockClusters(List<Vector3> rockPositions)
    {
        // Sort the rock positions by distance from the center of the terrain
        rockPositions.OrderBy(v => Vector3.Distance(v, terrain.transform.position));

        // Generate rock clusters
        List<List<Vector3>> rockClusters = new List<List<Vector3>>();
        List<Vector3> currentCluster = new List<Vector3>();
        float clusterDistanceThreshold = 5f;

        foreach (Vector3 rockPosition in rockPositions)
        {
            bool addedToCluster = false;

            foreach (Vector3 existingRockPosition in currentCluster)
            {
                if (Vector3.Distance(rockPosition, existingRockPosition) < clusterDistanceThreshold)
                {
                    currentCluster.Add(rockPosition);
                    addedToCluster = true;
                    break;
                }
            }

            if (!addedToCluster)
            {
                currentCluster = new List<Vector3>();
                currentCluster.Add(rockPosition);
                rockClusters.Add(currentCluster);
            }
        }

        // Generate rocks for each cluster
        foreach (List<Vector3> cluster in rockClusters)
        {
            Vector3 clusterCenter = Vector3.zero;

            foreach (Vector3 rockPosition in cluster)
            {
                clusterCenter += rockPosition;
            }

            clusterCenter /= cluster.Count;

            GameObject rockClusterObject = new GameObject("RockCluster");
            rockClusterObject.transform.parent = transform;

            foreach (Vector3 rockPosition in cluster)
            {
                Vector3 rockPositionOnTerrain = new Vector3(rockPosition.x, terrain.SampleHeight(rockPosition), rockPosition.z);
                Vector3 directionFromCenter = rockPositionOnTerrain - clusterCenter;
                float distanceFromCenter = directionFromCenter.magnitude;
                float distancePercent = distanceFromCenter / clusterDistanceThreshold;
                float scale = Mathf.Lerp(1.0f, 0.1f, distancePercent);

                GameObject rockObject = Instantiate(rockPrefab, rockPositionOnTerrain, Quaternion.identity);
                rockObject.transform.localScale = new Vector3(scale, scale, scale);
                rockObject.transform.LookAt(clusterCenter);
                rockObject.transform.parent = rockClusterObject.transform;
            }
        }
    }
}
