using System;
using UnityEngine;

public class DetectionObject : MonoBehaviour
{
    /// <summary>
    /// An event to indicate that the player is detected.
    /// </summary>
    public event Action<Vector3, PlayerSeenContext> OnPlayerDetected;

    /// <summary>
    /// Called when the player is detected.
    /// </summary>
    protected void PlayerIsDetected(Vector3 position, PlayerSeenContext context)
    {
        OnPlayerDetected?.Invoke(position, context);
    }
}
