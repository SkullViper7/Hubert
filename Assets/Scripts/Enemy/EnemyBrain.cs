using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour
{
    /// <summary>
    /// Component which manages the hearing of the enemy.
    /// </summary>
    [field: SerializeField, Header("General")]
    public EnemyHearing EnemyHearing { get; private set; }

    /// <summary>
    /// Component which manages the vision of the enemy.
    /// </summary>
    [field: SerializeField]
    public EnemyVision EnemyVision { get; private set; }

    /// <summary>
    /// Navmesh agent of the enemy.
    /// </summary>
    public NavMeshAgent NavMeshAgent { get; private set; }

    /// <summary>
    /// The current state of the enemy.
    /// </summary>
    protected IEnemyState _currentState;

    /// <summary>
    /// A value indicating if the destination has been reached.
    /// </summary>
    private bool _destinationReached;

    private void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Called to execute the current state behaviour.
    /// </summary>
    protected void Update()
    {
        _currentState?.UpdateState();
    }

    /// <summary>
    /// Called to switch to a new state.
    /// </summary>
    /// <param name="newState"> The new state to switch. </param>
    protected IEnumerator ChangeState(IEnemyState newState)
    {
        if (_currentState != null)
            yield return StartCoroutine(_currentState.OnExit());

        _currentState = newState;

        if (_currentState != null)
            yield return StartCoroutine(_currentState.OnEnter(this));
    }

    /// <summary>
    /// Called to cancel any state and return to default state.
    /// </summary>
    public void CancelCurrentState()
    {
        _currentState.CancelState();
    }

    /// <summary>
    /// Called to get the closest waypoint from a path using NavMesh pathfinding.
    /// </summary>
    /// <param name="path"> Path of the enemy. </param>
    /// <returns></returns>
    public int GetClosestWaypointNavMesh(List<Waypoint> path)
    {
        if (path == null || path.Count == 0)
            return -1;

        NavMeshPath navPath = new();
        float shortestDistance = Mathf.Infinity;
        int closestIndex = -1;

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 targetPos = path[i].transform.position;

            // Calculating the NavMesh path from the current position to the waypoint
            if (NavMesh.CalculatePath(transform.position, targetPos, NavMesh.AllAreas, navPath)
                && navPath.status == NavMeshPathStatus.PathComplete)
            {
                // Calculating the actual path length
                float pathLength = GetPathLength(navPath);

                if (pathLength < shortestDistance)
                {
                    shortestDistance = pathLength;
                    closestIndex = i;
                }
            }
        }

        if (closestIndex == -1)
        {
            Debug.LogError("No reachable waypoint (via NavMesh).");
        }

        return closestIndex;
    }

    /// <summary>
    /// Called to calculate total length of a NavMeshPath.
    /// </summary>
    private float GetPathLength(NavMeshPath path)
    {
        float length = 0f;

        if (path.corners.Length < 2)
            return length;

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }

        return length;
    }

    /// <summary>
    /// Called to go to a destination.
    /// </summary>
    /// <param name="destination"> Destination to reach. </param>
    /// <param name="onDestinationReached"> A value indicating if the enemy has reached his destination. </param>
    /// <returns></returns>
    public IEnumerator SetDestination(Vector3 destination, Action<bool> onDestinationReached)
    {
        _destinationReached = true;
        NavMeshAgent.isStopped = false;
        NavMeshAgent.SetDestination(destination);

        yield return new WaitUntil(() => !NavMeshAgent.pathPending);

        while (!NavMeshAgent.pathPending && NavMeshAgent.remainingDistance > NavMeshAgent.stoppingDistance && _destinationReached)
        {
            //_animationController.SetWalkSpeed(NavMeshAgent.velocity.magnitude / NavMeshAgent.speed);
            yield return null;
        }

        onDestinationReached?.Invoke(_destinationReached);
    }



    /// <summary>
    /// Called to stop a movement.
    /// </summary>
    public void StopMovement()
    {
        _destinationReached = false;
        NavMeshAgent.ResetPath();
    }
}
