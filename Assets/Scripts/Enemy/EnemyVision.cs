using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField, Header("General")]
    private VisionType _visionType;

    /// <summary>
    /// Range around the enemy to detect player.
    /// </summary>
    [SerializeField]
    private float detectionRange = 5f;

    /// <summary>
    /// Public reference to get or set the detection range.
    /// </summary>
    public float DetectionRange
    {
        get => detectionRange;
        set => _targetRange = value;
    }

    /// <summary>
    /// Targeted range.
    /// </summary>
    private float _targetRange;

    /// <summary>
    /// Smoothness of the transition to the target.
    /// </summary>
    [SerializeField]
    private float _rangeSmoothness;

    /// <summary>
    /// FOV where the player is visible for the enemy.
    /// </summary>
    [SerializeField]
    private float _visionAngle;

    /// <summary>
    /// Layer mask which occludes the vision.
    /// </summary>
    [SerializeField]
    private LayerMask _layerMask;

    /// <summary>
    /// A value indicating if the gizmos are visibles or not.
    /// </summary>
    [SerializeField]
    private bool _showGizmos = true;

    /// <summary>
    /// Events to indicate when the player is seen and the context.
    /// </summary>
    public event Action<Vector3, PlayerSeenContext> OnPlayerSeen;

    /// <summary>
    /// Last position seen of the player.
    /// </summary>
    private Vector3 _playerLastPos;

    /// <summary>
    /// A value indicating if the player is already detected.
    /// </summary>
    private bool _isPlayerAlreadyDetected;

    /// <summary>
    /// Light of the enemy.
    /// </summary>
    private Light _light;

    /// <summary>
    /// Precision of the FOV for the minimap.
    /// </summary>
    [SerializeField, Space, Header("Minimap")]
    private int _fovDetails;

    /// <summary>
    /// Material of the FOV for the minimap.
    /// </summary>
    [SerializeField]
    private Material _fovMaterial;

    /// <summary>
    /// Object which represent the FOV on the minimap.
    /// </summary>
    private GameObject _fovObject;

    /// <summary>
    /// Mesh of the FOV for the minimap.
    /// </summary>
    private Mesh _fovMesh;

    private void Awake()
    {
        _light = GetComponent<Light>();
    }

    private void Start()
    {
        _targetRange = detectionRange;

        // Create the mesh which represent the mesh for the minimap
        _fovMesh = new();
        { _fovMesh.name = "FOVMesh"; }
        _fovObject = new();
        { _fovObject.name = "FOVObject"; _fovObject.layer = LayerMask.NameToLayer("Minimap"); }
        _fovObject.transform.SetParent(transform, false);

        MeshFilter meshFilter = _fovObject.AddComponent<MeshFilter>();
        meshFilter.mesh = _fovMesh;
        MeshRenderer meshRenderer = _fovObject.AddComponent<MeshRenderer>();
        meshRenderer.material = _fovMaterial;
    }

    private void Update()
    {
        detectionRange = Mathf.MoveTowards(detectionRange, _targetRange, Time.deltaTime * _rangeSmoothness);
        _light.range = detectionRange;

        Vector3 origin = transform.position;
        float startingAngle = transform.eulerAngles.y;
        DrawFOV(origin, startingAngle);

        CheckRange();
    }

    /// <summary>
    /// Called to check if there is the player in the range around the enemy.
    /// </summary>
    private void CheckRange()
    {
        bool playerIsVisible = false;

        // Get colliders around the enemy
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange, LayerMask.GetMask("Player"));

        for (int i = 0; i < hitColliders.Length; i++)
        {
            // Check if it's the player
            if (hitColliders[i] != null && hitColliders[i].gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                // Try get control points
                if (hitColliders[i].TryGetComponent<VisionControlPoints>(out VisionControlPoints visionControlPoints))
                {
                    List<Transform> points = visionControlPoints.ControlPoints;

                    // Check if each control point is visible
                    for (int j = 0; j < points.Count; j++)
                    {
                        if (IsInFOV(points[j]) && ThereIsNoWallsBetween(points[j]))
                        {
                            _playerLastPos = hitColliders[i].transform.position;
                            playerIsVisible = true;
                            break;
                        }
                    }
                }
            }
        }

        if (playerIsVisible)
        {
            if (!_isPlayerAlreadyDetected)
            {
                _isPlayerAlreadyDetected = true;
                OnPlayerSeen?.Invoke(_playerLastPos, PlayerSeenContext.FirstTime);
                _light.color = Color.red;
            }
            else
            {
                OnPlayerSeen?.Invoke(_playerLastPos, PlayerSeenContext.Continue);
            }
        }
        else
        {
            if (_isPlayerAlreadyDetected)
            {
                _isPlayerAlreadyDetected = false;
                OnPlayerSeen?.Invoke(_playerLastPos, PlayerSeenContext.LastTime);
                _light.color = Color.green;
            }
        }
    }

    /// <summary>
    /// Called to check if a part of the player is in the FOV of the enemy.
    /// </summary>
    /// <param name="controlPoint"> Transform of the point to control. </param>
    /// <returns> If the point is in FOV. </returns>
    private bool IsInFOV(Transform controlPoint)
    {
        // Direction of the control point towards the enemy
        Vector3 direction = (controlPoint.position - transform.position).normalized;

        // Dot product between enemy direction and point direction
        float dotProduct = Vector3.Dot(transform.forward, direction);

        // Threshold based on field of view
        float angleThreshold = Mathf.Cos(_visionAngle * 0.5f * Mathf.Deg2Rad);

        return (dotProduct >= angleThreshold);
    }

    /// <summary>
    /// Called to check if there is walls between the enemy and a control point.
    /// </summary>
    /// <param name="controlPoint"> Transform of the point to control. </param>
    /// <returns> If there is a wall between. </returns>
    private bool ThereIsNoWallsBetween(Transform controlPoint)
    {
        Vector3 direction = (controlPoint.position - transform.position).normalized;
        float distance = (transform.position - controlPoint.transform.position).magnitude;
        int wallLayerMask = LayerMask.GetMask("Wall");

        return !Physics.Raycast(transform.position, direction, distance, wallLayerMask);
    }

    /// <summary>
    /// Called to draw the fov with a mesh.
    /// </summary>
    /// <param name="origin"> Origin of the vision. </param>
    /// <param name="startingAngle"> Direction of the vision. </param>
    private void DrawFOV(Vector3 origin, float startingAngle)
    {
        float angle = startingAngle - _visionAngle / 2f;
        float angleIncrease = _visionAngle / _fovDetails;

        List<Vector3> vertices = new() { Vector3.zero };
        List<int> triangles = new();

        for (int i = 0; i <= _fovDetails; i++)
        {
            // Cast the ray in the correct direction using Quaternion.Euler
            Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            // Raycast and calculate the distance to the hit point
            Vector3 hitPoint = CastRay(origin, rayDirection);

            // Convert the hit point to local space
            Vector3 localHitPoint = transform.InverseTransformPoint(hitPoint);

            vertices.Add(localHitPoint);

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
        _fovMesh.Clear();
        _fovMesh.vertices = vertices.ToArray();
        _fovMesh.triangles = triangles.ToArray();
        _fovMesh.RecalculateNormals();

        // Ensure the mesh is positioned correctly
        _fovObject.transform.SetPositionAndRotation(origin, transform.rotation);
    }

    /// <summary>
    /// Call to cast a ray and retrun the point where it ended.
    /// </summary>
    /// <param name="origin"> Origin of the cast. </param>
    /// <param name="direction"> Direction of the cast. </param>
    /// <returns></returns>
    private Vector3 CastRay(Vector3 origin, Vector3 direction)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, detectionRange, _layerMask))
        {
            return hit.point;
        }
        else
        {
            return origin + direction * detectionRange;
        }
    }

    private void OnDisable()
    {
        Destroy(_fovObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_showGizmos)
        {
            int segments = 30;

            // Draw range
            Gizmos.color = Color.yellow;

            Vector3 LeftPoint = transform.position + Quaternion.AngleAxis(-_visionAngle / 2, transform.up) * transform.forward * detectionRange;
            Vector3 RightPoint = transform.position + Quaternion.AngleAxis(_visionAngle / 2, transform.up) * transform.forward * detectionRange;
            Vector3 BottomPoint = transform.position + Quaternion.AngleAxis(-_visionAngle / 2, transform.right) * transform.forward * detectionRange;
            Vector3 TopPoint = transform.position + Quaternion.AngleAxis(_visionAngle / 2, transform.right) * transform.forward * detectionRange;

            Gizmos.DrawLine(transform.position, LeftPoint);
            Gizmos.DrawLine(transform.position, RightPoint);
            Gizmos.DrawLine(transform.position, BottomPoint);
            Gizmos.DrawLine(transform.position, TopPoint);

            // Draw circle
            Gizmos.color = Color.cyan;

            Vector3 center = (LeftPoint + RightPoint + BottomPoint + TopPoint) / 4;
            float radius = Vector3.Distance(LeftPoint, RightPoint) / 2;
            float angleStep = 360f / segments;

            Vector3 firstPoint = center + (transform.right * radius);
            Vector3 previousPoint = firstPoint;

            for (int i = 1; i <= segments; i++)
            {
                float angle = angleStep * i;
                Vector3 nextPoint = center + Quaternion.AngleAxis(angle, transform.forward) * (transform.right * radius);
                Gizmos.DrawLine(previousPoint, nextPoint);
                previousPoint = nextPoint;
            }
            Gizmos.DrawLine(previousPoint, firstPoint);

            // Draw horizontal circle of the sphere

            // Vision segment
            Gizmos.color = Color.magenta;

            angleStep = _visionAngle / segments;

            firstPoint = LeftPoint;
            previousPoint = firstPoint;

            for (int i = 1; i <= segments; i++)
            {
                float angle = angleStep * i;
                Vector3 nextPoint = transform.position + Quaternion.AngleAxis(angle, transform.up) * (LeftPoint - transform.position).normalized * detectionRange;
                Gizmos.DrawLine(previousPoint, nextPoint);
                previousPoint = nextPoint;
            }

            //// Draw vertical circle of the sphere

            //// Vision segment
            Gizmos.color = Color.magenta;

            angleStep = _visionAngle / segments;

            firstPoint = BottomPoint;
            previousPoint = firstPoint;

            for (int i = 1; i <= segments; i++)
            {
                float angle = angleStep * i;
                Vector3 nextPoint = transform.position + Quaternion.AngleAxis(angle, transform.right) * (BottomPoint - transform.position).normalized * detectionRange;
                Gizmos.DrawLine(previousPoint, nextPoint);
                previousPoint = nextPoint;
            }
        }
    }
#endif
}

