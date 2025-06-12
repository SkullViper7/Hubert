#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UIElements;

public class Checkpoint : MonoBehaviour
{
    /// <summary>
    /// The order of the checkpoint in the level.
    /// </summary>
    [field: SerializeField, Header("Checkpoint")]
    public int CheckpointOrder { get; private set; }

    /// <summary>
    /// The transform where Hubert will be respawned.
    /// </summary>
    [field: SerializeField]
    public Transform RespawnPosition { get; private set; }

    /// <summary>
    /// A value indicating if the gizmos are visibles or not.
    /// </summary>
    [SerializeField, Space, Header("Gizmos")]
    private bool _showGizmos = true;

    /// <summary>
    /// The color of the box wich represents the checkpoint.
    /// </summary>
    [SerializeField]
    private Color _checkpointColor;

    /// <summary>
    /// The color of the sphere wich represents the spawn.
    /// </summary>
    [SerializeField]
    private Color _spawnColor;

    /// <summary>
    /// The color of the arrow wich represents the forward of the player when he respawns.
    /// </summary>
    [SerializeField]
    private Color _orientationColor;

    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.UpdateCheckpoint(CheckpointOrder);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!_showGizmos) return;

        // Draw the checkpoint volume
        Gizmos.color = _checkpointColor;
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        Gizmos.DrawCube(transform.position, boxCollider.bounds.size);

        // Draw the spawn sphere
        Gizmos.color = _spawnColor;
        Gizmos.DrawSphere(RespawnPosition.position, 0.25f);

        // Draw the direction of the player on spawn
        Handles.color = _orientationColor;
        Handles.ArrowHandleCap(0, RespawnPosition.position, RespawnPosition.rotation, 1f, EventType.Repaint);

        // Draw the order of the checkpoint
        Handles.Label(transform.position, CheckpointOrder.ToString());
    }
#endif
}
