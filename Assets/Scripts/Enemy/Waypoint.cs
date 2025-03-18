using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField]
    private List<Waypoint> _waypoints = new();

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);

        Gizmos.color = Color.blue;
        for (int i = 0; i < _waypoints.Count; i++)
        {
            Gizmos.DrawLine(transform.position, _waypoints[i].transform.position);
        }
    }
#endif
}
