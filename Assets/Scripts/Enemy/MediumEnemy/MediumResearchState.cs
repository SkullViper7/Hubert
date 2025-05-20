using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MediumResearchState : IEnemyState
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
    /// Coroutine of the movement.
    /// </summary>
    private Coroutine _movementCoroutine;

    /// <summary>
    /// Coroutine of the look around.
    /// </summary>
    private Coroutine _lookAroundCoroutine;

    /// <summary>
    /// Direction of the patrol, +1 or -1 depending of if it's a ping-pong routine.
    /// </summary>
    private int _patrolDirection = 1;

    /// <summary>
    /// A value indicating if the enemy is already going to a sound.
    /// </summary>
    private bool _isAlreadyGoingToASound;

    /// <summary>
    /// A list which contains a temporary patrol.
    /// </summary>
    private List<Waypoint> _temporaryPatrol = new();

    /// <summary>
    /// The type of the temporary patrol.
    /// </summary>
    private PatrolType _temporaryPatrolType;

    /// <summary>
    /// An action to go to a sound position when one is heared.
    /// </summary>
    private Action<Vector3> _goToSoundPosition;

    /// <summary>
    /// Actions to switch to alerte state when player is seen and to switch to patrol state when research is ended.
    /// </summary>
    private Action _onPlayerSeen, _onResearchEnded;

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
        _agent.speed = _brain.ResearchWalkSpeed;
        _agent.acceleration = _brain.ResearchAcceleration;
        _agent.angularSpeed = _brain.ResearchAngularSpeed;

        // Launch animation
        _brain.MediumAnimationController.PlayResearchAnim();

        // Set listeners
        _goToSoundPosition = (Vector3 position) => _brain.StartCoroutine(GoToSoundPosition(position));
        _brain.EnemyHearing.OnSoundHeard += _goToSoundPosition;
        _onResearchEnded = () => _brain.StartCoroutine(_brain.ChangeState(_brain.MediumPatrolState, EnemyStateEnterType.Null));
        _enemyManager.OnResearchEnded += _onResearchEnded;
        //_onPlayerSeen = () => _brain.StartCoroutine(_brain.ChangeState(_brain.MediumAlerteState, EnemyStateEnterType.HasNoGoal));
        //_brain.EnemyVision.OnPlayerSeen += _onPlayerSeen;

        // If enemy has goal it means that he has to go to the last sound position
        if (enemyStateEnterType == EnemyStateEnterType.HasAGoal)
        {
            yield return _brain.StartCoroutine(GoToSoundPosition(_brain.LastSoundPosition));
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
    }

    public IEnumerator OnExit()
    {
        _brain.EnemyHearing.OnSoundHeard -= _goToSoundPosition;
        _enemyManager.OnResearchEnded -= _onResearchEnded;
        // _brain.EnemyVision.OnPlayerSeen -= _onPlayerSeen;
        CancelCoroutine(_movementCoroutine);
        CancelCoroutine(_lookAroundCoroutine);
        _isAlreadyGoingToASound = false;
        yield return null;
    }

    public void CancelState()
    {
        _brain.EnemyHearing.OnSoundHeard -= _goToSoundPosition;
        _enemyManager.OnResearchEnded -= _onResearchEnded;
        // _brain.EnemyVision.OnPlayerSeen -= _onPlayerSeen;
        CancelCoroutine(_movementCoroutine);
        CancelCoroutine(_lookAroundCoroutine);
        _isAlreadyGoingToASound = false;
    }

    /// <summary>
    /// Called to go to a sound position to check around.
    /// </summary>
    /// <param name="position"> Position of the sound. </param>
    /// <returns></returns>
    private IEnumerator GoToSoundPosition(Vector3 position)
    {
        if (!_isAlreadyGoingToASound)
        {
            _isAlreadyGoingToASound = true;

            _enemyManager.StartResearchChrono(15);

            CancelCoroutine(_movementCoroutine);

            // Launch animation
            _brain.MediumAnimationController.PlayResearchAnim();

            // Go to the last sound position
            bool reached = false;
            yield return _movementCoroutine = _brain.StartCoroutine(_brain.SetDestination(position, success => reached = success));

            // If enemy has reached the position, launch look around
            if (!reached) CancelCoroutine(_movementCoroutine);

            _isAlreadyGoingToASound = false;

            yield return _lookAroundCoroutine = _brain.StartCoroutine(_brain.LookAround(true));

            // Launch a patrol around
            StartPatrol();
        }
    }

    /// <summary>
    /// Called to launch a patrol around the enemy.
    /// </summary>
    private void StartPatrol()
    {
        CancelCoroutine(_movementCoroutine);

        // Launch animation
        _brain.MediumAnimationController.PlayResearchAnim();

        // Calculate a temporary patrol
        (List<Waypoint>, PatrolType) temporaryPatrolResult = AStarGenerator.GetPatrolAround(_brain.GetClosestWaypointFrom(_brain.transform.position, 5f), _brain.MinDistance, _brain.MaxDistance, _brain.PingPongDistance);
        _temporaryPatrol = temporaryPatrolResult.Item1;
        _temporaryPatrolType = temporaryPatrolResult.Item2;

        // Launch the patrol depending of the type
        if (_temporaryPatrolType == PatrolType.LoopPatrol)
        {
            _patrolDirection = 1;
            _movementCoroutine = _brain.StartCoroutine(GoToNextWaypoint(_brain.GetClosestWaypointNavMesh(_temporaryPatrol)));
        }
        else if (_brain.PatrolType == PatrolType.PingPongPatrol)
        {
            _movementCoroutine = _brain.StartCoroutine(GoToNextWaypoint(_brain.GetClosestWaypointNavMesh(_temporaryPatrol)));
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
        bool reached = false;

        // Go to waypoint
        yield return _brain.SetDestination(_temporaryPatrol[index].transform.position, success => reached = success);

        // If enemy has reached waypoint then continue
        if (!reached) CancelCoroutine(_movementCoroutine);

        // Check if the waypoint is a waypoint where the enemy can look around
        if (_temporaryPatrol[index].IsLookAroundWaypoint)
        {
            yield return _lookAroundCoroutine = _brain.StartCoroutine(_brain.LookAround(true));
        }

        switch (_brain.PatrolType)
        {
            case PatrolType.LoopPatrol:
                if (index + _patrolDirection > _temporaryPatrol.Count - 1)
                {
                    index = 0;
                    _movementCoroutine = _brain.StartCoroutine(GoToNextWaypoint(index));
                }
                else
                {
                    index += _patrolDirection;
                    _movementCoroutine = _brain.StartCoroutine(GoToNextWaypoint(index));
                }
                break;

            case PatrolType.PingPongPatrol:
                if (index + _patrolDirection > _temporaryPatrol.Count - 1)
                {
                    _patrolDirection = -1;
                    index += _patrolDirection;
                    _movementCoroutine = _brain.StartCoroutine(GoToNextWaypoint(index));
                }
                else if (index + _patrolDirection < 0)
                {
                    _patrolDirection = 1;
                    index += _patrolDirection;
                    _movementCoroutine = _brain.StartCoroutine(GoToNextWaypoint(index));
                }
                else
                {
                    index += _patrolDirection;
                    _movementCoroutine = _brain.StartCoroutine(GoToNextWaypoint(index));
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
