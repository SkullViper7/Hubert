using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour
{
    #region General
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
    /// Angular speed of the agent. (between 2 and 10 is good)
    /// </summary>
    [field: SerializeField]
    public float AngularSpeed { get; private set; } = 5f;

    /// <summary>
    /// The probability to look around at a defined waypoint (in percents, only in loop and ping-pong mode).
    /// </summary>
    [field: SerializeField, Range(0, 100)]
    public int LookAroundProbability { get; private set; }

    /// <summary>
    /// The position that the target UI focus.
    /// </summary>
    [field: SerializeField]
    public Transform TargetTransform { get; private set; }

    /// <summary>
    /// The radius around the enemy in which a state can be transmited.
    /// </summary>
    [field: SerializeField]
    public float TransmissionRadius { get; private set; }

    /// <summary>
    /// Navmesh agent of the enemy.
    /// </summary>
    public NavMeshAgent NavMeshAgent { get; private set; }

    /// <summary>
    /// A filter to calculating paths.
    /// </summary>
    private NavMeshQueryFilter _navMeshQueryFilter;

    /// <summary>
    /// The current room in which enemy is.
    /// </summary>
    public Room CurrentRoom { get; private set; }

    /// <summary>
    /// An event to indicate that the current room has changed.
    /// </summary>
    public event Action<Room> OnRoomChanged;

    /// <summary>
    /// The source of the last sound heared.
    /// </summary>
    public SoundSource LastSoundHeared { get; private set; }

    /// <summary>
    /// An event to indicate that the player has been seen for the first time.
    /// </summary>
    public event Action OnPlayerSeenForTheFirstTime;

    /// <summary>
    /// The current state of the enemy.
    /// </summary>
    public IEnemyState CurrentState { get; private set; }

    /// <summary>
    /// A value indicating that the enemy is already changing to a new state.
    /// </summary>
    private bool _isAlreadyChangingState;

    /// <summary>
    /// A value indicating if the movement is canceled.
    /// </summary>
    private bool _isMovementCanceled;

    /// <summary>
    /// A value indicating if the look around is canceled.
    /// </summary>
    private bool _isLookAroundCanceled;

    /// <summary>
    /// An action to manage if the look around animation is finished.
    /// </summary>
    private Action _lookAroundFinished;

    /// <summary>
    /// A value indicating if the astonishment is canceled.
    /// </summary>
    private bool _isAstonishmentCanceled;

    /// <summary>
    /// An action to manage if the astonishment animation is finished.
    /// </summary>
    private Action _astonishmentFinished;

    /// <summary>
    /// An event for when the enemy is hit.
    /// </summary>
    public event Action OnHit;

    /// <summary>
    /// Dead state of the enemy.
    /// </summary>
    private readonly MediumDeadState _deadState = new();
    #endregion

    protected virtual void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        NavMeshAgent.updateRotation = false;
        _navMeshQueryFilter = new NavMeshQueryFilter
        {
            agentTypeID = NavMeshAgent.agentTypeID,
            areaMask = NavMesh.AllAreas
        };
    }

    protected virtual void Start()
    {
        EnemyVision.OnPlayerSeen += HasSeen;

        EnemyHearing.OnSoundHeard += HasHeared;
    }

    /// <summary>
    /// Called to execute the current state behaviour.
    /// </summary>
    protected virtual void FixedUpdate()
    {
        CurrentState?.UpdateState();
        UpdateRotation();
    }

    /// <summary>
    /// Called to switch to a new state.
    /// </summary>
    /// <param name="newState"> The new state to switch. </param>
    /// <param name="enemyStateEnterType"> A value to know of the enemy has directly a goal when he enter a state. </param>
    public IEnumerator ChangeState(IEnemyState newState, EnemyStateEnterType enemyStateEnterType)
    {
        if (newState != CurrentState && !_isAlreadyChangingState)
        {
            _isAlreadyChangingState = true;
            if (CurrentState != null)
                yield return StartCoroutine(CurrentState.OnExit());

            CurrentState = newState;
            _isAlreadyChangingState = false;

            if (CurrentState != null)
                yield return StartCoroutine(CurrentState.OnEnter(this, enemyStateEnterType));
        }
    }

    /// <summary>
    /// Called to indicate to the enemy that he is in a new room.
    /// </summary>
    /// <param name="newRoom"> The new room. </param>
    public void IsInNewRoom(Room newRoom)
    {
        OnRoomChanged?.Invoke(CurrentRoom);
        CurrentRoom = newRoom;
    }

    /// <summary>
    /// Called to set the source of the last sound heared.
    /// </summary>
    /// <param name="soundSource"> Source of the sound. </param>
    private void HasHeared(SoundSource soundSource)
    {
        LastSoundHeared = soundSource;
    }

    /// <summary>
    /// Called when the enemy has seen the player to process the information.
    /// </summary>
    /// <param name="position"> Position of the player. </param>
    /// <param name="playerSeenContext"> Context of the vision. </param>
    private void HasSeen(Vector3 position, PlayerSeenContext playerSeenContext)
    {
        if (CurrentRoom != null)
        {
            if (playerSeenContext == PlayerSeenContext.FirstTime)
            {
                OnPlayerSeenForTheFirstTime?.Invoke();
            }

            CurrentRoom.TryUpdatePlayerPos(position, playerSeenContext);
        }
    }

    /// <summary>
    /// Called to get the closest waypoint around a position.
    /// </summary>
    /// <param name="position"> Origin of the check. </param>
    /// <param name="radius"> Radius of the check. </param>
    /// <returns></returns>
    public Waypoint GetClosestWaypointFrom(Vector3 position, float radius)
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
            if (NavMesh.CalculatePath(transform.position, targetPos, _navMeshQueryFilter, navPath)
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
        _isMovementCanceled = false;
        NavMeshAgent.isStopped = false;
        NavMeshAgent.ResetPath();

        NavMeshPath navPath = new();
        if (NavMesh.CalculatePath(transform.position, destination, _navMeshQueryFilter, navPath)
                && navPath.status == NavMeshPathStatus.PathComplete)
        {
            NavMeshAgent.SetPath(navPath);
        }

        yield return new WaitUntil(() => !NavMeshAgent.pathPending);

        while (!NavMeshAgent.pathPending && NavMeshAgent.remainingDistance > NavMeshAgent.stoppingDistance && !_isMovementCanceled)
        {
            AnimationController.SetWalkSpeed(NavMeshAgent.velocity.magnitude / NavMeshAgent.speed);
            yield return null;
        }

        onDestinationReached?.Invoke(!_isMovementCanceled);
    }

    /// <summary>
    /// Called to update the rotation of the enemy in the direction of the movement.
    /// </summary>
    private void UpdateRotation()
    {
        // No rotation if it doesn't move
        if (NavMeshAgent.velocity.sqrMagnitude < 0.01f)
            return;

        // Direction of motion on the XZ plane only
        Vector3 direction = new Vector3(NavMeshAgent.velocity.x, 0, NavMeshAgent.velocity.z).normalized;

        if (direction == Vector3.zero)
            return;

        // Calculate target rotation on Y axis only
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        // Smooth rotation with Slerp
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            AngularSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// Called to stop a movement.
    /// </summary>
    public void StopMovement()
    {
        _isMovementCanceled = true;
        NavMeshAgent.ResetPath();
    }

    /// <summary>
    /// Called to determine if the enemy has to look around him.
    /// </summary>
    /// <param name="isObligatory"> A value indicating if the look around is obligatory or if it's determined by probability. </param>
    /// <param name="trigger"> The trigger of the animation. </param>
    /// <returns></returns>
    public IEnumerator LookAround(bool isObligatory, string trigger)
    {
        if (UnityEngine.Random.Range(0, 100) > LookAroundProbability && !isObligatory)
            yield break;

        _isLookAroundCanceled = false;

        AnimationController.PlayLookAroundAnim(trigger);

        bool eventFired = false;

        _lookAroundFinished = () => eventFired = true;

        AnimationController.OnFinishToLookAround += _lookAroundFinished;

        while (!eventFired && !_isLookAroundCanceled)
        {
            yield return null;
        }

        // Clean
        if (_lookAroundFinished != null)
        {
            AnimationController.OnFinishToLookAround -= _lookAroundFinished;
            _lookAroundFinished = null;
        }
    }

    /// <summary>
    /// Called to stop looking around.
    /// </summary>
    public void StopLookingAround()
    {
        _isLookAroundCanceled = true;
    }

    /// <summary>
    /// Called to play an astonishment animation when enemy hears something or sees player.
    /// <param name="trigger"> The trigger of the animation. </param>
    /// </summary>
    public IEnumerator Astonishment(string trigger)
    {
        _isAstonishmentCanceled = false;

        AnimationController.PlayAstonishmentAnim(trigger);

        bool eventFired = false;

        _astonishmentFinished = () => eventFired = true;

        AnimationController.OnFinishAstonishment += _astonishmentFinished;

        while (!eventFired && !_isAstonishmentCanceled)
        {
            yield return null;
        }

        // Clean
        if (_astonishmentFinished != null)
        {
            AnimationController.OnFinishAstonishment -= _astonishmentFinished;
            _astonishmentFinished = null;
        }
    }

    /// <summary>
    /// Called to stop astonishment.
    /// </summary>
    public void StopAstonishment()
    {
        _isAstonishmentCanceled = true;
    }

    /// <summary>
    /// Called to try to transmite the state to an other enemy.
    /// </summary>
    public virtual void TryTransmiteState()
    {
        return;
    }

    /// <summary>
    /// Called to transmite a more active state to the enemy.
    /// </summary>
    /// <param name="stateToTransmite"> The state to transmite. </param>
    public virtual void TransmitState(IEnemyState stateToTransmite)
    {
        return;
    }

    #region Death
    /// <summary>
    /// Called to death.
    /// </summary>
    public void Death(EnemyStateEnterType enemyStateEnterType)
    {
        StartCoroutine(ChangeState(_deadState, enemyStateEnterType));
    }

    /// <summary>
    /// Called when the player hit the enemy in the animation.
    /// </summary>
    public void HasBeenHit()
    {
        OnHit?.Invoke();
    }
    #endregion
}
