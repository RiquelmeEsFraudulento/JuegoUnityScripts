using UnityEngine;
using System.Collections.Generic;
using System.Linq;



public class TerrainClass : MonoBehaviour
{
    public Terrain terrain;
    public UnityEngine.TerrainData terrainData;
    public float basemapDistance;
    public float[,] heightmap;
    private Cell[,] cells; // Updated variable name
    public int heightmapResolution = 513;
    public int splatmapResolution = 64;
    public float tileSize = 2000f;
    public int numTiles = 1;
    public Material terrainMaterial;
    public Texture2D[] splatmaps;

    public float heightScale = 5;
    public float detailScale = 5.0f;
    public int detailResolution = 1024;
    public int resolutionPerPatch = 32;
    public int alphamapResolution = 2048;
    float perlinScale = 0.1f;
    float perlinHeight = 0.1f;
    public int basemapResolution = 1024;
    public float[,] heights;
    public Texture2D[] textureMaps;
    private float[,,] textureMaps3; // Declare textureMaps here
    //int width = 1024;
    //int height = 1024;
    //int numTextures = 4;
    private Terrain[] terrains;


    public TerrainClass(int sizeX, int sizeY, float cellSize, float[,] heightmapE, float[,] noiseMap, Cell[,] cells, float edgeHeight, float shoreHeight)
    {
        

        textureMaps = new Texture2D[3];
        textureMaps[0] = new Texture2D(256, 256);
        textureMaps[1] = new Texture2D(256, 256);
        textureMaps[2] = new Texture2D(256, 256);
        // Create new terrain data
        terrainData = new UnityEngine.TerrainData();
        terrainData.size = new Vector3(100, heightScale, 100);
        terrainData.baseMapResolution = basemapResolution;
        heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        terrainData.heightmapResolution = sizeX + 1;
        terrainData.SetDetailResolution(sizeX + 1, 16);
        terrainData.alphamapResolution = sizeX + 1;
        terrainData.size = new Vector3(sizeX * cellSize, 600, sizeY * cellSize);
        float[,] heightmapEE = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        float[,] heightmapData = new float[sizeX + 1, sizeY + 1];
        for (int y = 0; y <= sizeY; y++)
        {
            for (int x = 0; x <= sizeX; x++)
            {
                heightmapData[x, y] = Mathf.PerlinNoise(x * perlinScale, y * perlinScale) * perlinHeight;
            }
        }
        terrainData.SetHeights(0, 0, heightmapE);


        // Create new terrain object
        GameObject terrainObject = Terrain.CreateTerrainGameObject(terrainData);
        terrainObject.name = "Terrain";

        // Set terrain object properties
        terrain = terrainObject.GetComponent<Terrain>();
        terrain.basemapDistance = 500;
        terrain.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
        terrain.materialTemplate = new Material(Shader.Find("Nature/Terrain/Diffuse"));

        // Apply textures based on height and slope
        float[,,] splatmaps = new float[sizeX + 1, sizeY + 1, 2];
        for (int x = 0; x <= sizeX; x++)
        {
            for (int y = 0; y <= sizeY; y++)
            {
                float height = heightmapE[x, y];
                float slope = Mathf.Abs(Mathf.Atan2(noiseMap[x, y], noiseMap[x, y]) * Mathf.Rad2Deg);

                if (height < shoreHeight)
                {
                    splatmaps[x, y, 0] = 1;
                }
                else if (height > edgeHeight || slope > 45)
                {
                    splatmaps[x, y, 1] = 1;
                }
                else
                {
                    float blend = (height - shoreHeight) / (edgeHeight - shoreHeight);
                    splatmaps[x, y, 0] = blend;
                    splatmaps[x, y, 1] = 1 - blend;
                }
            }
        }
        terrainData.SetAlphamaps(0, 0, splatmaps);

        // Set terrain position
        terrain.transform.position = new Vector3(0, 0, 0);
    }



