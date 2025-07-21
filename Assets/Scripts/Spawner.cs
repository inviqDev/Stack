using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public event Action<float> OnSpawnerMovedUp;
    
    [SerializeField] private Material platformMaterial;

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform targetPoint;

    public Transform CurrentTowerTopObj { get; private set; }
    public Vector3 CurrentTowerSize { get; private set; }
    public Transform CurrentPlatformObj { get; private set; }
    public Vector3 CurrentPlatformSize { get; private set; }
    public float MinPlatformSizeX { get; } = 0.2f;

    private readonly Vector3 _startTowerSize = new(5.0f, 20.0f, 5.0f);
    private readonly Vector3 _startPlatformSize = new(5.0f, 1.0f, 5.0f);

    public static Spawner Instance { get; private set; }

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (!GameManager.Instance)
        {
            Debug.LogError("GameManager not found");
            return;
        }
        
        SetCurrentTowerSize(_startTowerSize);
        SetCurrentPlatformSize(_startPlatformSize);
        
        CreateBaseTowerObject(_startTowerSize);

        GameManager.Instance.OnTimerCountdownFinished += OnTimerCountdownFinished;
    }

    private void OnEnable()
    {
        if (!GameManager.Instance)
        {
            Debug.LogError("GameManager not found");
            return;
        }
        
        GameManager.Instance.OnTimerCountdownFinished -= OnTimerCountdownFinished;
    }

    private void OnTimerCountdownFinished()
    {
        CreateNewMovingPlatform();
    }

    public void SetCurrentTowerSize(Vector3 newTowerSize)
    {
        CurrentTowerSize = newTowerSize;
    }
    
    public void SetCurrentPlatformSize(Vector3 newPlatformSize)
    {
        CurrentPlatformSize = newPlatformSize;
    }

    public GameObject SpawnNewPlatform(string platformName, Vector3 spawnPosition, Vector3 platformSize)
    {
        var newPlatform = new GameObject(platformName);
        var mesh = new Mesh();

        // Рассчитываем половины размеров
        var halfWidth = platformSize.x * 0.5f;
        var halfHeight = platformSize.y * 0.5f;
        var halfDepth = platformSize.z * 0.5f;

        // 8 уникальных вершин (как в классическом кубе)
        var points = CreatePointsArray(halfWidth, halfHeight, halfDepth);

        // 24 вершины (по 4 на каждую грань, для корректных UV)
        var vertices = CreateVerticesArray(points);
        mesh.vertices = vertices;
        
        // Треугольники (2 на грань, 6 граней, по 4 вершины на грань)
        var triangles = CreateTrianglesArray();
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        
        var meshFilter = newPlatform.AddComponent<MeshFilter>();
        var meshRenderer = newPlatform.AddComponent<MeshRenderer>();
        var boxCollider = newPlatform.AddComponent<BoxCollider>();

        meshFilter.mesh = mesh;
        meshRenderer.material = platformMaterial;

        // Ставим позицию объекта в указанную точку
        newPlatform.transform.position = spawnPosition;
        return newPlatform;
    }
    private static int[] CreateTrianglesArray()
    {
        int[] triangles =
        {
            // Задняя грань (0,1,2,3) // front?
            0, 2, 1, 0, 3, 2,
            // Передняя грань (4,5,6,7)
            4, 5, 6, 4, 6, 7,
            // Левая грань (8,9,10,11)
            8, 10, 9, 8, 11, 10,
            // Правая грань (12,13,14,15)
            12, 14, 13, 12, 15, 14,
            // Верхняя грань (16,17,18,19)
            16, 18, 17, 16, 19, 18,
            // Нижняя грань (20,21,22,23)
            20, 22, 21, 20, 23, 22
        };
        return triangles;
    }
    private static Vector3[] CreateVerticesArray(Vector3[] points)
    {
        Vector3[] vertices =
        {
            // Передняя грань
            points[0], points[3], points[7], points[4],
            // Задняя грань
            points[1], points[2], points[6], points[5],
            // Левая грань
            points[0], points[4], points[5], points[1],
            // Правая грань
            points[3], points[2], points[6], points[7],
            // Верхняя грань
            points[4], points[7], points[6], points[5],
            // Нижняя грань
            points[0], points[1], points[2], points[3],
        };
        return vertices;
    }
    private static Vector3[] CreatePointsArray(float halfWidth, float halfHeight, float halfDepth)
    {
        Vector3[] points =
        {
            // Нижняя грань
            new(-halfWidth, -halfHeight, -halfDepth), // 0
            new(-halfWidth, -halfHeight, halfDepth), // 1
            new(halfWidth, -halfHeight, halfDepth), // 2
            new(halfWidth, -halfHeight, -halfDepth), // 3
            // Верхняя грань
            new(-halfWidth, halfHeight, -halfDepth), // 4
            new(-halfWidth, halfHeight, halfDepth), // 5
            new(halfWidth, halfHeight, halfDepth), // 6
            new(halfWidth, halfHeight, -halfDepth), // 7
        };
        return points;
    }
    

    private void CreateBaseTowerObject(Vector3 size, string towerTopObjName = "Tower_Top_Obj")
    {
        var towerBase = SpawnNewPlatform(towerTopObjName, Vector3.zero, size).transform;
        SetNewTowerTopObjTransform(towerBase);

        // move spawner points from current Y position Y" to (half tower height) + (half platform height)
        var newSpawnerPositionY = transform.position.y + size.y * 0.5f + _startPlatformSize.y * 0.5f;
        transform.position = new Vector3(transform.position.x, newSpawnerPositionY, transform.position.z);
    }
    
    public void CreateNewMovingPlatform()
    {   
        var randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        CurrentPlatformObj = SpawnNewPlatform("Moving_Platform", randomSpawnPoint, CurrentPlatformSize).transform;
        CurrentPlatformObj.gameObject.AddComponent<PlatformMover>().SetPlatformMovingSettings(targetPoint.position);

        // next platform will be spawned "_startPlatformSize.y" points upper
        MoveSpawnersUp(_startPlatformSize.y);
    }

    private void MoveSpawnersUp(float offsetY)
    {
        var newSpawnPointsPositionY = transform.position.y + offsetY;
        transform.position = new Vector3(transform.position.x, newSpawnPointsPositionY, transform.position.z);
        
        OnSpawnerMovedUp?.Invoke(offsetY);
    }

    public void SetNewTowerTopObjTransform(Transform newTopTowerObject)
    {
        CurrentTowerTopObj = newTopTowerObject;
    }

    private void OnDisable()
    {
        if (!GameManager.Instance)
        {
            Debug.LogError("GameManager not found");
            return;
        }
        
        GameManager.Instance.OnTimerCountdownFinished -= OnTimerCountdownFinished;
    }
    
    private void OnDestroy()
    {
        if (!GameManager.Instance)
        {
            Debug.LogError("GameManager not found");
            return;
        }
        
        GameManager.Instance.OnTimerCountdownFinished -= OnTimerCountdownFinished;
    }
}