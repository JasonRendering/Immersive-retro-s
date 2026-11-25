using System.Collections.Generic;
using UnityEngine;

public class BackroomsGenerator : MonoBehaviour
{
    [Header("Maze Settings")]
    [Min(2)] public int width = 20;
    [Min(2)] public int depth = 20;
    [Min(2f)] public float cellSize = 6f;

    [Header("Building Settings")]
    [Min(1f)] public float wallHeight = 3.5f;
    [Min(0.05f)] public float wallThickness = 0.2f;
    public bool generateCeiling = true;

    [Header("Architecture Settings")]
    [Range(1, 30)] public int largeRoomCount = 12;
    [Range(0, 100)] public int extraOpenings = 25;
    [Range(2, 5)] public int maxRoomSize = 4;

    [Header("Pillar Settings")]
    public bool generatePillars = true;
    [Range(0, 100)] public int pillarChance = 65;
    [Min(0.2f)] public float pillarSize = 0.5f;
    [Min(0.5f)] public float pillarSpacing = 6f;

    [Header("Lighting Settings")]
    public bool generateLights = true;
    [Range(1, 10)] public int lightSpacing = 2;
    [Range(0.5f, 5f)] public float lightIntensity = 2f;
    [Range(5f, 60f)] public float lightRange = 20f;
    public bool enableFlickering = true;

    [Header("Fluorescent Appearance")]
    public Material lightHousingMaterial;
    public Material lightDiffuserMaterial;

    [Header("Fluorescent Audio")]
    public AudioClip fluorescentBuzz;
    [Range(0f, 1f)] public float buzzVolume = 0.15f;

    [Header("Materials")]
    public Material wallMaterial;
    public Material floorMaterial;
    public Material ceilingMaterial;

    private const int North = 0;
    private const int East = 1;
    private const int South = 2;
    private const int West = 3;

    private class Cell
    {
        public bool[] walls = { true, true, true, true };
        public bool visited;
    }

    private Cell[,] cells;

    [ContextMenu("Generate Level 0")]
    public void GenerateLevel()
    {
    ClearLevel();

    width = Mathf.Max(2, width);
    depth = Mathf.Max(2, depth);

    cells = new Cell[width, depth];

    for (int x = 0; x < width; x++)
    {
        for (int z = 0; z < depth; z++)
        {
            cells[x, z] = new Cell();
        }
    }

    GenerateMaze();
    BuildLevel();

        Debug.Log("Backrooms Level 0 generated!");
    }
    private void GenerateMaze()
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        Vector2Int current = new Vector2Int(0, 0);
        cells[0, 0].visited = true;

        while (true)
        {
            List<int> available = new List<int>();

            if (current.y < depth - 1 &&
                !cells[current.x, current.y + 1].visited)
                available.Add(North);

            if (current.x < width - 1 &&
                !cells[current.x + 1, current.y].visited)
                available.Add(East);

            if (current.y > 0 &&
                !cells[current.x, current.y - 1].visited)
                available.Add(South);

            if (current.x > 0 &&
                !cells[current.x - 1, current.y].visited)
                available.Add(West);

            if (available.Count > 0)
            {
                int direction = available[
                    Random.Range(0, available.Count)];

                Vector2Int next = current;

                switch (direction)
                {
                    case North: next.y++; break;
                    case East: next.x++; break;
                    case South: next.y--; break;
                    case West: next.x--; break;
                }

                cells[current.x, current.y].walls[direction] = false;

                int opposite = (direction + 2) % 4;
                cells[next.x, next.y].walls[opposite] = false;

                stack.Push(current);

                current = next;
                cells[current.x, current.y].visited = true;
            }
            else if (stack.Count > 0)
            {
                current = stack.Pop();
            }
            else
            {
                break;
            }
        }

