using System;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField]
    private float _detectionRange;

    [SerializeField]
    private float _visionAngle;

    private Light _light;

    public event Action OnPlayerDetected;
    public event Action OnPlayerLost;
    public event Action<Vector3> OnPlayerLostPos;

    private bool _isPlayerDetected;
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

        if (_isPlayerDetected && _playerDetected != null)
        {
            _enemyScript.ChasePlayer(_playerDetected.position);
            _playerLastPos = _playerDetected.position;
        }
    }

    private void CheckRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _detectionRange);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i] != null && hitColliders[i].CompareTag("Player"))
            {
                CheckFOV(hitColliders[i].transform);
                return;
            }
            // else
            // {
            //     if (_isPlayerDetected)
            //     {
            //         _isPlayerDetected = false;
            //         OnPlayerLost?.Invoke();
            //     }
        }
        
        if (_isPlayerDetected)
        {
            _isPlayerDetected = false;
            _playerDetected = null;

            OnPlayerLost?.Invoke();
            OnPlayerLostPos?.Invoke(_playerLastPos);
        }
    }

    private void CheckFOV(Transform player)
    {
        // Direction du joueur vers l'ennemi
        Vector3 direction = (player.position - transform.position).normalized;

        // Produit scalaire entre la direction de l'ennemi et la direction du joueur
        float dotProduct = Vector3.Dot(transform.forward, direction);

        // Seuil bas� sur le champ de vision
        float angleThreshold = Mathf.Cos(_visionAngle * 0.5f * Mathf.Deg2Rad);

        if (dotProduct >= angleThreshold)
        {
            CheckWalls(player);
        }
        else
        {
            if (_isPlayerDetected)
            {
                _isPlayerDetected = false;
                _playerDetected = null;

                OnPlayerLost?.Invoke();
                OnPlayerLostPos?.Invoke(_playerLastPos);
            }

            _light.color = Color.green;
        }
    }

    private void CheckWalls(Transform player)
    {
        RaycastHit hit;
        Vector3 direction = (player.position - transform.position).normalized;
        float distance = (transform.position - player.transform.position).magnitude;
        int wallLayerMask = LayerMask.GetMask("Wall");

        if (!Physics.Raycast(transform.position, direction, out hit, distance, wallLayerMask))
        {
            if (!_isPlayerDetected)
            {
                _isPlayerDetected = true;
                _playerDetected = player;
                OnPlayerDetected?.Invoke();
            }

            _light.color = Color.red;
        }
        else
        {
            if (_isPlayerDetected)
            {
                _isPlayerDetected = false;
                _playerDetected = null;

                OnPlayerLost?.Invoke();
                OnPlayerLostPos?.Invoke(_playerLastPos);
            }

            _light.color = Color.green;
        }
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

        // Not in vision segment
        Gizmos.color = Color.red;

        angleStep = (360 - _visionAngle) / segments;

        firstPoint = RightPoint;
        previousPoint = firstPoint;

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i;
            Vector3 nextPoint = transform.position + Quaternion.AngleAxis(angle, transform.up) * (RightPoint - transform.position).normalized * _detectionRange;
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

        //// Not in vision segment
        Gizmos.color = Color.red;

        angleStep = (360 - _visionAngle) / segments;

        firstPoint = TopPoint;
        previousPoint = firstPoint;

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i;
            Vector3 nextPoint = transform.position + Quaternion.AngleAxis(angle, transform.right) * (TopPoint - transform.position).normalized * _detectionRange;
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
#endif
}

