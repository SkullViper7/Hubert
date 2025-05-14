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
    /// Coroutine of the patrol.
    /// </summary>
    private Coroutine _patrolRoutine;

    /// <summary>
    /// Direction of the patrol, +1 or -1 depending of if it's a ping-pong routine.
    /// </summary>
    private int _patrolDirection = 1;

    /// <summary>
    /// An action to manage if the look around animation is finished.
    /// </summary>
    private Action _onLookAroundFinished;

    public IEnumerator OnEnter(EnemyBrain enemyBrain)
    {
        // Get components
        _brain = (MediumEnemyBrain)enemyBrain;
        _agent = _brain.NavMeshAgent;

        // Get values
        _agent.speed = _brain.PatrolWalkSpeed;
        _agent.acceleration = _brain.PatrolAcceleration;
        _agent.angularSpeed = _brain.PatrolAngularSpeed;

        // Launch the patrol depending of the type
        if (_brain.PatrolType == PatrolType.LoopPatrol)
        {
            _patrolDirection = 1;
            _patrolRoutine = _brain.StartCoroutine(StartPatrol(_brain.GetClosestWaypointNavMesh(_brain.Path)));
        }
        else if (_brain.PatrolType == PatrolType.PingPongPatrol)
        {
            _patrolRoutine = _brain.StartCoroutine(StartPatrol(_brain.GetClosestWaypointNavMesh(_brain.Path)));
        }
        else if (_brain.PatrolType == PatrolType.Fixed)
        {
            _patrolRoutine = _brain.StartCoroutine(StartFixedRoutine());
        }

        yield return null;
    }

    public void UpdateState()
    {
        _brain.AnimationController.SetWalkSpeed(_brain.NavMeshAgent.velocity.magnitude / _brain.NavMeshAgent.speed);
    }

    public IEnumerator OnExit()
    {
        yield return null;
    }

    public void CancelState()
    {

    }

    /// <summary>
    /// Called to go to a waypoint and launch the next.
    /// </summary>
    /// <param name="index"> Index of the waypoint to go to. </param>
    /// <returns></returns>
    private IEnumerator StartPatrol(int index)
    {
        bool reached = false;

        // Go to waypoint
        yield return _brain.SetDestination(_brain.Path[index].transform.position, success => reached = success);

        // If enemy has reached waypoint then continue
        if (!reached) CancelCoroutine(_patrolRoutine);

        // Check if the waypoint is a waypoint where the enemy can look around
        if (_brain.Path[index].IsLookAroundWaypoint)
        {
            yield return LookAround();
        }

        switch (_brain.PatrolType)
        {
            case PatrolType.LoopPatrol:
                if (index + _patrolDirection > _brain.Path.Count - 1)
                {
                    index = 0;
                    _patrolRoutine = _brain.StartCoroutine(StartPatrol(index));
                }
                else
                {
                    index += _patrolDirection;
                    _patrolRoutine = _brain.StartCoroutine(StartPatrol(index));
                }
                break;

            case PatrolType.PingPongPatrol:
                if (index + _patrolDirection > _brain.Path.Count - 1)
                {
                    _patrolDirection = -1;
                    index += _patrolDirection;
                    _patrolRoutine = _brain.StartCoroutine(StartPatrol(index));
                }
                else if (index + _patrolDirection < 0)
                {
                    _patrolDirection = 1;
                    index += _patrolDirection;
                    _patrolRoutine = _brain.StartCoroutine(StartPatrol(index));
                }
                else
                {
                    index += _patrolDirection;
                    _patrolRoutine = _brain.StartCoroutine(StartPatrol(index));
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
        bool reached = false;

        // Go to waypoint
        yield return _brain.SetDestination(_brain.Path[0].transform.position, success => reached = success);

        // If enemy has reached waypoint then continue
        if (!reached) CancelCoroutine(_patrolRoutine);

        yield return LookAround();

        _patrolRoutine = _brain.StartCoroutine(FixedRoutine());
    }

    /// <summary>
    /// Called to go to the fixed point and launch the fixed routine.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FixedRoutine()
    {
        int waitingTime = UnityEngine.Random.Range(_brain.FixedWaypointDuration.Min, _brain.FixedWaypointDuration.Max);

        yield return new WaitForSeconds(waitingTime);

        yield return LookAround();

        _patrolRoutine = _brain.StartCoroutine(FixedRoutine());
    }

    /// <summary>
    /// Called to determine if the enemy has to look around him.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LookAround()
    {
        if (UnityEngine.Random.Range(1, 100) > _brain.LookAroundProbability)
            yield break;

        _brain.AnimationController.PlayLookAroundAnim();

        bool eventFired = false;

        _onLookAroundFinished = () => eventFired = true;

        _brain.AnimationController.OnFinishToLookAround += _onLookAroundFinished;

        yield return new WaitUntil(() => eventFired);

        _brain.AnimationController.StopLookAroundAnim();

        CleanupLookAround();
    }

    /// <summary>
    /// Called to clean events which manages the look around animation.
    /// </summary>
    private void CleanupLookAround()
    {
        if (_onLookAroundFinished != null)
        {
            _brain.AnimationController.OnFinishToLookAround -= _onLookAroundFinished;
            _onLookAroundFinished = null;
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
            coroutine = null;
        }

        CleanupLookAround();
    }
}
