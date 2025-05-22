using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MediumPatrolState : IEnemyState
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
    /// Direction of the patrol, +1 or -1 depending of if it's a ping-pong routine.
    /// </summary>
    private int _patrolDirection = 1;

    /// <summary>
    /// An action to switch to the research state when a sound is heared.
    /// </summary>
    private Action<SoundSource> _onSoundHeared;

    public IEnumerator OnEnter(EnemyBrain enemyBrain, EnemyStateEnterType enemyStateEnterType)
    {
        // Get components
        _brain = (MediumEnemyBrain)enemyBrain;
        _agent = _brain.NavMeshAgent;

        // Get values
        _agent.speed = _brain.PatrolWalkSpeed;
        _agent.acceleration = _brain.PatrolAcceleration;

        // Launch animation
        _brain.MediumAnimationController.PlayPatrolAnim();

        // Set listeners
        _onSoundHeared = (SoundSource source) =>
        {
            _brain.HasHeared(source);
            _brain.StartCoroutine(_brain.ChangeState(_brain.MediumResearchState, EnemyStateEnterType.HasAGoal));
        };
        _brain.EnemyHearing.OnSoundHeard += _onSoundHeared;

        // Launch the patrol depending of the type
        if (_brain.PatrolType == PatrolType.LoopPatrol)
        {
            _patrolDirection = 1;
            _movementCoroutine = _brain.StartCoroutine(GoToNextWaypoint(_brain.GetClosestWaypointNavMesh(_brain.Path)));
        }
        else if (_brain.PatrolType == PatrolType.PingPongPatrol)
        {
            _movementCoroutine = _brain.StartCoroutine(GoToNextWaypoint(_brain.GetClosestWaypointNavMesh(_brain.Path)));
        }
        else if (_brain.PatrolType == PatrolType.Fixed)
        {
            _movementCoroutine = _brain.StartCoroutine(StartFixedRoutine());
        }

        yield return null;
    }

    public void UpdateState()
    {
        _brain.AnimationController.SetWalkSpeed(_brain.NavMeshAgent.velocity.magnitude / _brain.NavMeshAgent.speed);
    }

    public IEnumerator OnExit()
    {
        _brain.EnemyHearing.OnSoundHeard -= _onSoundHeared;
        _brain.StopMovement();
        CancelCoroutine(_movementCoroutine);
        _brain.StopLookingAround();
        yield return null;
    }

    public void CancelState()
    {
        _brain.EnemyHearing.OnSoundHeard -= _onSoundHeared;
        _brain.StopMovement();
        CancelCoroutine(_movementCoroutine);
        _brain.StopLookingAround();
    }

    /// <summary>
    /// Called to go to a waypoint and launch the next.
    /// </summary>
    /// <param name="index"> Index of the waypoint to go to. </param>
    /// <returns></returns>z
    private IEnumerator GoToNextWaypoint(int index)
    {
        // Go to waypoint
        bool reached = false;
        yield return _brain.SetDestination(_brain.Path[index].transform.position, success => reached = success);

        // If enemy has reached waypoint then continue
        if (!reached) CancelCoroutine(_movementCoroutine);

        // Check if the waypoint is a waypoint where the enemy can look around
        if (_brain.Path[index].IsLookAroundWaypoint)
        {
            yield return _brain.LookAround(false, "LookAroundPatrol");
        }

        switch (_brain.PatrolType)
        {
            case PatrolType.LoopPatrol:
                if (index + _patrolDirection > _brain.Path.Count - 1)
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
                if (index + _patrolDirection > _brain.Path.Count - 1)
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
    /// Called to wait a time and then look around.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartFixedRoutine()
    {
        // Go to waypoint
        bool reached = false;
        yield return _brain.SetDestination(_brain.Path[0].transform.position, success => reached = success);

        // If enemy has reached waypoint then continue
        if (!reached) CancelCoroutine(_movementCoroutine);

        yield return _brain.LookAround(true, "LookAroundPatrol");

        _movementCoroutine = _brain.StartCoroutine(FixedRoutine());
    }

    /// <summary>
    /// Called to go to the fixed point and launch the fixed routine.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FixedRoutine()
    {
        int waitingTime = UnityEngine.Random.Range(_brain.FixedWaypointDuration.Min, _brain.FixedWaypointDuration.Max);

        yield return new WaitForSeconds(waitingTime);

        yield return _brain.LookAround(true, "LookAroundPatrol");

        _movementCoroutine = _brain.StartCoroutine(FixedRoutine());
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