    public void Initialize(int sizeX, int sizeY, float cellSize, float[,] heightmapE, float[,] noiseMap, Cell[,] cells, float edgeHeight, float shoreHeight)
    {


        textureMaps = new Texture2D[3];
        textureMaps[0] = new Texture2D(256, 256);
        textureMaps[1] = new Texture2D(256, 256);
        textureMaps[2] = new Texture2D(256, 256);
        // Create new terrain data
        terrainData = new UnityEngine.TerrainData();
        terrainData.size = new Vector3(100, heightScale, 100);
        terrainData.baseMapResolution = basemapResolution;
        heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        terrainData.heightmapResolution = sizeX + 1;
        terrainData.SetDetailResolution(sizeX + 1, 16);
        terrainData.alphamapResolution = sizeX + 1;
        terrainData.size = new Vector3(sizeX * cellSize, 600, sizeY * cellSize);
        float[,] heightmapEE = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        float[,] heightmapData = new float[sizeX + 1, sizeY + 1];
        for (int y = 0; y <= sizeY; y++)
        {
            for (int x = 0; x <= sizeX; x++)
            {
                heightmapData[x, y] = Mathf.PerlinNoise(x * perlinScale, y * perlinScale) * perlinHeight;
            }
        }
        terrainData.SetHeights(0, 0, heightmapE);


        // Create new terrain object
        GameObject terrainObject = Terrain.CreateTerrainGameObject(terrainData);
        terrainObject.name = "Terrain";

        // Set terrain object properties
        terrain = terrainObject.GetComponent<Terrain>();
        terrain.basemapDistance = 500;
        terrain.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
        terrain.materialTemplate = new Material(Shader.Find("Nature/Terrain/Diffuse"));

        // Apply textures based on height and slope
        float[,,] splatmaps = new float[sizeX + 1, sizeY + 1, 2];
        for (int x = 0; x <= sizeX; x++)
        {
            for (int y = 0; y <= sizeY; y++)
            {
                float height = heightmapE[x, y];
                float slope = Mathf.Abs(Mathf.Atan2(noiseMap[x, y], noiseMap[x, y]) * Mathf.Rad2Deg);

                if (height < shoreHeight)
                {
                    splatmaps[x, y, 0] = 1;
                }
                else if (height > edgeHeight || slope > 45)
                {
                    splatmaps[x, y, 1] = 1;
                }
                else
                {
                    float blend = (height - shoreHeight) / (edgeHeight - shoreHeight);
                    splatmaps[x, y, 0] = blend;
                    splatmaps[x, y, 1] = 1 - blend;
                }
            }
        }
        terrainData.SetAlphamaps(0, 0, splatmaps);

        // Set terrain position
        terrain.transform.position = new Vector3(0, 0, 0);

    }
    // Rest of the class methods...

