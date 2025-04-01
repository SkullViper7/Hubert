using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private List<Waypoint> _path;

    private NavMeshAgent _navMeshAgent;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Called to kill the enemy.
    /// </summary>
    public void Death()
    {
        Destroy(gameObject);
    }
}
