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
    /// Component which manages animations.
    /// </summary>
    [field: SerializeField]
    public EnemyAnimationController AnimationController { get; private set; }

    /// <summary>
    /// Navmesh agent of the enemy.
    /// </summary>
    public NavMeshAgent NavMeshAgent { get; private set; }

    /// <summary>
    /// The position of the last sound heared.
    /// </summary>
    public Vector3 LastSoundPosition { get; private set; }

    /// <summary>
    /// The current state of the enemy.
    /// </summary>
    protected IEnemyState _currentState;

    /// <summary>
    /// A value indicating if the destination has been reached.
    /// </summary>
    private bool _destinationReached;

    /// <summary>
    /// The probability to look around at a defined waypoint (in percents, only in loop and ping-pong mode).
    /// </summary>
    [field: SerializeField, Range(0, 100)]
    public int LookAroundProbability { get; private set; }

    /// <summary>
    /// An action to manage if the look around animation is finished.
    /// </summary>
    private Action _onLookAroundFinished;

    protected virtual void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {
        EnemyHearing.OnSoundHeard += SetSoundPosition;
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
    /// <param name="enemyStateEnterType"> A value to know of the enemy has directly a goal when he enter a state. </param>
    public IEnumerator ChangeState(IEnemyState newState, EnemyStateEnterType enemyStateEnterType)
    {
        if (_currentState != null)
            yield return StartCoroutine(_currentState.OnExit());

        _currentState = newState;

        if (_currentState != null)
            yield return StartCoroutine(_currentState.OnEnter(this, enemyStateEnterType));
    }

    /// <summary>
    /// Called to cancel any state and return to default state.
    /// </summary>
    public void CancelCurrentState()
    {
        _currentState.CancelState();
    }

    /// <summary>
    /// Called to set the position of the last sound heared.
    /// </summary>
    /// <param name="position"> Position of the sound. </param>
    private void SetSoundPosition(Vector3 position)
    {
        LastSoundPosition = position;
    }

    /// <summary>
    /// Called to get the closest waypoint around a position.
    /// </summary>
    /// <param name="position"> Origin of the check. </param>
    /// <param name="radius"> Radius of the check. </param>
    /// <returns></returns>
    public Waypoint GetClosestWaypointFrom(Vector2 position, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(position, radius, LayerMask.GetMask("Waypoint"));

        if (colliders.Length < 1) return GetClosestWaypointFrom(position, radius + 5f);

        List<Waypoint> waypoints = new();

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent<Waypoint>(out _))
            {
                waypoints.Add(colliders[i].GetComponent<Waypoint>());
            }
        }

        return waypoints[GetClosestWaypointNavMesh(waypoints)];
    }

    /// <summary>
    /// Called to get the closest waypoint from a list using NavMesh pathfinding.
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
            AnimationController.SetWalkSpeed(NavMeshAgent.velocity.magnitude / NavMeshAgent.speed);
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

    /// <summary>
    /// Called to determine if the enemy has to look around him.
    /// </summary>
    /// <param name="isObligatory"> A value indicating if the look around is obligatory or if it's determined by probability. </param>
    /// <returns></returns>
    public IEnumerator LookAround(bool isObligatory)
    {
        if (UnityEngine.Random.Range(0, 100) > LookAroundProbability && !isObligatory)
            yield break;

        AnimationController.PlayLookAroundAnim();

        bool eventFired = false;

        _onLookAroundFinished = () => eventFired = true;

        AnimationController.OnFinishToLookAround += _onLookAroundFinished;

        yield return new WaitUntil(() => eventFired);

        AnimationController.ReturnToPreviousState();

        // Clean
        if (_onLookAroundFinished != null)
        {
            AnimationController.OnFinishToLookAround -= _onLookAroundFinished;
            _onLookAroundFinished = null;
        }
    }
}
