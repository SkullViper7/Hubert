using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class RoomDoor : MonoBehaviour
{
    /// <summary>
    /// List which containes all faces associated to a room.
    /// </summary>
    [SerializeField]
    private List<DoorFace> _roomEntries = new();

    /// <summary>
    /// A dictionary to stock all colliders in the door and their last positions.
    /// </summary>
    private readonly Dictionary<Collider, Vector3> _lastPositions = new();

    /// <summary>
    /// The color of the door.
    /// </summary>
    [SerializeField]
    private Color _doorColor = Color.green;

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

        DoorFace bestMatch = default;
        float bestDot = float.NegativeInfinity;
        bool hasMatch = false;

        foreach (var face in _roomEntries)
        {
            float dot = Vector3.Dot(localMove, face.LocalDirection.normalized);
            if (dot > bestDot)
            {
                bestDot = dot;
                bestMatch = face;
                hasMatch = true;
            }
        }

        if (hasMatch && other.TryGetComponent(out EnemyBrain enemyBrain))
        {
            bestMatch.AssociatedRoom.TryAddEnemy(enemyBrain);
            RemoveEnemyFromOtherRooms(enemyBrain, bestMatch.AssociatedRoom);
        }
        else if (hasMatch && other.TryGetComponent(out PlayerStateManager playerStateManager))
        {
            bestMatch.AssociatedRoom.AddPlayer(playerStateManager);
            RemovePlayerFromOtherRooms(bestMatch.AssociatedRoom);
        }

        _lastPositions.Remove(other);
    }

    private void OnTriggerStay(Collider other)
    {
        // Updates position while in it
        _lastPositions[other] = other.transform.position;
    }

    /// <summary>
    /// Called to remove the enemy from other rooms when he enters in a new room.
    /// </summary>
    /// <param name="enemy"> The enemy to remove. </param>
    /// <param name="excludedRoom"> The room where enemy is entering. </param>
    private void RemoveEnemyFromOtherRooms(EnemyBrain enemy, Room excludedRoom)
    {
        for (int i = 0; i < _roomEntries.Count; i++)
        {
            if (_roomEntries[i].AssociatedRoom != excludedRoom)
            {
                _roomEntries[i].AssociatedRoom.TryRemoveEnemy(enemy);
            }
        }
    }

    /// <summary>
    /// Called to remove the player from other rooms when he enters in a new room.
    /// </summary>
    /// <param name="excludedRoom"> The room where player is entering. </param>
    private void RemovePlayerFromOtherRooms(Room excludedRoom)
    {
        for (int i = 0; i < _roomEntries.Count; i++)
        {
            if (_roomEntries[i].AssociatedRoom != excludedRoom)
            {
                _roomEntries[i].AssociatedRoom.RemovePlayer();
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_showGizmos)
        {
            BoxCollider box = GetComponent<BoxCollider>();
            if (!box)
                return;

            Gizmos.color = _doorColor;
            Gizmos.DrawCube(transform.position, box.bounds.size);

            if (_roomEntries == null || _roomEntries.Count == 0)
                return;

            for (int i = 0; i < _roomEntries.Count; ++i)
            {
                DoorFace entry = _roomEntries[i];
                Handles.color = entry.Color;

                // Direction en espace local
                Vector3 localDirection = entry.LocalDirection.normalized;

                // Taille de la box en local
                Vector3 halfSize = box.size * 0.5f;

                // Position locale sur la face correspondante
                Vector3 localOffset = new Vector3(
                    localDirection.x * halfSize.x,
                    0f, // Ignore Y pour une porte verticale
                    localDirection.z * halfSize.z
                );

                // Convertir en espace monde
                Vector3 worldPos = box.transform.TransformPoint(box.center + localOffset);
                Vector3 worldDir = box.transform.TransformDirection(localDirection);

                if (worldDir != Vector3.zero)
                {
                    // Dessiner la flèche
                    Handles.ArrowHandleCap(0, worldPos, Quaternion.LookRotation(worldDir), 1f, EventType.Repaint);
                }
            }
        }
    }
#endif
}
