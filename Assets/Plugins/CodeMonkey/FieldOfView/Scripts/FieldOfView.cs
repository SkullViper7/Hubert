using UnityEngine;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Material material;
    [SerializeField] private float coneHeight = 5f;  // Height of the cone (extrusion depth)

    private Mesh mesh;
    private float fov = 90f;
    private float viewDistance = 10f;
    private int rayCount = 50;

    private Transform parent;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;

        parent = transform.parent;
    }

    private void LateUpdate()
    {
        if (parent == null) return;

        Vector3 origin = parent.position - Vector3.up * 1.1f;
        float startingAngle = parent.eulerAngles.y;
        GenerateFieldOfView(origin, startingAngle);
    }

    private void GenerateFieldOfView(Vector3 origin, float startingAngle)
    {
        float angle = startingAngle - fov / 2f;
        float angleIncrease = fov / rayCount;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // The base layer vertices (at y = 0)
        List<int> baseVertexIndices = new List<int>();

        // Add the base vertices from raycasts
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            Vector3 hitPoint = CastRay(origin, rayDirection);

            // Keep base vertices at ground level (y = 0)
            Vector3 baseVertex = transform.InverseTransformPoint(hitPoint);
            baseVertex.y = 0; // Flatten the vertex to the ground level

            // Add the base vertex to the list and save its index
            baseVertexIndices.Add(vertices.Count);
            vertices.Add(baseVertex);

            angle += angleIncrease;
        }

        // Add the top layer vertices (at y = coneHeight)
        List<int> topVertexIndices = new List<int>();
        foreach (int baseIndex in baseVertexIndices)
        {
            Vector3 baseVertex = vertices[baseIndex];
            topVertexIndices.Add(vertices.Count);
            vertices.Add(new Vector3(baseVertex.x, coneHeight, baseVertex.z)); // Raise the top layer vertex
        }

        // Add the base center (for closed bottom)
        vertices.Add(Vector3.zero); // The center of the base for the bottom triangles

        // Generate triangles for the cone sides (extruded part)
        for (int i = 0; i < rayCount; i++)
        {
            int currentBase = baseVertexIndices[i];
            int nextBase = baseVertexIndices[(i + 1) % (rayCount + 1)];

            int currentTop = topVertexIndices[i];
            int nextTop = topVertexIndices[(i + 1) % (rayCount + 1)];

            // Side triangles (faces of the cone)
            triangles.Add(currentBase);
            triangles.Add(currentTop);
            triangles.Add(nextTop);

            triangles.Add(currentBase);
            triangles.Add(nextTop);
            triangles.Add(nextBase);
        }

        // Base triangles for the bottom (optional, for a filled base)
        for (int i = 0; i < rayCount - 1; i++)
        {
            triangles.Add(vertices.Count - 1); // Center of the base
            triangles.Add(baseVertexIndices[i]);
            triangles.Add(baseVertexIndices[i + 1]);
        }
        triangles.Add(vertices.Count - 1); // Last triangle
        triangles.Add(baseVertexIndices[rayCount - 1]);
        triangles.Add(baseVertexIndices[0]);

        // Apply the calculated mesh vertices and triangles
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        // Ensure the mesh is positioned correctly
        transform.position = origin;
        transform.rotation = Quaternion.Euler(0, startingAngle, 0);
    }

    private Vector3 CastRay(Vector3 origin, Vector3 direction)
    {
        Debug.DrawRay(origin, direction * viewDistance, Color.red);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, viewDistance, layerMask))
        {
            return hit.point;
        }
        else
        {
            return origin + direction * viewDistance;
        }
    }
}
