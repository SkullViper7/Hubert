using System.Collections.Generic;
using UnityEngine;

public class VisionControlPoints : MonoBehaviour
{
    /// <summary>
    /// All points which an enemy must control to see the player.
    /// </summary>
    [field: SerializeField]
    public List<Transform> ControlPoints { get; private set; } = new();

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < ControlPoints.Count; i++)
        {
            Gizmos.DrawSphere(ControlPoints[i].position, 0.025f);
        }
    }
#endif
}