        CreateLargeRooms();
        CreateExtraOpenings();
    }

    private void CreateLargeRooms()
    {
        for (int i = 0; i < largeRoomCount; i++)
        {
            int roomWidth = Random.Range(2, maxRoomSize + 1);
            int roomDepth = Random.Range(2, maxRoomSize + 1);

            if (roomWidth > width || roomDepth > depth)
                continue;

            int startX = Random.Range(0, width - roomWidth + 1);
            int startZ = Random.Range(0, depth - roomDepth + 1);

            for (int x = startX; x < startX + roomWidth; x++)
            {
                for (int z = startZ; z < startZ + roomDepth; z++)
                {
                    if (x < startX + roomWidth - 1)
                    {
                        cells[x, z].walls[East] = false;
                        cells[x + 1, z].walls[West] = false;
                    }

                    if (z < startZ + roomDepth - 1)
                    {
                        cells[x, z].walls[North] = false;
                        cells[x, z + 1].walls[South] = false;
                    }
                }
            }
        }
    }

    private void CreateExtraOpenings()
    {
        for (int i = 0; i < extraOpenings; i++)
        {
            int x = Random.Range(0, width);
            int z = Random.Range(0, depth);

            int direction = Random.Range(0, 4);

            int nextX = x;
            int nextZ = z;

            switch (direction)
            {
                case North: nextZ++; break;
                case East: nextX++; break;
                case South: nextZ--; break;
                case West: nextX--; break;
            }

            if (nextX < 0 || nextX >= width ||
                nextZ < 0 || nextZ >= depth)
                continue;

            cells[x, z].walls[direction] = false;
            cells[nextX, nextZ].walls[(direction + 2) % 4] = false;
        }
    }

    private void BuildLevel()
    {
        Transform root = transform.Find("Generated_Level");

if (root == null)
{
    root = new GameObject("Generated_Level").transform;
    root.SetParent(transform, false);
}

        float totalWidth = width * cellSize;
        float totalDepth = depth * cellSize;

        CreateCube(
            "Floor",
            new Vector3(totalWidth / 2f, -0.1f, totalDepth / 2f),
            new Vector3(totalWidth, 0.2f, totalDepth),
            floorMaterial,
            root
        );

        if (generateCeiling)
        {
            CreateCube(
                "Ceiling",
                new Vector3(totalWidth / 2f, wallHeight + 0.1f, totalDepth / 2f),
                new Vector3(totalWidth, 0.2f, totalDepth),
                ceilingMaterial,
                root
            );
        }

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Cell cell = cells[x, z];

                float centerX = (x + 0.5f) * cellSize;
                float centerZ = (z + 0.5f) * cellSize;

                if (x == 0 && cell.walls[West])
                {
                    CreateWall(centerX - cellSize / 2f, centerZ, true, root);
                }

                if (z == 0 && cell.walls[South])
                {
                    CreateWall(centerX, centerZ - cellSize / 2f, false, root);
                }

                if (cell.walls[East])
                {
                    CreateWall(centerX + cellSize / 2f, centerZ, true, root);
                }

                if (cell.walls[North])
                {
                    CreateWall(centerX, centerZ + cellSize / 2f, false, root);
                }
            }
        }

        GeneratePillars(root);
        GenerateLights(root);
    }

    private void CreateWall(float x, float z, bool vertical, Transform root)
    {
        Vector3 position = new Vector3(x, wallHeight / 2f, z);

        Vector3 scale = vertical
            ? new Vector3(wallThickness, wallHeight, cellSize)
            : new Vector3(cellSize, wallHeight, wallThickness);

        CreateCube("Wall", position, scale, wallMaterial, root);
    }

    private void CreateCube(
        string objectName,
        Vector3 position,
        Vector3 scale,
        Material material,
        Transform parent)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

        obj.name = objectName;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = position;
        obj.transform.localScale = scale;

        if (material != null)
        {
            obj.GetComponent<Renderer>().sharedMaterial = material;
        }
    }

    
