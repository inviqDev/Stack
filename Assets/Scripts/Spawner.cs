using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Material platformMaterial;

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
        GenerateNewPlatform("Tower", new Vector3(0, 0, 0), 5.0f, 1.0f, 5.0f);
    }

    public void GenerateNewPlatform(string platformName, Vector3 deltaCenter, float width, float height, float depth)
    {
        var newPlatform = new GameObject(platformName);
        var meshFilter = newPlatform.AddComponent<MeshFilter>();
        var meshRenderer = newPlatform.AddComponent<MeshRenderer>();

        var mesh = new Mesh();

        // Рассчитываем половины размеров
        var halfWidth = width / 2f;
        var halfHeight = height / 2f;
        var halfDepth = depth / 2f;

        // 8 уникальных вершин (как в классическом кубе)
        Vector3[] points =
        {
            // Нижняя грань
            new(-halfWidth, -halfHeight, -halfDepth), // 0
            new(-halfWidth, -halfHeight, halfDepth), // 1
            new(halfWidth, -halfHeight, halfDepth), // 2
            new(halfWidth, -halfHeight, -halfDepth), // 3
            // Верхняя грань
            new(-halfWidth, halfHeight, -halfDepth), // 4
            new(-halfWidth, halfHeight, halfDepth), // 5c
            new(halfWidth, halfHeight, halfDepth), // 6
            new(halfWidth, halfHeight, -halfDepth), // 7
        };

        // 24 вершины (по 4 на каждую грань, для корректных UV)
        Vector3[] vertices =
        {
            // Задняя грань
            points[0], points[3], points[7], points[4],
            // Передняя грань
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
        
        // Треугольники (2 на грань, 6 граней, по 4 вершины на грань)
        int[] triangles =
        {
            // Задняя грань (0,1,2,3)
            0, 2, 1, 0, 3, 2,
            // Передняя грань (4,5,6,7)
            4, 6, 5, 4, 7, 6,
            // Левая грань (8,9,10,11)
            8, 10, 9, 8, 11, 10,
            // Правая грань (12,13,14,15)
            12, 14, 13, 12, 15, 14,
            // Верхняя грань (16,17,18,19)
            16, 18, 17, 16, 19, 18,
            // Нижняя грань (20,21,22,23)
            20, 22, 21, 20, 23, 22
        };

        // UV-развёртка: на каждую грань 4 uv
        Vector2[] uvs =
        {
            // Задняя
            new(0, 0), new(1, 0), new(1, 1), new(0, 1),
            // Передняя
            new(0, 0), new(1, 0), new(1, 1), new(0, 1),
            // Левая
            new(0, 0), new(1, 0), new(1, 1), new(0, 1),
            // Правая
            new(0, 0), new(1, 0), new(1, 1), new(0, 1),
            // Верхняя
            new(0, 0), new(1, 0), new(1, 1), new(0, 1),
            // Нижняя
            new(0, 0), new(1, 0), new(1, 1), new(0, 1)
        };

        

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshRenderer.material = platformMaterial;

        // Ставим позицию объекта на центр основания
        newPlatform.transform.position = deltaCenter;
    }
}