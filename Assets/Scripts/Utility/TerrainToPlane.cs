using UnityEngine;
using System.Collections;

/// <summary>
/// Copy terrain to a plane.
/// </summary>
public class TerrainToPlane : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public Material Material;

    /// <summary>
    /// 
    /// </summary>
    public float YOffSet = 2.5f;

    /// <summary>
    /// 
    /// </summary>
    private Terrain Terrain;

    /// <summary>
    /// How many tiles we have on the X axis.
    /// </summary>
    private int mapWidth = -1;

    /// <summary>
    /// How many tiles we have on the Z axis.
    /// </summary>
    private int mapHeight = -1;

    /// <summary>
    /// Size of each square of the mesh.
    /// </summary>
    private float cellSize = 0.5f;

    /// <summary>
    /// 
    /// </summary>
    public bool ShouldRaycast;

    /// <summary>
    /// 
    /// </summary>
    public float RaycastHeight = 64f;

    /// <summary>
    /// 
    /// </summary>
    public float RaycastDistance = 128f;

    /// <summary>
    /// 
    /// </summary>
    public LayerMask RaycastLayers = -1;

    Vector3 DownDirection = Vector3.down;

    void Start()
    {
        Terrain = gameObject.GetComponent<Terrain>();
        BuildMesh();
    }

    /// <summary>
    /// Build terrain mesh
    /// </summary>
    void BuildMesh()
    {
        // Generate mesh data.
        Vector3[] vertices;
        int[] triangles;
        Vector2[] uv;

        //BuildSquare(out vertices, out triangles, out normals, out uv);
        BuildCustomMesh(out vertices, out triangles, out uv);

        // Create new mesh and populate the data.
        // Vector3 Vertices are coordinates.
        // Triangles are made of vertices.
        // Normals are reflection diretion.
        // UV is the mapping, from Texture Coordinate to our Mesh Filter
        var mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        // Create the plane
        var plane = new GameObject("Terrain - Plane");
        plane.transform.parent = gameObject.transform;

        // Add meshFilter and Renderer
        var meshFilter = plane.AddComponent<MeshFilter>();
        var meshRenderer = plane.AddComponent<MeshRenderer>();

        // Set the mesh
        meshFilter.mesh = mesh;
        meshRenderer.material = Material;

        // Translate a little bit up
        plane.transform.Translate(0, YOffSet, 0);
    }

    private void BuildCustomMesh(out Vector3[] vertices, out int[] triangles, out Vector2[] uv)
    {
        // Get size of the terrain.
        mapWidth = (int)(Terrain.terrainData.size.x / cellSize);
        mapHeight = (int)(Terrain.terrainData.size.z / cellSize);

        // Number of squares/tiles in mesh.
        int numberSquares = mapWidth * mapHeight;

        // We need 2 triangles for each square.
        int numberTriangles = numberSquares * 2;

        // Horizontal vertices.
        int horizontalVertices = mapWidth + 1;

        // Vertical vertices.
        int verticalVertices = mapHeight + 1;

        // Number of vertices needed.
        int numberVertices = horizontalVertices * verticalVertices;

        // Set up vertices.
        vertices = new Vector3[numberVertices];

        // As we need one normal/uv for each vertice:
        //normals = new Vector3[numberVertices];
        uv = new Vector2[numberVertices];

        // And triangles.
        // REMEMBER, we need THREE points for EACH triangle.
        triangles = new int[numberTriangles * 3];

        var planePosition = transform.position;

        // Loop through each vertices first
        // Left to Right, Top to Bottom
        for (int y = 0; y < verticalVertices; y++)
        {
            for (int x = 0; x < horizontalVertices; x++)
            {
                // Calculate the vertice index.
                int verticeIndex = (y * horizontalVertices) + x;

                // Define this vertice position
                var position = new Vector3(x * cellSize, 0, y * cellSize) + planePosition;

                if (ShouldRaycast)
                {
                    position.y += RaycastHeight;

                    RaycastHit hit;

                    if (Physics.Raycast(position, DownDirection, out hit, RaycastDistance, RaycastLayers))
                    {
                        position.y = hit.point.y;
                    }
                    else
                    {
                        position.y = Terrain.SampleHeight(position);
                    }
                }
                else
                {
                    position.y = Terrain.SampleHeight(position);
                }

                vertices[verticeIndex] = position;

                // Face the normal up.
                //normals[verticeIndex] = Vector3.up;

                // For UV we kinda need a "percentage" value
                // If x its 0, we use 0
                // If x it's half the horizontalVertices, we need 0.5
                // If x it's equal to horizontalVertices, we need 1.
                float xUV = (float)x / horizontalVertices;
                float yUV = (float)y / verticalVertices;

                // Good to go
                uv[verticeIndex] = new Vector2(xUV, yUV);
            }
        }

        // Loop though squares now
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // Tile index.
                int tileIndex = (y * mapWidth) + x;

                // Triangle index
                int triangleIndex = tileIndex * 6;

                // We need to ordenate the triangle's point anticlockwise, so it points towards the camera.
                // Just remember to use right hand rule, google it if neede, it's pretty easy.

                // First triangle
                triangles[triangleIndex] = (y * horizontalVertices) + x;                      // 0
                triangles[triangleIndex + 1] = triangles[triangleIndex] + horizontalVertices; // 2
                triangles[triangleIndex + 2] = triangles[triangleIndex + 1] + 1;              // 3

                // Second triangle
                triangles[triangleIndex + 3] = triangles[triangleIndex];     // 0
                triangles[triangleIndex + 4] = triangles[triangleIndex + 2]; // 3
                triangles[triangleIndex + 5] = triangles[triangleIndex] + 1; // 1
            }
        }
    }
}
