using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    /// <summary>
    /// A list which stocks neighbors of the waypoint.
    /// </summary>
    [field: SerializeField]
    public List<Waypoint> Neighbors { get; private set; }

    /// <summary>
    /// A value indicating if this waypoint is a waypoint where an enemy can look around him.
    /// </summary>
    [field: SerializeField]
    public bool IsLookAroundWaypoint { get; private set; } = false;

    /// <summary>
    /// A value indicating if the gizmos are visibles or not.
    /// </summary>
    [Space, SerializeField]
    private bool _showGizmos = true;

    /// <summary>
    /// Color of the gizmos.
    /// </summary>
    [SerializeField]
    private Color _gizmoColor = Color.green;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_showGizmos)
        {
            Gizmos.color = _gizmoColor;
            Gizmos.DrawSphere(transform.position, 0.25f);

            if (IsLookAroundWaypoint)
            {
                Gizmos.DrawIcon(transform.position, "EyeIcon.png", false, Color.black);
            }
        }
    }
#endif
}