private void GeneratePillars(Transform root)
{
    if (!generatePillars)
        return;

    for (int x = 0; x < width; x++)
    {
        for (int z = 0; z < depth; z++)
        {
            Cell cell = cells[x, z];

            int openSides = 0;

            for (int i = 0; i < 4; i++)
            {
                if (!cell.walls[i])
                    openSides++;
            }

            // Only place pillars in open areas
            if (openSides < 3)
                continue;

            // Random pillar chance
            if (Random.Range(0, 100) >= pillarChance)
                continue;

            // Keep the starting area clear
            if (x == 0 && z == 0)
                continue;

            // Control pillar spacing
            int pillarStep = Mathf.Max(
                1,
                Mathf.RoundToInt(pillarSpacing / cellSize)
            );

            if (x % pillarStep != 0 || z % pillarStep != 0)
                continue;

            // Don't place pillars where lights spawn
            if (generateLights &&
                x % lightSpacing == 0 &&
                z % lightSpacing == 0)
                continue;

            float centerX = (x + 0.5f) * cellSize;
            float centerZ = (z + 0.5f) * cellSize;

            CreateCube(
                "Backrooms_Pillar",
                new Vector3(centerX, wallHeight / 2f, centerZ),
                new Vector3(pillarSize, wallHeight, pillarSize),
                wallMaterial,
                root
            );
        }
    }
}


    private void GenerateLights(Transform root)
    {
        if (!generateLights)
            return;

        for (int x = 0; x < width; x += lightSpacing)
        {
            for (int z = 0; z < depth; z += lightSpacing)
            {
                float centerX = (x + 0.5f) * cellSize;
                float centerZ = (z + 0.5f) * cellSize;

                GameObject fixture = new GameObject("Fluorescent_Fixture");

                fixture.transform.SetParent(root, false);
                fixture.transform.localPosition = new Vector3(centerX, 0f, centerZ);

                CreateLightPart(
                    "Light_Housing",
                    new Vector3(0f, wallHeight - 0.09f, 0f),
                    new Vector3(2.4f, 0.12f, 0.9f),
                    lightHousingMaterial,
                    fixture.transform
                );

                CreateLightPart(
                    "Light_Diffuser",
                    new Vector3(0f, wallHeight - 0.17f, 0f),
                    new Vector3(2.15f, 0.025f, 0.65f),
                    lightDiffuserMaterial,
                    fixture.transform
                );

                GameObject lightObject = new GameObject("Fluorescent_Light");

                lightObject.transform.SetParent(fixture.transform, false);
                lightObject.transform.localPosition =
                    new Vector3(0f, wallHeight - 0.30f, 0f);

                Light lightSource = lightObject.AddComponent<Light>();

                lightSource.type = LightType.Point;

                lightSource.color = new Color(
                    Random.Range(0.90f, 1f),
                    Random.Range(0.92f, 1f),
                    1f
                );

                lightSource.intensity = Random.Range(
                    lightIntensity * 0.8f,
                    lightIntensity * 1.2f
                );

                lightSource.range = lightRange;
                lightSource.shadows = LightShadows.None;

                if (enableFlickering)
                {
                    lightObject.AddComponent<BackroomsLightFlicker>();
                }

                if (fluorescentBuzz != null)
                {
                    AudioSource audioSource =
                        fixture.AddComponent<AudioSource>();

                    audioSource.clip = fluorescentBuzz;
                    audioSource.loop = true;
                    audioSource.playOnAwake = true;
                    audioSource.volume = buzzVolume;
                    audioSource.spatialBlend = 0.7f;
                    audioSource.minDistance = 2f;
                    audioSource.maxDistance = 12f;
                }
            }
        }
    }

    private void CreateLightPart(
        string partName,
        Vector3 position,
        Vector3 scale,
        Material material,
        Transform parent)
    {
        GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);

        part.name = partName;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = position;
        part.transform.localScale = scale;

        Collider col = part.GetComponent<Collider>();

        if (col != null)
        {
            if (Application.isPlaying)
                Destroy(col);
            else
                DestroyImmediate(col);
        }

        if (material != null)
        {
            part.GetComponent<Renderer>().sharedMaterial = material;
        }
    }

    [ContextMenu("Clear Level 0")]
    public void ClearLevel()
    {
        Transform existing = transform.Find("Generated_Level");

        if (existing == null)
            return;

        for (int i = existing.childCount - 1; i >= 0; i--)
        {
            GameObject child = existing.GetChild(i).gameObject;

            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }

        DestroyImmediate(existing.gameObject);
}
}
