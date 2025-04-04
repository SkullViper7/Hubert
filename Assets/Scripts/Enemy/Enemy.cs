using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private List<Waypoint> _path;

    private NavMeshAgent _navMeshAgent;

    public static event Action OnStopSearching;

    public Coroutine EnemySearchingCoroutine;

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

    public void SetPath(Vector3 soundPosition)
    {
        EnemySearchingCoroutine = StartCoroutine(GoToSoundPosition(soundPosition));
    }

    public void ChasePlayer(Vector3 playerPosition)
    {
        _navMeshAgent.SetDestination(playerPosition);
    }

    public IEnumerator GoToSoundPosition(Vector3 soundPosition)
    {
        _navMeshAgent.SetDestination(soundPosition);

        while (_navMeshAgent.pathPending || _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance)
        {
            yield return null;
        }

        OnStopSearching?.Invoke();
    }
}
