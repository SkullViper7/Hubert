using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class RoomDoor : MonoBehaviour
{
    /// <summary>
    /// The entry direction of the door.
    /// </summary>
    [SerializeField] 
    private Vector3 _entryDirection = Vector3.forward;

    /// <summary>
    /// A dictionary to stock all colliders in the door and their last positions.
    /// </summary>
    private Dictionary<Collider, Vector3> _lastPositions = new();

    /// <summary>
    /// Events when an enemy enter or exit the door.
    /// </summary>
    public event Action<EnemyBrain> OnEnemyEnter, OnEnemyExit;

    /// <summary>
    /// The color of the gizmos.
    /// </summary>
    [SerializeField]
    private Color _gizmosColor = Color.green;

    /// <summary>
    /// A value indicating if the gizmos are showed.
    /// </summary>
    [SerializeField]
    private bool _showGizmos = true;

    private void OnTriggerEnter(Collider other)
    {
        // Saves the entry position
        _lastPositions[other] = other.transform.position;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_lastPositions.TryGetValue(other, out Vector3 lastPos))
            return;

        Vector3 movement = (other.transform.position - lastPos).normalized;
        Vector3 localMove = transform.InverseTransformDirection(movement);
        Vector3 localEntry = _entryDirection.normalized;

        float dot = Vector3.Dot(localMove, localEntry);

        if (dot > 0)
        {
            if (other.TryGetComponent(out EnemyBrain enemyBrain))
            {
                OnEnemyEnter?.Invoke(enemyBrain);
            }
        }
        else
        {
            if (other.TryGetComponent(out EnemyBrain enemyBrain))
            {
                OnEnemyExit?.Invoke(enemyBrain);
            }
        }

        _lastPositions.Remove(other);
    }

    private void OnTriggerStay(Collider other)
    {
        // Updates position while in it
        _lastPositions[other] = other.transform.position;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_showGizmos)
        {
            Gizmos.color = _gizmosColor;
            Gizmos.DrawCube(transform.position, GetComponent<BoxCollider>().bounds.size);
            Handles.color = _gizmosColor;
            Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(_entryDirection), 1f, EventType.Repaint);
        }
    }
#endif
}
