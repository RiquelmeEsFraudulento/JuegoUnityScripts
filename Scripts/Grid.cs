    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;

    public class Grid : MonoBehaviour
    {
        public float rockNoiseScale = .05f;
        public float rockDensity = .5f;
        public Material terrainMaterial;
        public Material edgeMaterial;
        public Material lavaMaterial;
        public Transform bulletSpawnPoint;
        public float lavaLevel = 0f;
        public float scale = .1f;
        public int size = 100;
        public float movementSpeed = 5f;
        public float shootForce = 100f;
        public GameObject bulletPrefab;
        public GameObject[] cameraPrefabs;
        public GameObject[] rockPrefabs;
        public GameObject[] fruitPrefabs;
        public GameObject[] gemPrefabs;
        public GameObject[] enemyPrefabs;
        public GameObject playerSpawn;
        public GameObject[] playerPrefabs;
        public GameObject edgeObj;
        public float bulletSpeed = 100f;
        public float mouseSensitivity = 2f;
        private float cameraRotationX = 0f;
        private GameObject playerObject;
        private Camera playerCamera;
        private Quaternion initialGunRotation;
        private GameObject gunObject;
        private int userScore;
        private List<Vector3> lavaVertices;
        private float lavaFlowSpeed = 1f;



    Cell[,] grid;

        void Start()
        {

            // Check if UserScore exists in PlayerPrefs
            if (PlayerPrefs.HasKey("UserScore"))
            {
                // Load the UserScore from PlayerPrefs
                userScore = PlayerPrefs.GetInt("UserScore");
            }
            else
            {
                // UserScore not found, set it to 0
                userScore = 0;
            }


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

            // Generate rocks
            int numberOfRocks = Random.Range(7, 11);
            float rockPlacementRadius = 1.5f;
            DrawTerrainMesh(grid);
            DrawEdgeMesh(grid);
            DrawTexture(grid);
            //GenerateRocks(numberOfRocks, rockPlacementRadius, terrain);
            GenerateRocks(grid);

            

            
    }


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

            string tagLand = "Land";
            land.tag = tagLand;
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



        Vector3 GetRandomPosition(Cell[,] grid)
        {
            while (true)
            {
                // Generate a random position within the grid
                int x = Random.Range(0, size);
                int y = Random.Range(0, size);

                // Check if the position is not in the lava
                Cell cell = grid[x, y];
                if (!cell.isLava)
                {
                    // Convert the grid position to world position
                    Vector3 position = new Vector3(x, 0, y);
                    return position;
                }
            }
        }



        void GenerateRocks(Cell[,] grid, bool interestingGrid = true)
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
                rockDensity *= 0.4f; // Increase rock density if needed
            }

            // Generate rocks with reasonable distance
            float minDistance = 10f;
            List<Vector3> rockPositions = new List<Vector3>();
            Vector3 farthestGemPosition = Vector3.zero; // Track the farthest gem position
            Vector3 previousGemPosition = Vector3.zero;
            Vector3 gemPosition = Vector3.zero;
            float maxGemPlayerDistance = 0f;


            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float noiseValue = noiseMap[x, y];
                    Cell cell = grid[x, y];
                    if (!cell.isLava && noiseValue < rockDensity)
                    {
                        Vector3 pos = new Vector3(x, 0, y);
                        bool isTooClose = false;
                        foreach (Vector3 existingPos in rockPositions)
                        {
                            if (Vector3.Distance(pos, existingPos) < minDistance)
                            {
                                isTooClose = true;
                                break;
                            }
                        }
                        if (!isTooClose)
                        {
                            if (cell.isGem)
                            {
                                float distanceToPlayer = Vector3.Distance(pos, playerSpawn.transform.position);
                                if (distanceToPlayer > maxGemPlayerDistance)
                                {
                                    farthestGemPosition = pos;
                                    maxGemPlayerDistance = distanceToPlayer;
                                    if (previousGemPosition != Vector3.zero)
                                    {
                                        // Set the previous farthest gem cell to false
                                        grid[Mathf.RoundToInt(previousGemPosition.x), Mathf.RoundToInt(previousGemPosition.z)].isGem = false;
                                    }
                                    previousGemPosition = farthestGemPosition;
                                }
                                else
                                {
                                    // Set the current cell as the gem cell
                                    cell.isGem = false;
                                }
                            }

                            GameObject prefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];
                            GameObject rock = Instantiate(prefab, transform);
                            rock.transform.position = pos;
                            rock.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                            rock.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f);
                            rockPositions.Add(pos);
                        }
                    }
                }
            }


            
            // Spawn fruits
            int maxFruits = 2;

            if(userScore > 10000){
                maxFruits = 1;
            }

            float minDistanceBetweenObjects = 2f;
            List<Vector3> fruitPositions = new List<Vector3>();
            for (int i = 0; i < maxFruits; i++)
            {
                bool validPosition = false;
                Vector3 fruitPosition = Vector3.zero;

                while (!validPosition)
                {
                    fruitPosition = GetRandomPosition(grid);

                    // Check if the fruit position is too close to any rocks
                    bool tooCloseToRocks = false;
                    foreach (Vector3 rockPos in rockPositions)
                    {
                        float distance = Vector3.Distance(fruitPosition, rockPos);
                        if (distance < minDistanceBetweenObjects)
                        {
                            tooCloseToRocks = true;
                            break;
                        }
                    }

                    // Check if the fruit position is too close to other fruits
                    foreach (Vector3 existingFruitPos in fruitPositions)
                    {
                        float distanceToFruit = Vector3.Distance(fruitPosition, existingFruitPos);
                        if (distanceToFruit < minDistanceBetweenObjects)
                        {
                            tooCloseToRocks = true;
                            break;
                        }
                    }

                    // Check if the fruit position is too close to the player spawn point
                    if (Vector3.Distance(fruitPosition, playerSpawn.transform.position) < minDistanceBetweenObjects)
                    {
                        tooCloseToRocks = true;
                    }

                    validPosition = !tooCloseToRocks;
                }

                GameObject fruitPrefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];
                GameObject fruit = Instantiate(fruitPrefab, transform);
                fruit.transform.position = fruitPosition;
                fruitPositions.Add(fruitPosition);
            }

            // Spawn gem
            float minDistanceToGem = 5f;
            bool validGemPosition = false;

            while (!validGemPosition)
            {
                gemPosition = GetRandomPosition(grid);

                // Check if the gem position is too close to any rocks
                bool tooCloseToRocks = false;
                foreach (Vector3 rockPos in rockPositions)
                {
                    float distance = Vector3.Distance(gemPosition, rockPos);
                    if (distance < minDistanceToGem)
                    {
                        tooCloseToRocks = true;
                        break;
                    }
                }

                // Check if the gem position is too close to other fruits
                foreach (Vector3 existingFruitPos in fruitPositions)
                {
                    float distanceToFruit = Vector3.Distance(gemPosition, existingFruitPos);
                    if (distanceToFruit < minDistanceToGem)
                    {
                        tooCloseToRocks = true;
                        break;
                    }
                }

                // Check if the gem position is too close to the player spawn point
                if (Vector3.Distance(gemPosition, playerSpawn.transform.position) < minDistanceToGem)
                {
                    tooCloseToRocks = true;
                }

                validGemPosition = !tooCloseToRocks;
            }

            GameObject gemPrefab = gemPrefabs[Random.Range(0, gemPrefabs.Length)];
            GameObject gemObject = Instantiate(gemPrefab, transform);
            gemObject.transform.position = gemPosition;

            // Randomly choose a player prefab
            GameObject selectedPlayerPrefab = playerPrefabs[Random.Range(0, playerPrefabs.Length)];

            // Spawn the player at the player spawn point
            Vector3 playerSpawnPosition = GetRandomPosition(grid);
            bool validPlayerSpawnPosition = false;

            while (!validPlayerSpawnPosition)
            {
                // Check if the player spawn position is too close to any rocks
                bool tooCloseToRocks = false;
                foreach (Vector3 rockPos in rockPositions)
                {
                    float distance = Vector3.Distance(playerSpawnPosition, rockPos);
                    if (distance < minDistanceBetweenObjects)
                    {
                        tooCloseToRocks = true;
                        break;
                    }
                }

                // Check if the player spawn position is too close to any fruits
                foreach (Vector3 fruitPos in fruitPositions)
                {
                    float distance = Vector3.Distance(playerSpawnPosition, fruitPos);
                    if (distance < minDistanceBetweenObjects)
                    {
                        tooCloseToRocks = true;
                        break;
                    }
                }

                // Check if the player spawn position is too close to the gem
                if (Vector3.Distance(playerSpawnPosition, gemPosition) < minDistanceBetweenObjects)
                {
                    tooCloseToRocks = true;
                }

                validPlayerSpawnPosition = !tooCloseToRocks;
            }


            playerObject = Instantiate(selectedPlayerPrefab, transform);
            Vector3 elevatedPosition = playerSpawnPosition + new Vector3(0f, 0f, 0f);
            playerObject.transform.position = elevatedPosition;
            playerCamera = playerObject.GetComponentInChildren<Camera>();





            // Spawn enemies
            int minEnemies = 4;
            int maxEnemies = 6;

            if(userScore > 0){

                minEnemies = Mathf.RoundToInt(4 + Mathf.RoundToInt(userScore / 2000));
                maxEnemies = Mathf.RoundToInt(6 + Mathf.RoundToInt(userScore / 2000));

                if (minEnemies > 13)
                {
                    minEnemies = 13;
                    maxEnemies = 15;
                }
            }

            float minDistanceBetweenEnemies = 2f;
            float minDistanceBetweenGemPlayer = 30f;
            List<Vector3> enemyPositions = new List<Vector3>();
            int enemyCount = Random.Range(minEnemies, maxEnemies + 1);

            for (int i = 0; i < enemyCount; i++)
            {
                bool validPosition = false;
                Vector3 enemyPosition = Vector3.zero;

                while (!validPosition)
                {
                    enemyPosition = GetRandomPosition(grid);

                    // Check if the enemy position is too close to any rocks, fruits, gem, player spawn point, or other enemies
                    bool tooCloseToRocks = false;
                    foreach (Vector3 rockPos in rockPositions)
                    {
                        float distance = Vector3.Distance(enemyPosition, rockPos);
                        if (distance < minDistanceBetweenEnemies)
                        {
                            tooCloseToRocks = true;
                            break;
                        }
                    }

                    foreach (Vector3 fruitPos in fruitPositions)
                    {
                        float distance = Vector3.Distance(enemyPosition, fruitPos);
                        if (distance < minDistanceBetweenEnemies)
                        {
                            tooCloseToRocks = true;
                            break;
                        }
                    }

                    if (Vector3.Distance(enemyPosition, gemPosition) < minDistanceBetweenEnemies)
                    {
                        tooCloseToRocks = true;
                    }

                    if (Vector3.Distance(enemyPosition, playerSpawn.transform.position) < minDistanceBetweenEnemies)
                    {
                        tooCloseToRocks = true;
                    }

                    foreach (Vector3 existingEnemyPos in enemyPositions)
                    {
                        float distanceToEnemy = Vector3.Distance(enemyPosition, existingEnemyPos);
                        if (distanceToEnemy < minDistanceBetweenEnemies)
                        {
                            tooCloseToRocks = true;
                            break;
                        }
                    }


                    float distanceToGem = Vector3.Distance(playerObject.transform.position, gemObject.transform.position);

                    Debug.Log("The value is: " + distanceToGem);


                    if (distanceToGem < minDistanceBetweenGemPlayer)
                    {
                        interestingGrid = false;

                        Debug.Log("ENTRO PELOTUDO DISTANCIA");



                        // Destroy gem
                        if (gemObject != null)
                        {
                            Destroy(gemObject);
                        }

                        // Destroy player
                        if (playerObject != null)
                        {
                            Destroy(playerObject);
                        }

                        enemyPositions = null;
                        rockPositions = null;
                        fruitPositions = null;


                        ResetGrid(grid);
                        GenerateRocks(grid, true); // Generate with interestingGrid flag set to true
                        return;

                    }


                    validPosition = !tooCloseToRocks;

                    if (!validPosition)
                    {

                        Debug.Log("ENTRO PELOTUDO");

                        // Destroy gem
                        if (gemObject != null)
                        {
                            Destroy(gemObject);

                        }

                        // Destroy player
                        if (playerObject != null)
                        {
                            Destroy(playerObject);
                        }

                        enemyPositions = null;
                        rockPositions = null;
                        fruitPositions = null;

                        ResetGrid(grid);
                        GenerateRocks(grid, true); // Generate with interestingGrid flag set to true
                        return;
                    }
                }

                // Spawn the enemy
                GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                GameObject enemyObject = Instantiate(enemyPrefab, transform);
                enemyObject.transform.position = enemyPosition;
                //grid[Mathf.RoundToInt(enemyPosition.x), Mathf.RoundToInt(enemyPosition.y)].SetEnemy(enemyObject);
                enemyPositions.Add(enemyPosition);
            }

        
        gunObject = GameObject.FindGameObjectWithTag("Gun");
        initialGunRotation = gunObject.transform.localRotation;
    }



        void ResetGrid(Cell[,] grid)
        {
            int numRows = grid.GetLength(0);
            int numColumns = grid.GetLength(1);

            // Reset cell properties
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numColumns; col++)
                {
                    grid[row, col].IsVisited = false;
                    grid[row, col].IsEmpty = true;
                    grid[row, col].IsRock = false;
                    //grid[row, col].isLava = false;
                    grid[row, col].isGem = false;
                    //grid[row, col].Player = null;
                    //grid[row, col].Enemy = null;
                    //grid[row, col].Neighbors.Clear(); // Clear neighbors list
                }
            }
        }
    /*
    void Update()
    {
        // Player movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * movementSpeed * Time.deltaTime;
        transform.Translate(movement);



        // Shooting
        
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }


    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bulletRigidbody.AddForce(bulletSpawnPoint.forward * shootForce, ForceMode.Impulse);
    }


    */


    void Update()
    {
        // Player movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical) * movementSpeed * Time.deltaTime;
        playerObject.transform.Translate(movement);



        if (Input.GetButtonDown("Jump"))
        {
            // Calculate the jump direction
            Vector3 jumpDirection = new Vector3(moveHorizontal, 0f, moveVertical).normalized;

            // Calculate the horizontal jump distance
            float horizontalJumpDistance = 0.25f;

            // Calculate the vertical jump height
            float verticalJumpHeight = 0.25f;

            // Apply the jump force
            Vector3 jumpForce = jumpDirection * horizontalJumpDistance / Time.fixedDeltaTime;
            jumpForce.y = Mathf.Sqrt(2f * verticalJumpHeight * Mathf.Abs(Physics.gravity.y));
            playerObject.GetComponent<Rigidbody>().AddForce(jumpForce, ForceMode.VelocityChange);
        }


        // Camera rotation
        float mouseMovementX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseMovementY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        cameraRotationX -= mouseMovementY;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90f, 90f);

        //cameraRotationY -= mouseMovementX;
        //cameraRotationY = Mathf.Clamp(cameraRotationY, 90f, -90f);


        playerCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);
        playerObject.transform.Rotate(Vector3.up * mouseMovementX);



        if (gunObject == null)
        {
           gunObject = GameObject.FindGameObjectWithTag("Gun");
        }
        //gunObject.transform.localRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);
        //gunObject.transform.Rotate(Vector3.up * mouseMovementX);


        //bulletSpawnPoint = playerObject.transform.Find("Gun");

        //bulletSpawnPoint.rotation = rotation

        //Debug.Log("Initial Gun Rotation: " + initialGunRotation);

        /*
        gunObject.transform.localRotation = initialGunRotation * Quaternion.Euler(mouseMovementY, 0f, 0f);

        */
        //Debug.Log("GunObject: " + gunObject);
        /*
        if (Input.GetButtonDown("Fire1"))
        {
            bulletSpawnPoint = playerCamera.transform;

            Shoot();
        }

        */

    }


    void Shoot()
    {

        /*
        GameObject bulletSpawnPointGO = GameObject.FindGameObjectWithTag("GunPoint");
        GameObject Crosshair = GameObject.FindGameObjectWithTag("Crosshair");
        if (bulletSpawnPointGO != null)
        {
            Transform bulletSpawnPoint = bulletSpawnPointGO.transform;

            Vector3 crosshairPosition = Camera.main.ScreenToWorldPoint(Crosshair.transform.position);

            Vector3 direction = crosshairPosition - bulletSpawnPoint.position;




            //GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position + new Vector3(0.668f, -0.21f, 0.5f), bulletSpawnPoint.rotation * Quaternion.Euler(0f, 0f, 30f));
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            //Vector3 weapon1pos = new Vector3(0.668f, -0.21f, 0.5f);
            //weapon1rot = new Quaternion.Euler(0f, 0f, 30f);
            //Vector3 targetPosition = bullet.transform.TransformPoint(weapon1pos);
            //bullet.transform.position = targetPosition;
            //bullet.transform.localRotation = Quaternion.Euler(0f, 0f, 30f);

            //bullet.transform.rotation = Quaternion.LookRotation(direction);
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            bulletRigidbody.AddForce(direction.normalized * 20f, ForceMode.Impulse);
        } 

        
        */
    }

    /*
    void OnCollisionEnter(Collision collision)
    {

        Debug.Log("Posible muerte?");


        GameObject collidedObject = collision.gameObject;
        if (collidedObject.CompareTag("Enemy"))
        {
            Debug.Log("El enemigo ha sido muerto");
            // Remove the enemy object
            Destroy(collidedObject);
        }
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
                    // Calculate the lava flow direction and intensity based on the cell properties
                    Vector3 lavaFlowDirection = cell.lavaFlowDirection.normalized;
                    float lavaFlowIntensity = cell.lavaFlowIntensity;

                    // Calculate the height of the lava at each corner of the cell
                    float aHeight = cell.Height - 0.5f + lavaFlowIntensity;
                    float bHeight = cell.Height - 0.5f + lavaFlowIntensity;
                    float cHeight = cell.Height - 0.5f;
                    float dHeight = cell.Height - 0.5f;

                    // Offset the vertices in the lava flow direction to create a flowing effect
                    Vector3 a = new Vector3(x - .5f, aHeight, y + .5f) + lavaFlowDirection;
                    Vector3 b = new Vector3(x + .5f, bHeight, y + .5f) + lavaFlowDirection;
                    Vector3 c = new Vector3(x - .5f, cHeight, y - .5f);
                    Vector3 d = new Vector3(x + .5f, dHeight, y - .5f);

                    // Calculate the UV coordinates based on the cell position
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

        // Instantiate the lava object
        GameObject lava = new GameObject("Lava", typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider), typeof(UnityEngine.AI.NavMeshObstacle));
        lava.transform.SetParent(transform);

        MeshFilter meshFilter = lava.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = lava.GetComponent<MeshRenderer>();
        meshRenderer.material = lavaMaterial;

        BoxCollider collider = lava.GetComponent<BoxCollider>();
        collider.size = new Vector3(size, 5f, size);
        collider.center = new Vector3(size / 2f - 0.5f, 0f, size / 2f - 0.5f);
        collider.isTrigger = true;

        UnityEngine.AI.NavMeshObstacle navMeshObstacle = lava.GetComponent<UnityEngine.AI.NavMeshObstacle>();
        navMeshObstacle.carving = true;

        string tagHot = "Hot";
        lava.tag = tagHot;
        collider.gameObject.tag = tagHot;

        return mesh;
    }

}

