using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TerrainData
{
    public int heightmapResolution;
    public Vector3 size;
    public float[,] heights;

    public TerrainData(int heightmapResolution, Vector3 size)
    {
        this.heightmapResolution = heightmapResolution;
        this.size = size;
        this.heights = new float[heightmapResolution, heightmapResolution];
    }

    public void SetHeights(int x, int y, float[,] heights)
    {
        int length = Mathf.Min(heightmapResolution - x, heights.GetLength(0));
        int width = Mathf.Min(heightmapResolution - y, heights.GetLength(1));
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                this.heights[i + x, j + y] = heights[i, j];
            }
        }
    }

    public float SampleHeight(float x, float z)
    {
        float xCoord = x / size.x;
        float zCoord = z / size.z;

        int xIndex = Mathf.RoundToInt(xCoord * (heightmapResolution - 1));
        int zIndex = Mathf.RoundToInt(zCoord * (heightmapResolution - 1));

        return heights[xIndex, zIndex] * size.y;
    }
}

