using UnityEngine;
using System.Collections;

/// <summary>
/// Copy terrain to a plane.
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class MeshBuilder : MonoBehaviour
{
    /// <summary>
    /// Used by the BuildSquareMesh.
    /// </summary>
    public int VerticeScale = 100;

    /// <summary>
    /// How many tiles we have on the X axis.
    /// </summary>
    public int mapWidth = 6;

    /// <summary>
    /// How many tiles we have on the Z axis.
    /// </summary>
    public int mapHeight = 1;

    /// <summary>
    /// Size of each square of the mesh.
    /// </summary>
    public float cellSize = 1.0f;

    void Start()
    {
        BuildMesh();
    }

    /// <summary>
    /// Build terrain mesh
    /// </summary>
    void BuildMesh()
    {
        // Generate mesh data.
        Vector3[] vertices;
        Vector3[] normals;
        int[] triangles;
        Vector2[] uv;

        //BuildSquare(out vertices, out triangles, out normals, out uv);
        BuildCustomMesh(out vertices, out triangles, out normals, out uv);

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

        // Asign the mesh to the gameObject.
        var meshFilter = GetComponent<MeshFilter>();
        var meshRenderer = GetComponent<MeshRenderer>();
        var meshCollider = GetComponent<MeshCollider>();

        meshFilter.mesh = mesh;
    }

    private void BuildCustomMesh(out Vector3[] vertices, out int[] triangles, out Vector3[] normals, out Vector2[] uv)
    {
        // Number of squares/tiles in mesh
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
        normals = new Vector3[numberVertices];
        uv = new Vector2[numberVertices];

        // And triangles.
        // REMEMBER, we need THREE points for EACH triangle.
        triangles = new int[numberTriangles * 3];

        // Loop through each vertices first
        // Left to Right, Top to Bottom
        for (int y = 0; y < verticalVertices; y++)
        {
            for (int x = 0; x < horizontalVertices; x++)
            {
                // Calculate the vertice index.
                int verticeIndex = (y * horizontalVertices) + x;

                // Define this vertice position
                var position = new Vector3(x * cellSize, 0, y * cellSize);
                vertices[verticeIndex] = position;

                // Face the normal up.
                normals[verticeIndex] = Vector3.up;

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

    /// <summary>
    /// Build a simple square.
    /// </summary>
    /// <param name="vertices">Vertices to be used.</param>
    /// <param name="triangles">Triangles array store the "index" of the vertice to be used.</param>
    void BuildSquare(out Vector3[] vertices, out int[] triangles, out Vector3[] normals, out Vector2[] uv)
    {
        // A square, has only 4 vertices.
        vertices = new Vector3[4];

        // Set up vertices
        // To have a big and plane (no height) map, we just adjust the scale.
        vertices[0] = new Vector3(0, 0, 0); // A
        vertices[1] = new Vector3(VerticeScale, 0, 0); // B
        vertices[2] = new Vector3(0, 0, -VerticeScale); // C
        vertices[3] = new Vector3(VerticeScale, 0, -VerticeScale); // D

        // We have one normal for each vertice.
        normals = new Vector3[4];

        // Set up normals
        normals[0] = Vector3.up;
        normals[1] = Vector3.up;
        normals[2] = Vector3.up;
        normals[3] = Vector3.up;

        // We can calculate it in many ways.
        // 1) We want 1 square, so we have 6 points: 1 * 6
        // 2) We want 1 square, made of 2 triangles, each made of 3 points: (1 * 2) * 3
        // Either way, it works properly.
        triangles = new int[2 * 3];

        // Set up first triangle
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        // Set up second triangle
        triangles[3] = 2;
        triangles[4] = 1;
        triangles[5] = 3;

        // Just like normals, we need 1 UV for each vertice
        // UV goes from 0 (LEFT-BOTTOM most) to 1 (RIGHT-TOP most).
        // Left-bottom to right-top, because unity uses inversed texture coordinate.
        uv = new Vector2[4];

        // Unity uses "inversed" uv mapping, so the (0,0) coordinate, it's the 
        uv[0] = new Vector2(0, 1); // Vertice A - Top-left
        uv[1] = new Vector2(1, 1); // Vertice B - Top-Right
        uv[2] = new Vector2(0, 0); // Vertice C - Bottom-Left
        uv[3] = new Vector2(1, 0); // Vertice D - Bottom-Right
    }
}
