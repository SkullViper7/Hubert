using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    /// <summary>
    /// Range around the enemy to detect player.
    /// </summary>
    [SerializeField]
    private float _detectionRange;

    /// <summary>
    /// FOV where the player is visible for the enemy.
    /// </summary>
    [SerializeField]
    private float _visionAngle;

    /// <summary>
    /// Light of the enemy.
    /// </summary>
    private Light _light;

    public event Action OnPlayerDetected;
    public event Action OnPlayerLost;
    public event Action<Vector3> OnPlayerLostPos;

    private bool _isPlayerAlreadyDetected;
    private Transform _playerDetected;

    [SerializeField] Enemy _enemyScript;

    private Vector3 _playerLastPos;

    private void Awake()
    {
        _light = GetComponent<Light>();
    }

    private void FixedUpdate()
    {
        _light.range = _detectionRange;
        _light.innerSpotAngle = _visionAngle;
        _light.spotAngle = _visionAngle;
        CheckRange();

        if (_isPlayerAlreadyDetected && _playerDetected != null)
        {
            _enemyScript.ChasePlayer(_playerDetected.position);
            _playerLastPos = _playerDetected.position;
        }
    }

    /// <summary>
    /// Called to check if there is the player in the range around the enemy
    /// </summary>
    private void CheckRange()
    {
        bool playerIsVisible = false;

        // Get colliders around the enemy
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _detectionRange);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            // Check if it's the player
            if (hitColliders[i] != null && hitColliders[i].CompareTag("Player"))
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
                _light.color = Color.red;
            }
        }
        else
        {
            if (_isPlayerAlreadyDetected)
            {
                _isPlayerAlreadyDetected = false;
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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        int segments = 30;

        // Draw range
        Gizmos.color = Color.yellow;

        Vector3 LeftPoint = transform.position + Quaternion.AngleAxis(-_visionAngle / 2, transform.up) * transform.forward * _detectionRange;
        Vector3 RightPoint = transform.position + Quaternion.AngleAxis(_visionAngle / 2, transform.up) * transform.forward * _detectionRange;
        Vector3 BottomPoint = transform.position + Quaternion.AngleAxis(-_visionAngle / 2, transform.right) * transform.forward * _detectionRange;
        Vector3 TopPoint = transform.position + Quaternion.AngleAxis(_visionAngle / 2, transform.right) * transform.forward * _detectionRange;

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
            Vector3 nextPoint = transform.position + Quaternion.AngleAxis(angle, transform.up) * (LeftPoint - transform.position).normalized * _detectionRange;
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
            Vector3 nextPoint = transform.position + Quaternion.AngleAxis(angle, transform.right) * (BottomPoint - transform.position).normalized * _detectionRange;
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
#endif
}

