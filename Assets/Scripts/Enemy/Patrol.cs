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

    [SerializeField] EnemyVision _enemyVision;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        ClearPatrol();
        PatrolCoroutine = StartCoroutine(StartPatrol(0));

        _enemyVision.OnPlayerDetected += ClearPatrol;
        _enemyVision.OnPlayerLostPos += RestartPatrol;
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

            if (_waypointIndex == _waypoints.Count)
            {
                _waypointIndex = 0;
            }
        }

        ClearPatrol();
        PatrolCoroutine = StartCoroutine(StartPatrol(0));
    }

    void RestartPatrol(Vector3 playerLastPos)
    {
        ClearPatrol();
        PatrolCoroutine = StartCoroutine(SearchPlayer(playerLastPos));
    }

    IEnumerator SearchPlayer(Vector3 playerLastPos)
    {
        _navMeshAgent.SetDestination(playerLastPos);

        while (_navMeshAgent.pathPending || _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance)
        {
            yield return null;
        }

        _animator.Play(_lookAroundAnimation.name);
        ClearPatrol();
        PatrolCoroutine = StartCoroutine(StartPatrol(_lookAroundAnimation.length));
    }

    private void ClearPatrol()
    {
        if (PatrolCoroutine == null) return;
        StopCoroutine(PatrolCoroutine);
        PatrolCoroutine = null;
    }

    private void OnDisable()
    {
        ClearPatrol();
    }
}
