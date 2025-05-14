using System.Collections.Generic;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    List<Rigidbody> _members;

    private void Start()
    {
        _members = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
    }

    public void EnableRagdoll(bool enabled)
    {
        foreach (Rigidbody member in _members)
        {
            member.isKinematic = !enabled;
        }
    }
}