    public void SetHeightmap(float[,] newHeightmap)
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain is not assigned!");
            return;
        }

        if (newHeightmap.GetLength(0) != terrain.terrainData.heightmapResolution || newHeightmap.GetLength(1) != terrain.terrainData.heightmapResolution)
        {
            Debug.LogError("New heightmap has different resolution from terrain's heightmap resolution!");
            return;
        }

        heightmap = newHeightmap;
        terrain.terrainData.SetHeights(0, 0, heightmap);
    }


    public float[,] GetHeightmap()
    {
        return heightmap;
    }


    public Vector3 GetTerrainSize()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain is not assigned!");
            return Vector3.zero;
        }

        return terrain.terrainData.size;
    }

    public Vector3 GetTerrainPosition()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain is not assigned!");
            return Vector3.zero;
        }

        return terrain.transform.position;
    }

    public Vector3 GetTerrainNormal(Vector3 position)
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain is not assigned!");
            return Vector3.zero;
        }

        // Convert the world position to a position relative to the terrain
        Vector3 terrainLocalPos = position - terrain.transform.position;

        // Get the normalized position of the point on the terrain
        float normX = terrainLocalPos.x / terrain.terrainData.size.x;
        float normZ = terrainLocalPos.z / terrain.terrainData.size.z;

        // Convert the normalized position to terrain coordinate values
        int coordX = Mathf.RoundToInt(normX * (terrain.terrainData.heightmapResolution - 1));
        int coordZ = Mathf.RoundToInt(normZ * (terrain.terrainData.heightmapResolution - 1));

        // Sample the terrain normal at the terrain coordinate values
        Vector3 normal = terrain.terrainData.GetInterpolatedNormal(normX, normZ);

        return normal;
    }

    public Vector2 GetTerrainTextureCoord(Vector3 position)
    {
        // Get the normalized terrain coordinates that correspond to the world position
        float normX = Mathf.InverseLerp(0, terrain.terrainData.size.x, position.x);
        float normZ = Mathf.InverseLerp(0, terrain.terrainData.size.z, position.z);

        // Get the terrain texture alpha value at the given coordinates
        float[,,] alphaMap = terrain.terrainData.GetAlphamaps(
            Mathf.FloorToInt(normX * (terrain.terrainData.alphamapWidth - 1)),
            Mathf.FloorToInt(normZ * (terrain.terrainData.alphamapHeight - 1)),
            1,
            1
        );
        float alpha = alphaMap[0, 0, 0];

        // Return the texture coordinate
        return new Vector2(alpha, 0f);
    }



    public void SetTerrainTexture(Vector3 position, int textureIndex)
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain is not assigned!");
            return;
        }

        // Convert the world position to a position relative to the terrain
        Vector3 terrainLocalPos = position - terrain.transform.position;

        // Get the normalized position of the point on the terrain
        float normX = terrainLocalPos.x / terrain.terrainData.size.x;
        float normZ = terrainLocalPos.z / terrain.terrainData.size.z;

        // Convert the normalized position to terrain coordinate values
        int coordX = Mathf.RoundToInt(normX * (terrain.terrainData.alphamapWidth - 1));
        int coordZ = Mathf.RoundToInt(normZ * (terrain.terrainData.alphamapHeight - 1));

        // Sample the current texture weights at the terrain coordinate values
        float[,,] alphaMaps = terrain.terrainData.GetAlphamaps(coordX, coordZ, 1, 1);

        // Set the new texture weight
        for (int i = 0; i < alphaMaps.GetLength(2); i++)
        {
            if (i == textureIndex)
            {
                alphaMaps[0, 0, i] = 1f;
            }
            else
            {
                alphaMaps[0, 0, i] = 0f;
            }
        }

        // Apply the changes to the terrain
        terrain.terrainData.SetAlphamaps(coordX, coordZ, alphaMaps);
    }

    public void SetTerrainTexture(Texture2D texture)
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain is not assigned!");
            return;
        }

        // Get the number of alpha layers in the terrain data
        int alphaLayerCount = terrain.terrainData.alphamapLayers;

        // Get the number of alpha maps needed to represent the texture
        int alphaMapCount = Mathf.CeilToInt((float)texture.width / terrain.terrainData.alphamapResolution) *
                           Mathf.CeilToInt((float)texture.height / terrain.terrainData.alphamapResolution);

        // Make sure there are enough alpha maps in the terrain data
        if (alphaLayerCount < alphaMapCount)
        {
            Debug.LogError("Not enough alpha maps in the terrain data!");
            return;
        }

        // Resize the texture to match the terrain size
        Texture2D resizedTexture = new Texture2D(terrain.terrainData.alphamapResolution * alphaMapCount,
                                                 terrain.terrainData.alphamapResolution * alphaMapCount);

        resizedTexture.filterMode = FilterMode.Point;
        resizedTexture.wrapMode = TextureWrapMode.Clamp;

        Graphics.ConvertTexture(texture, resizedTexture);

        // Create the alpha maps from the resized texture
        float[,,] alphaMaps = new float[terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight, alphaLayerCount];

        for (int i = 0; i < alphaLayerCount; i++)
        {
            for (int y = 0; y < terrain.terrainData.alphamapHeight; y++)
            {
                for (int x = 0; x < terrain.terrainData.alphamapWidth; x++)
                {
                    float xCoord = (float)x / (float)terrain.terrainData.alphamapWidth;
                    float yCoord = (float)y / (float)terrain.terrainData.alphamapHeight;

                    float pixelValue = resizedTexture.GetPixelBilinear(xCoord, yCoord).grayscale;

                    if (i == 0)
                    {
                        alphaMaps[x, y, i] = 1f - pixelValue;
                    }
                    else
                    {
                        alphaMaps[x, y, i] = pixelValue;
                    }
                }
            }
        }

        // Assign the alpha maps to the terrain data
        terrain.terrainData.SetAlphamaps(0, 0, alphaMaps);
    }


    public void SmoothTerrain(int iterations)
    {
        float[,] smoothHeights = new float[heightmap.GetLength(0), heightmap.GetLength(1)];

        for (int i = 0; i < iterations; i++)
        {
            for (int x = 1; x < heightmap.GetLength(0) - 1; x++)
            {
                for (int y = 1; y < heightmap.GetLength(1) - 1; y++)
                {
                    // Calculate the average height of the surrounding vertices
                    float avgHeight = (heightmap[x, y] +
                                       heightmap[x - 1, y] +
                                       heightmap[x + 1, y] +
                                       heightmap[x, y - 1] +
                                       heightmap[x, y + 1]) / 5f;

                    // Set the smoothed height
                    smoothHeights[x, y] = avgHeight;
                }
            }

            // Update the heightmap with the smoothed heights
            heightmap = smoothHeights;
        }

        Flush();
    }

    public void Flush()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain is not assigned!");
            return;
        }

        // Apply any changes made to the heightmap to the terrain
        terrain.terrainData.SetHeights(0, 0, heightmap);

        // Apply any changes made to the textures to the terrain
        if (textureMaps != null)
        {
            int alphamapWidth = terrain.terrainData.alphamapWidth;
            int alphamapHeight = terrain.terrainData.alphamapHeight;
            float[,,] textureMaps3 = new float[terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight, terrain.terrainData.alphamapLayers];

            int numTextures = textureMaps3.GetLength(2);

            float[,,] alphamap = new float[alphamapWidth, alphamapHeight, numTextures];

            // Define textureMaps as a 3D float array with dimensions matching the terrain's alphamap size and number of textures

            // Fill textureMaps with random values between 0 and 1
            for (int y = 0; y < terrain.terrainData.alphamapHeight; y++)
            {
                for (int x = 0; x < terrain.terrainData.alphamapWidth; x++)
                {
                    // Calculate the sum of all texture weights for this pixel
                    float sum = 0f;
                    for (int i = 0; i < terrain.terrainData.alphamapLayers; i++)
                    {
                        textureMaps3[x, y, i] = Random.value;
                        sum += textureMaps3[x, y, i];
                    }

                    // Normalize the texture weights so they sum up to 1
                    for (int i = 0; i < terrain.terrainData.alphamapLayers; i++)
                    {
                        textureMaps3[x, y, i] /= sum;
                    }
                }
            }


            terrain.terrainData.SetAlphamaps(0, 0, alphamap);
        }
    }


    public void Generate()
    {
        // Create an array to hold the terrains
        terrains = new Terrain[numTiles * numTiles];

        // Loop through each tile
        for (int x = 0; x < numTiles; x++)
        {
            for (int y = 0; y < numTiles; y++)
            {
                // Calculate the position of this tile
                float xPos = x * tileSize;
                float zPos = y * tileSize;

                // Create a new game object for the terrain
                GameObject terrainObject = new GameObject("Terrain " + (x * numTiles + y));
                terrainObject.transform.parent = transform;


                // Add a terrain component to the game object
                Terrain terrain = terrainObject.AddComponent<Terrain>();

                terrain.terrainData = new UnityEngine.TerrainData();
                terrain.terrainData.heightmapResolution = heightmapResolution;
                terrain.terrainData.size = new Vector3(tileSize, heightScale, tileSize);
                terrain.terrainData.SetDetailResolution(detailResolution, resolutionPerPatch);
                terrain.terrainData.alphamapResolution = alphamapResolution;
                terrain.terrainData.baseMapResolution = basemapResolution;

                // Set the splatmaps
                terrain.terrainData.terrainLayers = new TerrainLayer[splatmaps.Length];
                for (int i = 0; i < splatmaps.Length; i++)
                {
                    terrain.terrainData.terrainLayers[i] = new TerrainLayer();
                    terrain.terrainData.terrainLayers[i].diffuseTexture = splatmaps[i];
                    terrain.terrainData.terrainLayers[i].tileSize = new Vector2(tileSize, tileSize);
                }

                // Set the heights of the terrain
                heights = new float[heightmapResolution, heightmapResolution];
                for (int i = 0; i < heightmapResolution; i++)
                {
                    for (int j = 0; j < heightmapResolution; j++)
                    {
                        heights[i, j] = Mathf.PerlinNoise((float)i / heightmapResolution * 8f, (float)j / heightmapResolution * 8f);
                    }
                }
                terrain.terrainData.SetHeights(0, 0, heights);

                // Set the position and size of the terrain
                terrain.transform.position = new Vector3(xPos, 0f, zPos);
                terrain.transform.localScale = new Vector3(1f, 1f, 1f);
                terrain.materialTemplate = terrainMaterial;

                // Add the terrain to the array
                terrains[x * numTiles + y] = terrain;
            }
        }
    }

}