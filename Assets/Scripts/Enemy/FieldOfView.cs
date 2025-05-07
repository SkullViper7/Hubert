using UnityEngine;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Material material;

    private Mesh mesh;
    private float fov = 90f;
    private float viewDistance = 5f;
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

        Vector3 origin = parent.position;
        float startingAngle = parent.eulerAngles.y;
        GenerateFieldOfView(origin, startingAngle);
    }

    private void GenerateFieldOfView(Vector3 origin, float startingAngle)
    {
        float angle = startingAngle - fov / 2f;
        float angleIncrease = fov / rayCount;

        List<Vector3> vertices = new List<Vector3> { Vector3.zero };  // The origin is at (0,0,0) in local space
        List<int> triangles = new List<int>();

        for (int i = 0; i <= rayCount; i++)
        {
            // Cast the ray in the correct direction using Quaternion.Euler
            Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            // Raycast and calculate the distance to the hit point
            Vector3 hitPoint = CastRay(origin, rayDirection);

            // Convert the hit point to local space
            Vector3 localHitPoint = transform.InverseTransformPoint(hitPoint);

            vertices.Add(localHitPoint);  // Store the hit point in local space

            if (i > 0)
            {
                // Define triangles for the mesh
                triangles.Add(0);
                triangles.Add(vertices.Count - 2);
                triangles.Add(vertices.Count - 1);
            }

            angle += angleIncrease;
        }

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
