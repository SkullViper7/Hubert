using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MediumAlerteState : IEnemyState
{
    #region General
    /// <summary>
    /// Brain of the enemy.
    /// </summary>
    private MediumEnemyBrain _brain;

    /// <summary>
    /// Navmesh agent of the enemy.
    /// </summary>
    private NavMeshAgent _agent;

    /// <summary>
    /// Actions when the room is changed.
    /// </summary>
    private Action<Room, Room> _roomChanged;

    /// <summary>
    /// Actions to switch to patrol state when alerte is ended.
    /// </summary>
    private Action _alerteEnded;

    /// <summary>
    /// The manager of all enemies.
    /// </summary>
    private EnemyManager _enemyManager;
    #endregion

    #region Patrol
    /// <summary>
    /// Coroutine of the patrol.
    /// </summary>
    private Coroutine _patrolCoroutine;

    /// <summary>
    /// Direction of the patrol, +1 or -1 depending of if it's a ping-pong routine.
    /// </summary>
    private int _patrolDirection = 1;

    /// <summary>
    /// A list which contains a temporary patrol.
    /// </summary>
    private List<Waypoint> _temporaryPatrol = new();

    /// <summary>
    /// The type of the temporary patrol.
    /// </summary>
    private PatrolType _temporaryPatrolType;
    #endregion

    #region Sound
    /// <summary>
    /// Coroutine of going to sound.
    /// </summary>
    private Coroutine _goToSoundCoroutine;

    /// <summary>
    /// An action to go to a sound source when one is heared.
    /// </summary>
    private Action<SoundSource> _goToSoundSource;

    /// <summary>
    /// The current sound source followed by the enemy.
    /// </summary>
    private SoundSource _currentSoundSource;

    /// <summary>
    /// Actions to cancel going to a sound when it is already checked.
    /// </summary>
    private Action _goingToSoundCanceled;
    #endregion

    #region Vision
    /// <summary>
    /// Coroutine of going to player.
    /// </summary>
    private Coroutine _goToPlayerCoroutine;

    /// <summary>
    /// Action when enemy is astonished.
    /// </summary>
    private Action _astonishment;

    /// <summary>
    /// Action to go to the player position.
    /// </summary>
    private Action<PlayerPosition> _goToPlayerPos;

    /// <summary>
    /// The current player pos followed by the enemy.
    /// </summary>
    private PlayerPosition _currentPlayerPos;

    /// <summary>
    /// Actions to cancel going to player pos.
    /// </summary>
    private Action _goingToPlayerPosCanceled;
    #endregion

    #region Aim
    /// <summary>
    /// Action when enemy is enough close to aim.
    /// </summary>
    private Action _aimTriggered;
    #endregion

    public IEnumerator OnEnter(EnemyBrain enemyBrain, EnemyStateEnterType enemyStateEnterType)
    {
        // Get components
        _brain = (MediumEnemyBrain)enemyBrain;
        _agent = _brain.NavMeshAgent;
        _enemyManager = EnemyManager.Instance;

        // Get values
        _agent.speed = _brain.AlerteWalkSpeed;
        _agent.acceleration = _brain.AlerteAcceleration;
        _brain.EnemyVision.DetectionRange = _brain.AlerteVisionRange;

        // Set listeners
        // Action when a sound is heared
        _goToSoundSource = (SoundSource source) =>
        {
            if (_brain.CurrentRoom.PlayerIsCurrentlySeen) return;
            CancelGoingToSoundSource();
            _goToSoundCoroutine = _brain.StartCoroutine(GoToSoundSource(source));
        };
        // Listener when the sound is heared
        _brain.EnemyHearing.OnSoundHeard += _goToSoundSource;

        // Action when going to a sound is canceled
        _goingToSoundCanceled = () => _patrolCoroutine = _brain.StartCoroutine(SoundHasAlreadyBeenChecked());

        // Action when player is seen
        _astonishment = () =>
        {
            // Play astonishment
            CancelGoingToPlayerPos();
            _goToPlayerCoroutine = _brain.StartCoroutine(PlayerIsSeen(false));
        };
        // Event when player is seen
        _brain.OnPlayerSeenForTheFirstTime += _astonishment;

        // Action when player position is updated
        _goToPlayerPos = (PlayerPosition position) =>
        {
            // Go to player position
            CancelGoingToPlayerPos();
            _goToPlayerCoroutine = _brain.StartCoroutine(GoToPlayerPos(position));
        };

        // Action when going to a player position is canceled
        _goingToPlayerPosCanceled = () =>
        {
            _patrolCoroutine = _brain.StartCoroutine(PositionHasAlreadyBeenChecked());
        };

        // Action when enemy is enough close to aim
        _aimTriggered = () =>
        {
            _brain.StartCoroutine(_brain.ChangeState(_brain.MediumAimingState, EnemyStateEnterType.Null));
        };
        // Listener when enemy is enough close to aim
        _brain.OnAimTriggered += _aimTriggered;

        // Action when room is changed
        _roomChanged = (Room oldRoom, Room newRoom) =>
        {
            oldRoom.UnsubscribeSoundSource(_currentSoundSource, _goingToSoundCanceled);
            newRoom.SubscribeSoundSource(_currentSoundSource, _goingToSoundCanceled);
            oldRoom.UnsubscribePlayerPos(_currentPlayerPos, _goingToSoundCanceled);
            newRoom.SubscribePlayerPos(_goingToPlayerPosCanceled);
            oldRoom.OnAlerteEnded -= _alerteEnded;
            newRoom.OnAlerteEnded += _alerteEnded;
        };
        // Listener when room is changed
        _brain.OnRoomChanged += _roomChanged;

        // Action when the alerte time is ended
        _alerteEnded = () =>
        {
            _brain.StartCoroutine(_brain.ChangeState(_brain.MediumResearchState, EnemyStateEnterType.HasNoGoal));
        };
        // Listener when the alerte is ended
        _brain.CurrentRoom.OnAlerteEnded += _alerteEnded;

        // If enemy has goal it means that he has to go to the first player position seen
        if (enemyStateEnterType == EnemyStateEnterType.HasAGoal)
        {
            // Play astonishment
            CancelGoingToPlayerPos();
            _goToPlayerCoroutine = _brain.StartCoroutine(PlayerIsSeen(true));
        }
        else if (enemyStateEnterType == EnemyStateEnterType.HasAGoalButNoAstonishment)
        {
            // Don't play animation
            _brain.CurrentRoom.OnPlayerPosUpdated += _goToPlayerPos;
            CancelGoingToPlayerPos();
            _goToPlayerCoroutine = _brain.StartCoroutine(GoToPlayerPos(_brain.CurrentRoom.LastKnownPlayerPos));
        }
        else if (enemyStateEnterType == EnemyStateEnterType.HasNoGoal)
        {
            _brain.CurrentRoom.OnPlayerPosUpdated += _goToPlayerPos;

            // Launch a patrol
            StartPatrol();
        }

        yield return null;
    }

    public void UpdateState()
    {
        _brain.AnimationController.SetWalkSpeed(_brain.NavMeshAgent.velocity.magnitude / _brain.NavMeshAgent.speed);
        _brain.TryTransmiteState();
    }

    public IEnumerator OnExit()
    {
        _brain.EnemyHearing.OnSoundHeard -= _goToSoundSource;
        _brain.OnPlayerSeenForTheFirstTime -= _astonishment;
        _brain.OnRoomChanged -= _roomChanged;
        _brain.CurrentRoom.OnAlerteEnded -= _alerteEnded;
        _brain.CurrentRoom.OnPlayerPosUpdated -= _goToPlayerPos;
        _brain.OnAimTriggered -= _aimTriggered;

        // Unsubscribe to the last position
        _brain.CurrentRoom.UnsubscribePlayerPos(_currentPlayerPos, _goingToPlayerPosCanceled);
        _brain.CurrentRoom.UnsubscribeSoundSource(_currentSoundSource, _goingToSoundCanceled);

        CancelCoroutine(_patrolCoroutine);
        CancelCoroutine(_goToPlayerCoroutine);
        CancelCoroutine(_goToSoundCoroutine);
        _brain.StopMovement();
        _brain.StopLookingAround();
        _brain.StopAstonishment();

        yield return null;
    }

    #region Sound
    /// <summary>
    /// Called to go to a sound source to check around.
    /// </summary>
    /// <param name="soundSource"> Source of the sound. </param>
    /// <returns></returns>
    private IEnumerator GoToSoundSource(SoundSource soundSource)
    {
        CancelCoroutine(_patrolCoroutine);
        CancelCoroutine(_goToPlayerCoroutine);
        _brain.StopMovement();
        _brain.StopLookingAround();
        _brain.StopAstonishment();

        // Subscribe to the new source
        _currentSoundSource = soundSource;
        _brain.CurrentRoom.SubscribeSoundSource(_currentSoundSource, _goingToSoundCanceled);

        if (_currentSoundSource.SoundType == SoundType.OneShot)
        {
            yield return _brain.Astonishment("VisionAstonishmentLow");
        }

        // Launch animation
        _brain.MediumAnimationController.PlayAlerteAnim();

        // Go to the last sound position
        bool reached = false;
        yield return _brain.SetDestination(_currentSoundSource.Position, success => reached = success);

        yield return _brain.LookAround(true, "LookAroundAlerte");

        if (reached)
        {
            _brain.CurrentRoom.InvokeSoundSource(_currentSoundSource, _goingToSoundCanceled);
        }

        // Launch a patrol around
        StartPatrol();
    }

    /// <summary>
    /// Called to cancel going to a sound.
    /// </summary>
    private void CancelGoingToSoundSource()
    {
        // Unsubscribe to the last source
        _brain.CurrentRoom.UnsubscribeSoundSource(_currentSoundSource, _goingToSoundCanceled);

        CancelCoroutine(_goToSoundCoroutine);
        _brain.StopMovement();
        _brain.StopLookingAround();
        _brain.StopAstonishment();
    }

    /// <summary>
    /// Called when going to a sound is canceled cause is already checked.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SoundHasAlreadyBeenChecked()
    {
        CancelGoingToSoundSource();

        yield return _brain.LookAround(true, "LookAroundAlerte");

        // Launch a patrol around
        StartPatrol();
    }
    #endregion

    #region Vision
    /// <summary>
    /// Called play an astonishment animation.
    /// </summary>
    /// <param name="isFirstTime"> A value to indicate if it's the first time we see it. </param>
    /// <returns></returns>
    private IEnumerator PlayerIsSeen(bool isFirstTime)
    {
        // Remove event when player is seen
        _brain.CurrentRoom.OnPlayerPosUpdated -= _goToPlayerPos;

        CancelCoroutine(_patrolCoroutine);
        CancelCoroutine(_goToSoundCoroutine);
        _brain.StopMovement();
        _brain.StopLookingAround();
        _brain.StopAstonishment();

        if (isFirstTime)
        {
            // Play astonishment animation
            yield return _brain.Astonishment("VisionAstonishment");
        }
        else
        {
            // Play soft astonishment animation
            yield return _brain.Astonishment("VisionAstonishmentLow");
        }

        // Event when player is seen
        _brain.CurrentRoom.OnPlayerPosUpdated += _goToPlayerPos;

        _goToPlayerCoroutine = _brain.StartCoroutine(GoToPlayerPos(_brain.CurrentRoom.LastKnownPlayerPos));
    }

    /// <summary>
    /// Called to go to a player position.
    /// </summary>
    /// <param name="playerPos"> Player position to go. </param>
    /// <returns></returns>
    private IEnumerator GoToPlayerPos(PlayerPosition playerPos)
    {
        CancelCoroutine(_patrolCoroutine);
        CancelCoroutine(_goToSoundCoroutine);
        _brain.StopMovement();
        _brain.StopLookingAround();
        _brain.StopAstonishment();

        // Subscribe to the new player position
        _currentPlayerPos = playerPos;
        _brain.CurrentRoom.SubscribePlayerPos(_goingToPlayerPosCanceled);

        // Launch timer
        _brain.CurrentRoom.StartAlerteChrono(_enemyManager.AlerteTimer);

        // Launch animation
        _brain.MediumAnimationController.PlayAlerteAnim();

        // Go to the last player position
        bool reached = false;
        yield return _brain.SetDestination(_currentPlayerPos.Position, success => reached = success);

        yield return _brain.LookAround(true, "LookAroundAlerte");

        if (reached)
        {
            _brain.CurrentRoom.InvokePlayerPos(_currentPlayerPos, _goingToPlayerPosCanceled);
        }

        // Launch a patrol around
        StartPatrol();
    }

    /// <summary>
    /// Called to cancel going to a player position.
    /// </summary>
    private void CancelGoingToPlayerPos()
    {
        // Unsubscribe to the last source
        _brain.CurrentRoom.UnsubscribePlayerPos(_currentPlayerPos, _goingToPlayerPosCanceled);

        CancelCoroutine(_goToPlayerCoroutine);
        _brain.StopMovement();
        _brain.StopLookingAround();
        _brain.StopAstonishment();
    }

    /// <summary>
    /// Called when going to a player position is canceled cause is already checked.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PositionHasAlreadyBeenChecked()
    {
        CancelGoingToPlayerPos();

        yield return _brain.LookAround(true, "LookAroundAlerte");

        // Launch a patrol around
        StartPatrol();
    }
    #endregion

    #region Patrol
    /// <summary>
    /// Called to launch a patrol around the enemy.
    /// </summary>
    private void StartPatrol()
    {
        CancelCoroutine(_patrolCoroutine);
        CancelCoroutine(_goToSoundCoroutine);
        CancelCoroutine(_goToPlayerCoroutine);
        _brain.StopMovement();
        _brain.StopLookingAround();
        _brain.StopAstonishment();

        // Calculate a temporary patrol
        (List<Waypoint>, PatrolType) temporaryPatrolResult = AStarGenerator.GetPatrolAround(_brain.GetClosestWaypointFrom(_brain.transform.position, 5f), _brain.MinDistance, _brain.MaxDistance, _brain.PingPongDistance);
        _temporaryPatrol = temporaryPatrolResult.Item1;
        _temporaryPatrolType = temporaryPatrolResult.Item2;

        // Launch the patrol depending of the type
        if (_temporaryPatrolType == PatrolType.LoopPatrol)
        {
            _patrolDirection = 1;
            _patrolCoroutine = _brain.StartCoroutine(GoToNextWaypoint(_brain.GetClosestWaypointNavMesh(_temporaryPatrol)));
        }
        else if (_brain.PatrolType == PatrolType.PingPongPatrol)
        {
            _patrolCoroutine = _brain.StartCoroutine(GoToNextWaypoint(_brain.GetClosestWaypointNavMesh(_temporaryPatrol)));
        }
        else
        {
            Debug.LogError("No fixed patrol here");
        }
    }

    /// <summary>
    /// Called to go to a waypoint and launch the next.
    /// </summary>
    /// <param name="index"> Index of the waypoint to go to. </param>
    /// <returns></returns>
    private IEnumerator GoToNextWaypoint(int index)
    {
        CancelCoroutine(_goToPlayerCoroutine);
        CancelCoroutine(_goToSoundCoroutine);
        _brain.StopMovement();
        _brain.StopLookingAround();
        _brain.StopAstonishment();

        // Launch animation
        _brain.MediumAnimationController.PlayAlerteAnim();

        // Go to waypoint
        bool reached = false;
        yield return _brain.SetDestination(_temporaryPatrol[index].transform.position, success => reached = success);

        // If enemy has reached waypoint then continue
        if (!reached) CancelCoroutine(_patrolCoroutine);

        // Check if the waypoint is a waypoint where the enemy can look around
        if (_temporaryPatrol[index].IsLookAroundWaypoint)
        {
            yield return _brain.LookAround(true, "LookAroundAlerte");
        }

        switch (_temporaryPatrolType)
        {
            case PatrolType.LoopPatrol:
                if (index + _patrolDirection > _temporaryPatrol.Count - 1)
                {
                    index = 0;
                    _patrolCoroutine = _brain.StartCoroutine(GoToNextWaypoint(index));
                }
                else
                {
                    index += _patrolDirection;
                    _patrolCoroutine = _brain.StartCoroutine(GoToNextWaypoint(index));
                }
                break;

            case PatrolType.PingPongPatrol:
                if (index + _patrolDirection > _temporaryPatrol.Count - 1)
                {
                    _patrolDirection = -1;
                    index += _patrolDirection;
                    _patrolCoroutine = _brain.StartCoroutine(GoToNextWaypoint(index));
                }
                else if (index + _patrolDirection < 0)
                {
                    _patrolDirection = 1;
                    index += _patrolDirection;
                    _patrolCoroutine = _brain.StartCoroutine(GoToNextWaypoint(index));
                }
                else
                {
                    index += _patrolDirection;
                    _patrolCoroutine = _brain.StartCoroutine(GoToNextWaypoint(index));
                }
                break;
        }
    }
    #endregion

    /// <summary>
    /// Called to cancel a coroutine.
    /// </summary>
    /// <param name="coroutine"> The coroutine to cancel. </param>
    private void CancelCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            _brain.StopCoroutine(coroutine);
        }
    }
}
