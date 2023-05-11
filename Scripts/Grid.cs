using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Grid : MonoBehaviour
{
    public float rockNoiseScale = .05f;
    public float rockDensity = .5f;
    public Material terrainMaterial;
    //public GameObject terrainObject;

    public Material edgeMaterial;
    public Material lavaMaterial;
    public float lavaLevel = .4f;
    public float scale = .1f;
    public int size = 100;
    //public TerrainClass terrain;
    public GameObject[] rockPrefabs;
    public GameObject edgeObj;

    //public GameObject Land;
    //public Terrain terrain; // Define terrain here

    Cell[,] grid;

    void Start()
    {

        /*
        // Use the Terrain object assigned in the inspector
        Terrain terrain = this.terrain;

        //terrain = GetComponent<Terrain>();


        // Generate noise and falloff maps
        float[,] noiseMap = GenerateNoiseMap(size, scale, rockNoiseScale);
        float[,] falloffMap = GenerateFalloffMap(size);

        // Create cells and assign them to the grid
        grid = new Cell[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = noiseMap[x, y];
                noiseValue -= falloffMap[x, y];
                bool isLava = noiseValue < lavaLevel;
                Cell cell = new Cell(x, y, isLava);
                grid[x, y] = cell;
            }
        }
        */



        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * scale + xOffset, y * scale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        float[,] falloffMap = new float[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float xv = x / (float)size * 2 - 1;
                float yv = y / (float)size * 2 - 1;
                float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
                falloffMap[x, y] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
            }
        }



        // Calculate the heights of the terrain based on the noise values
        float[,] heightmap = new float[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = noiseMap[x, y];
                float falloffValue = falloffMap[x, y];
                float height = noiseValue - falloffValue;
                heightmap[x, y] = height;
            }
        }

        grid = new Cell[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = noiseMap[x, y];
                noiseValue -= falloffMap[x, y];
                bool isLava = noiseValue < lavaLevel;
                Cell cell = new Cell(x, y, isLava, noiseValue);
                grid[x, y] = cell;
            }
        }

        float heightMultiplier = 1f;
        float lavaHeightMultiplier = 0.4f;


        DrawLavaMesh(grid);



        /*

        int baseMapResolution = 512;
        int basemapDistance = 1000;


        if (terrainMaterial == null)
        {
            terrainMaterial = new Material(Shader.Find("Diffuse"));  // Provide a default material
        }

        Debug.Log("terrainMaterial is " + (terrainMaterial == null ? "null" : "not null")); // Add this line for debugging
        Debug.Log($"heightmap: {heightmap}");

        Vector3 a = new Vector3(0, 0, 0);
        TerrainClass terrain = new TerrainClass(a, heightmap, baseMapResolution, basemapDistance, terrainMaterial);
        */
        /*
         //Create the terrain mesh and apply it to the terrain object
         Mesh terrainMesh = CreateTerrainMesh(grid);

         terrain.terrainData.SetHeights(0, 0, GenerateHeightsFromNoiseMap(noiseMap));
         terrain.terrainData.RefreshPrototypes();
         terrain.Flush();
         terrain.basemapDistance = 1000f;
         terrain.materialTemplate = terrainMaterial;
         terrain.GetComponent<TerrainCollider>().terrainData = terrain.terrainData;
         terrain.GetComponent<MeshFilter>().sharedMesh = terrainMesh;



         // Create the edge mesh and apply it to the edge object
         Mesh edgeMesh = CreateEdgeMesh(grid);
         GameObject edge = Instantiate(edgeObj, Vector3.zero, Quaternion.identity);
         edge.transform.SetParent(transform);
         edge.GetComponent<MeshFilter>().sharedMesh = edgeMesh;
         edge.GetComponent<MeshRenderer>().material = edgeMaterial;
        */
        /*
        // Create the lava mesh and apply it to the terrain object
        Mesh lavaMesh = CreateLavaMesh(grid);
        GameObject lava = new GameObject("Lava", typeof(MeshFilter), typeof(MeshRenderer));
        lava.transform.SetParent(transform);
        lava.GetComponent<MeshFilter>().sharedMesh = lavaMesh;
        lava.GetComponent<MeshRenderer>().material = lavaMaterial;
        lava.transform.localPosition = Vector3.zero;
        lava.transform.localScale = new Vector3(1f, lavaLevel, 1f);

         */

        // Generate rocks
        int numberOfRocks = Random.Range(7, 11);
        float rockPlacementRadius = 1.5f;
        DrawTerrainMesh(grid);
        DrawEdgeMesh(grid);
        DrawTexture(grid);
        //GenerateRocks(numberOfRocks, rockPlacementRadius, terrain);
        GenerateRocks(grid);
    }

    /*   
    Mesh CreateEdgeMesh(Cell[,] grid)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Loop through all cells and add vertices for the edges
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                Cell currentCell = grid[x, y];

                // If this cell is empty or already visited, skip it
                if (currentCell.isEmpty || currentCell.IsVisited) continue;

                // Loop through all neighbors of the cell
                for (int i = 0; i < 4; i++)
                {
                    int nx = x + GridHelpers.NeighborX[i];
                    int ny = y + GridHelpers.NeighborY[i];

                    // If the neighbor is out of bounds or has already been visited, skip it
                    if (!GridHelpers.IsCellInBounds(nx, ny, grid.GetLength(0), grid.GetLength(1)) || grid[nx, ny].IsVisited)
                        continue;

                    // If the neighbor is empty, add a vertex at the midpoint between the current cell and the neighbor
                    if (grid[nx, ny].isEmpty)
                    {
                        Vector3 currentVertex = new Vector3(x, currentCell.height, y);
                        Vector3 neighborVertex = new Vector3(nx, currentCell.height, ny);
                        Vector3 midpoint = (currentVertex + neighborVertex) / 2f;
                        vertices.Add(midpoint);
                        currentCell.IsVisited = true;
                        break;
                    }
                }
            }
        }

        // Loop through all added vertices and create triangles between them
        for (int i = 0; i < vertices.Count; i++)
        {
            int index1 = i;
            int index2 = (i + 1) % vertices.Count;
            triangles.Add(index1);
            triangles.Add(index2);
            triangles.Add(vertices.Count);
        }

        // Add the center point of the terrain as the final vertex
        vertices.Add(new Vector3(grid.GetLength(0) / 2f, 0, grid.GetLength(1) / 2f));

        // Set mesh data and recalculate normals
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }


    Mesh CreateLavaMesh(Cell[,] grid)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        // Loop through each cell in the grid
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];

                // If the cell is lava
                if (cell.isLava)
                {
                    // Calculate the center of the cell
                    Vector3 center = new Vector3(x, 0, y);

                    // Calculate the vertices of the cell
                    Vector3 a = center + new Vector3(-.5f, 0, -.5f);
                    Vector3 b = center + new Vector3(.5f, 0, -.5f);
                    Vector3 c = center + new Vector3(-.5f, 0, .5f);
                    Vector3 d = center + new Vector3(.5f, 0, .5f);

                    // Add the vertices to the list
                    vertices.Add(a);
                    vertices.Add(b);
                    vertices.Add(c);
                    vertices.Add(d);

                    // Add the triangles to the list
                    int i = vertices.Count - 4;
                    triangles.Add(i);
                    triangles.Add(i + 1);
                    triangles.Add(i + 2);
                    triangles.Add(i + 1);
                    triangles.Add(i + 3);
                    triangles.Add(i + 2);

                    // Add the UV coordinates to the list
                    uvs.Add(new Vector2(0, 0));
                    uvs.Add(new Vector2(1, 0));
                    uvs.Add(new Vector2(0, 1));
                    uvs.Add(new Vector2(1, 1));
                }
            }
        }

        // Set the mesh data and recalculate the normals
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }


    float[,] GenerateHeightsFromNoiseMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        float[,] heights = new float[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                heights[x, y] = noiseMap[x, y];
            }
        }

        return heights;
    }


    public Mesh CreateTerrainMesh(Cell[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        // Create arrays to store the vertices, triangles, and UVs
        Vector3[] vertices = new Vector3[width * height];
        int[] triangles = new int[(width - 1) * (height - 1) * 6];
        Vector2[] uv = new Vector2[width * height];

        int vertIndex = 0;
        int trisIndex = 0;

        // Loop through each cell in the grid
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Cell cell = grid[x, y];

                // Calculate the center of the cell
                Vector3 center = new Vector3(x, 0, y);

                // Set the vertex height based on whether the cell is lava or not
                float heightValue = cell.isLava ? 0 : cell.height;

                // Set the vertex position and UV coordinates
                vertices[vertIndex] = center + new Vector3(-.5f, heightValue, -.5f);
                uv[vertIndex] = new Vector2((float)x / width, (float)y / height);

                // If this is not the last column or row
                if (x < width - 1 && y < height - 1)
                {
                    // Add the indices for the triangles
                    triangles[trisIndex] = vertIndex;
                    triangles[trisIndex + 1] = vertIndex + 1;
                    triangles[trisIndex + 2] = vertIndex + width;
                    triangles[trisIndex + 3] = vertIndex + 1;
                    triangles[trisIndex + 4] = vertIndex + width + 1;
                    triangles[trisIndex + 5] = vertIndex + width;
                    trisIndex += 6;
                }

                vertIndex++;
            }
        }

        // Create the mesh and set its data
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        return mesh;
    }



    public float[,] GenerateNoiseMap(int size, float scale, float noiseScale)
    {
        float[,] noiseMap = new float[size, size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float sampleX = x / scale * noiseScale;
                float sampleY = y / scale * noiseScale;
                float noiseValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = noiseValue;
            }
        }

        return noiseMap;
    }

    public float[,] GenerateFalloffMap(int size)
    {
        float[,] map = new float[size, size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float nx = x / (float)size - 0.5f;
                float ny = y / (float)size - 0.5f;

                float value = Mathf.Max(Mathf.Abs(nx), Mathf.Abs(ny));
                map[x, y] = Evaluate(value);
            }
        }

        return map;
    }

    float Evaluate(float value)
    {
        float a = 3f;
        float b = 2.2f;
        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }

    */
    Mesh DrawTerrainMesh(Cell[,] grid)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];
                if (!cell.isLava)
                {
                    Vector3 a = new Vector3(x - .5f, 0, y + .5f);
                    Vector3 b = new Vector3(x + .5f, 0, y + .5f);
                    Vector3 c = new Vector3(x - .5f, 0, y - .5f);
                    Vector3 d = new Vector3(x + .5f, 0, y - .5f);
                    Vector2 uvA = new Vector2(x / (float)size, y / (float)size);
                    Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                    Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                    Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);
                    Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                    Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                    for (int k = 0; k < 6; k++)
                    {
                        vertices.Add(v[k]);
                        triangles.Add(triangles.Count);
                        uvs.Add(uv[k]);
                    }
                }
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        GameObject land = new GameObject("Land", typeof(MeshFilter), typeof(MeshRenderer));
        land.transform.SetParent(transform);

        MeshFilter meshFilter = land.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = land.GetComponent<MeshRenderer>();
        meshRenderer.material = terrainMaterial;

        return mesh;
    }


    Mesh DrawEdgeMesh(Cell[,] grid)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];
                if (!cell.isLava)
                {
                    if (x > 0)
                    {
                        Cell left = grid[x - 1, y];
                        if (left.isLava)
                        {
                            Vector3 a = new Vector3(x - .5f, 0, y + .5f);
                            Vector3 b = new Vector3(x - .5f, 0, y - .5f);
                            Vector3 c = new Vector3(x - .5f, -1, y + .5f);
                            Vector3 d = new Vector3(x - .5f, -1, y - .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for (int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                    if (x < size - 1)
                    {
                        Cell right = grid[x + 1, y];
                        if (right.isLava)
                        {
                            Vector3 a = new Vector3(x + .5f, 0, y - .5f);
                            Vector3 b = new Vector3(x + .5f, 0, y + .5f);
                            Vector3 c = new Vector3(x + .5f, -1, y - .5f);
                            Vector3 d = new Vector3(x + .5f, -1, y + .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for (int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                    if (y > 0)
                    {
                        Cell down = grid[x, y - 1];
                        if (down.isLava)
                        {
                            Vector3 a = new Vector3(x - .5f, 0, y - .5f);
                            Vector3 b = new Vector3(x + .5f, 0, y - .5f);
                            Vector3 c = new Vector3(x - .5f, -1, y - .5f);
                            Vector3 d = new Vector3(x + .5f, -1, y - .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for (int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                    if (y < size - 1)
                    {
                        Cell up = grid[x, y + 1];
                        if (up.isLava)
                        {
                            Vector3 a = new Vector3(x + .5f, 0, y + .5f);
                            Vector3 b = new Vector3(x - .5f, 0, y + .5f);
                            Vector3 c = new Vector3(x + .5f, -1, y + .5f);
                            Vector3 d = new Vector3(x - .5f, -1, y + .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for (int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                }
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        edgeObj = new GameObject("Edge");
        //edgeObj.transform.SetParent(transform);

        MeshFilter meshFilter = edgeObj.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = edgeObj.AddComponent<MeshRenderer>();
        meshRenderer.material = edgeMaterial;

        return mesh;
    }

    void DrawTexture(Cell[,] grid)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] colorMap = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];
                if (cell.isLava)
                    colorMap[y * size + x] = Color.red;
                else
                    colorMap[y * size + x] = Color.gray;
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(colorMap);
        texture.Apply();

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material = terrainMaterial;
            meshRenderer.material.mainTexture = texture;
        }
        else
        {
            Debug.LogError("MeshRenderer component not found on the 'Grid' GameObject");
        }
    }

    /*
    public void GenerateRocks(int numberOfRocks, float rockPlacementRadius)
    {
        // Generate rock positions
        List<Vector3> rockPositions = new List<Vector3>();

        for (int i = 0; i < numberOfRocks; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * rockPlacementRadius;
            randomPoint.y = terrain.SampleHeight(randomPoint);
            rockPositions.Add(randomPoint);
        }

        // Generate rock clusters
        RockClusterGenerator rockClusterGenerator = GetComponent<RockClusterGenerator>();
        rockClusterGenerator.GenerateRockClusters(rockPositions);
    }
    */

    void GenerateRocks(Cell[,] grid)
    {
        // Ensure at least 7 rocks
        int rockCount = 0;
        int maxRockCount = 15;
        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        while (rockCount < 7 && rockCount < maxRockCount)
        {
            rockCount = 0;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float noiseValue = Mathf.PerlinNoise(x * rockNoiseScale + xOffset, y * rockNoiseScale + yOffset);
                    noiseMap[x, y] = noiseValue;
                    Cell cell = grid[x, y];
                    if (!cell.isLava)
                    {
                        float v = Random.Range(0f, rockDensity);
                        if (noiseValue < v)
                        {
                            rockCount++;
                        }
                    }
                }
            }
            rockDensity *= 1.1f; // Increase rock density if needed
        }

        // Generate rocks with reasonable distance
        float minDistance = 2f;
        List<Vector2Int> rockPositions = new List<Vector2Int>();
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = noiseMap[x, y];
                Cell cell = grid[x, y];
                if (!cell.isLava && noiseValue < rockDensity)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    bool isTooClose = false;
                    foreach (Vector2Int existingPos in rockPositions)
                    {
                        if (Vector2Int.Distance(pos, existingPos) < minDistance)
                        {
                            isTooClose = true;
                            break;
                        }
                    }
                    if (!isTooClose)
                    {
                        GameObject prefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];
                        GameObject rock = Instantiate(prefab, transform);
                        rock.transform.position = new Vector3(x, 0, y);
                        rock.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                        rock.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f);
                        rockPositions.Add(pos);
                    }
                }
            }
        }
    }

    /*
    public void GenerateRocks(int numberOfRocks, float rockPlacementRadius, TerrainClass terrain)
    {
        // Generate rock positions
        List<Vector3> rockPositions = new List<Vector3>();

        for (int i = 0; i < numberOfRocks; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * rockPlacementRadius;
            randomPoint.y = terrain.SampleHeight(randomPoint);
            rockPositions.Add(randomPoint);
        }

        // Generate rock clusters
        RockClusterGenerator rockClusterGenerator = GetComponent<RockClusterGenerator>();
        rockClusterGenerator.GenerateRockClusters(rockPositions);
    }


    */

    Mesh DrawLavaMesh(Cell[,] grid)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];
                if (cell.isLava)
                {
                    Vector3 a = new Vector3(x - .5f, cell.Height, y + .5f);
                    Vector3 b = new Vector3(x + .5f, cell.Height, y + .5f);
                    Vector3 c = new Vector3(x - .5f, cell.Height, y - .5f);
                    Vector3 d = new Vector3(x + .5f, cell.Height, y - .5f);
                    Vector2 uvA = new Vector2(x / (float)size, y / (float)size);
                    Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                    Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                    Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);
                    Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                    Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                    for (int k = 0; k < 6; k++)
                    {
                        vertices.Add(v[k]);
                        triangles.Add(triangles.Count);
                        uvs.Add(uv[k]);
                    }
                }
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        GameObject lava = new GameObject("Lava", typeof(MeshFilter), typeof(MeshRenderer));
        lava.transform.SetParent(transform);

        MeshFilter meshFilter = lava.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = lava.GetComponent<MeshRenderer>();
        meshRenderer.material = lavaMaterial;

        return mesh;
    }


}

