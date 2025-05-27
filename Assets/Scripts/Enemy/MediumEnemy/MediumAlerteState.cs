using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MediumAlerteState : IEnemyState
{
    /// <summary>
    /// Brain of the enemy.
    /// </summary>
    private MediumEnemyBrain _brain;

    /// <summary>
    /// Navmesh agent of the enemy.
    /// </summary>
    private NavMeshAgent _agent;

    /// <summary>
    /// Coroutine of the patrol.
    /// </summary>
    private Coroutine _patrolCoroutine;

    /// <summary>
    /// Coroutine of going to sound.
    /// </summary>
    private Coroutine _goToSoundCoroutine;

    /// <summary>
    /// Coroutine of going to player.
    /// </summary>
    private Coroutine _goToPlayerCoroutine;

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

    /// <summary>
    /// A value indicating if the enemy is in patrol.
    /// </summary>
    private bool _isInPatrol;

    /// <summary>
    /// An action to go to a sound source when one is heared.
    /// </summary>
    private Action<SoundSource> _goToSoundSource;

    /// <summary>
    /// The current sound source follow by the enemy.
    /// </summary>
    private SoundSource _currentSoundSource;

    /// <summary>
    /// Actions to switch to research state when player is seen or to switch to patrol state when alerte is ended and to cancel going to a sound when it is already checked.
    /// </summary>
    private Action _onPlayerSeen, _onAlerteEnded, _onGoingToSoundCanceled;

    /// <summary>
    /// The manager of all enemies.
    /// </summary>
    private EnemyManager _enemyManager;

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
        // Action when the alerte time is ended
        _onAlerteEnded = () => _brain.StartCoroutine(_brain.ChangeState(_brain.MediumResearchState, EnemyStateEnterType.HasNoGoal));
        // Listener when the research is ended
        _enemyManager.OnResearchEnded += _onAlerteEnded;

        // If enemy has goal it means that he has to go to the last sound position
        if (enemyStateEnterType == EnemyStateEnterType.HasAGoal)
        {
            Debug.Log("alerte at " + _brain.LastPosSeen);
        }
        else if (enemyStateEnterType == EnemyStateEnterType.HasNoGoal)
        {
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
        //_brain.EnemyHearing.OnSoundHeard -= _goToSoundSource;
        //_enemyManager.OnResearchEnded -= _onResearchEnded;
        //// _brain.EnemyVision.OnPlayerSeen -= _onPlayerSeen;

        //// Unsubscribe to the last source
        //_enemyManager.Unsubscribe(_currentSoundSource, _onGoingToSoundCanceled);

        //CancelCoroutine(_patrolCoroutine);
        //CancelCoroutine(_goToSoundCoroutine);
        //_brain.StopMovement();
        //_brain.StopLookingAround();
        //_brain.StopAstonishment();
        yield return null;
    }

    /// <summary>
    /// Called to launch a patrol around the enemy.
    /// </summary>
    private void StartPatrol()
    {
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
