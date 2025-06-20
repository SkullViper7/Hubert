using System.Collections.Generic;
using UnityEngine;

public class Laser : DetectionObject
{
    /// <summary>
    /// Dictionary which stocks colliders as an only object.
    /// </summary>
    private Dictionary<GameObject, HashSet<Collider>> _trackedColliders = new();

    private void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (!_trackedColliders.TryGetValue(root, out var colliders))
        {
            colliders = new HashSet<Collider>();
            _trackedColliders[root] = colliders;
            PlayerIsDetected(root.transform.position, PlayerSeenContext.FirstTime);
        }

        // Add the collider individually
        colliders.Add(other);
    }

    private void OnTriggerStay(Collider other)
    {
        GameObject root = other.transform.root.gameObject;
        if (_trackedColliders.ContainsKey(root))
        {
            PlayerIsDetected(root.transform.position, PlayerSeenContext.Continue);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (_trackedColliders.TryGetValue(root, out var colliders))
        {
            colliders.Remove(other);

            if (colliders.Count == 0)
            {
                _trackedColliders.Remove(root);
                PlayerIsDetected(root.transform.position, PlayerSeenContext.LastTime);
            }
        }
    }
}
