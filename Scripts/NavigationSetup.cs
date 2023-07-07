using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class NavigationSetup : MonoBehaviour
{
    public GameObject agentPrefab; // Prefab for the agent
    public float searchRadius = 100f; // Radius within which to search for the agent transform
    public int walkableAreaIndex = 1; // Index for the walkable area

    private NavMeshSurface navMeshSurface; // Reference to the NavMeshSurface component

    private List<GameObject> dynamicObjects; // List to hold dynamically moving objects

    void Start()
    {
        // Create a new instance of NavMeshSurface
        navMeshSurface = gameObject.AddComponent<NavMeshSurface>();

        // Set the build settings for the NavMeshSurface
        navMeshSurface.collectObjects = CollectObjects.Volume;
        navMeshSurface.defaultArea = 0; // Define the default area index
        navMeshSurface.overrideTileSize = true;
        navMeshSurface.tileSize = 3;

        Vector3 terrainSize = new Vector3(100f, 100f, 100f);
        navMeshSurface.size = terrainSize;

        // Attach the NavMeshSurface component to the agentPrefab
        navMeshSurface.agentTypeID = agentPrefab.GetComponent<NavMeshAgent>().agentTypeID;

        // Generate the NavMesh at runtime
        navMeshSurface.BuildNavMesh();

        // Include existing objects in the NavMesh generation
        IncludeExistingObjects();

        // Initialize the dynamicObjects list
        dynamicObjects = new List<GameObject>();
    }

    void IncludeExistingObjects()
    {
        // Find all existing objects with colliders in the scene
        Collider[] colliders = FindObjectsOfType<Collider>();

        // Create a new list of NavMeshBuildSource to hold the existing objects
        List<NavMeshBuildSource> buildSources = new List<NavMeshBuildSource>();

        // Iterate through the colliders and add them as NavMeshBuildSources
        foreach (Collider collider in colliders)
        {
            // Ignore the agentPrefab and any other objects you don't want to include
            if (collider.gameObject == agentPrefab)
                continue;

            if (collider.tag != "Headshot")
            {
                if (collider is MeshCollider meshCollider && meshCollider.sharedMesh != null)
                {
                    // Create a new NavMeshBuildSource for the meshCollider
                    NavMeshBuildSource buildSource = new NavMeshBuildSource();
                    buildSource.shape = NavMeshBuildSourceShape.Mesh;
                    buildSource.sourceObject = meshCollider.sharedMesh;
                    buildSource.transform = meshCollider.transform.localToWorldMatrix;

                    // Set the area index based on the collider's tag
                    if (collider.CompareTag("Land"))
                    {
                        buildSource.area = walkableAreaIndex; // Set the custom area index for the walkable area
                    }
                    else if (collider.CompareTag("Lava"))
                    {
                        // Make the lava collider and space above it as not walkable
                        buildSource.area = NavMesh.GetAreaFromName("Not Walkable"); // Set the area to "Not Walkable"
                    }

                    // Add the buildSource to the list
                    buildSources.Add(buildSource);

                    // Check if the collider's GameObject is dynamically moving
                    if (!meshCollider.gameObject.isStatic)
                    {
                        // Add the GameObject to the dynamicObjects list
                        dynamicObjects.Add(meshCollider.gameObject);
                    }
                }
                else if (collider is TerrainCollider terrainCollider && terrainCollider.terrainData != null)
                {
                    // Create a new NavMeshBuildSource for the terrainCollider
                    NavMeshBuildSource buildSource = new NavMeshBuildSource();
                    buildSource.shape = NavMeshBuildSourceShape.Terrain;
                    buildSource.sourceObject = terrainCollider.terrainData;
                    buildSource.transform = terrainCollider.transform.localToWorldMatrix;

                    // Set the area index based on the collider's tag
                    if (collider.CompareTag("Land"))
                    {
                        buildSource.area = walkableAreaIndex; // Set the custom area index for the walkable area
                    }
                    else if (collider.CompareTag("Lava"))
                    {
                        // Make the lava collider and space above it as not walkable
                        buildSource.area = NavMesh.GetAreaFromName("Not Walkable"); // Set the area to "Not Walkable"
                    }

                    // Add the buildSource to the list
                    buildSources.Add(buildSource);
                }
                else
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = collider.gameObject.GetComponent<SkinnedMeshRenderer>();
                    if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null)
                    {
                        // Get the shared mesh of the SkinnedMeshRenderer
                        Mesh skinnedMesh = new Mesh();
                        skinnedMeshRenderer.BakeMesh(skinnedMesh);

                        // Create a new NavMeshBuildSource for the skinned mesh
                        NavMeshBuildSource buildSource = new NavMeshBuildSource();
                        buildSource.shape = NavMeshBuildSourceShape.Mesh;
                        buildSource.sourceObject = skinnedMesh;
                        buildSource.transform = skinnedMeshRenderer.transform.localToWorldMatrix;

                        // Set the area index based on the collider's tag
                        if (collider.CompareTag("Land"))
                        {
                            buildSource.area = walkableAreaIndex; // Set the custom area index for the walkable area
                        }
                        else if (collider.CompareTag("Lava"))
                        {
                            // Make the lava collider and space above it as not walkable
                            buildSource.area = NavMesh.GetAreaFromName("Not Walkable"); // Set the area to "Not Walkable"
                        }

                        // Add the buildSource to the list
                        buildSources.Add(buildSource);

                        // Check if the renderer's GameObject is dynamically moving
                        if (!skinnedMeshRenderer.gameObject.isStatic)
                        {
                            // Add the GameObject to the dynamicObjects list
                            dynamicObjects.Add(skinnedMeshRenderer.gameObject);
                        }
                    }
                }
            }
        }

        // Build the NavMesh with the specified build sources
        NavMeshData navMeshData = NavMeshBuilder.BuildNavMeshData(navMeshSurface.GetBuildSettings(), buildSources, new Bounds(transform.position, transform.lossyScale), Vector3.zero, Quaternion.identity);

        // Assign the NavMesh data to the NavMeshSurface
        navMeshSurface.navMeshData = navMeshData;
    }





    void Update()
    {
        // Check for changes in dynamic objects and update the NavMesh
        foreach (GameObject dynamicObject in dynamicObjects)
        {
            // Check if the object's position or shape has changed
            if (HasObjectChanged(dynamicObject))
            {
                // Rebuild the NavMesh
                navMeshSurface.BuildNavMesh();
                break; // Exit the loop after rebuilding once
            }
        }
    }

    bool HasObjectChanged(GameObject obj)
    {
        // Check if the object's position has changed
        if (obj.transform.hasChanged)
        {
            obj.transform.hasChanged = false;
            return true;
        }

        // Check if the object's shape has changed (e.g., for skinned meshes)
        SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null && skinnedMeshRenderer.sharedMesh.isReadable)
        {
            if (skinnedMeshRenderer.sharedMesh.vertexCount != skinnedMeshRenderer.sharedMesh.vertexCount)
                return true;

            Vector3[] currentVertices = skinnedMeshRenderer.sharedMesh.vertices;
            Vector3[] storedVertices = skinnedMeshRenderer.sharedMesh.vertices;

            for (int i = 0; i < currentVertices.Length; i++)
            {
                if (currentVertices[i] != storedVertices[i])
                    return true;
            }
        }

        // Recursively check the children for SkinnedMeshRenderer components
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            if (HasObjectChanged(obj.transform.GetChild(i).gameObject))
                return true;
        }

        return false;
    }

}
