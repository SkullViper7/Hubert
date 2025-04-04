using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : MonoBehaviour
{
    [SerializeField] List<GameObject> _waypoints;
    Animator _animator;
    [SerializeField] AnimationClip _lookAroundAnimation;

    NavMeshAgent _navMeshAgent;

    public Coroutine PatrolCoroutine;

    int _waypointIndex = 0;
    int _increaseOperator = 1;

    [SerializeField] int _waitingTime = 5;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        PatrolCoroutine = StartCoroutine(StartPatrol(0));

        EnemyVision.OnPlayerDetected += StopPatrol;
        EnemyVision.OnPlayerLost += RestartPatrol;
    }

    IEnumerator StartPatrol(float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);

        _navMeshAgent.isStopped = false;

        for (int i = 0; i < _waypoints.Count; i++)
        {
            Vector3 targetPosition = _waypoints[_waypointIndex].transform.position;
            _navMeshAgent.SetDestination(targetPosition);

            _navMeshAgent.updateRotation = true;

            while (_navMeshAgent.pathPending || _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance)
            {
                yield return null;
            }

            int random = Random.Range(0, 100);
            if (random < 25)
            {
                _animator.Play(_lookAroundAnimation.name);
                yield return new WaitForSeconds(_lookAroundAnimation.length);
            }

            _waypointIndex = (_waypointIndex + _increaseOperator) % _waypoints.Count;

            if (_waypointIndex == _waypoints.Count - 1)
            {
                _waypointIndex = 0;
            }
        }

        StartCoroutine(StartPatrol(0));
    }

    void StopPatrol()
    {
        StopCoroutine(PatrolCoroutine);
    }

    void RestartPatrol()
    {
        StartCoroutine(SearchPlayer());
    }

    IEnumerator SearchPlayer()
    {
        while (_navMeshAgent.pathPending || _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance)
        {
            yield return null;
        }

        _animator.Play(_lookAroundAnimation.name);
        PatrolCoroutine = StartCoroutine(StartPatrol(_lookAroundAnimation.length));
    }
}
