using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshController : MonoBehaviour
{
    /// <summary>
    /// Animation controller of the player.
    /// </summary>
    [SerializeField]
    private PlayerAnimationController _animationController;

    /// <summary>
    /// The margin add to the transtion duration a the end of which the transition is canceled.
    /// </summary>
    [SerializeField]
    private float _cancelDelayMargin;

    /// <summary>
    /// The nav mesh agent of the player. 
    /// </summary>
    private NavMeshAgent _navMeshAgent;

    /// <summary>
    /// Elapsed time from the beginning of the transition.
    /// </summary>
    private float _elapsedTime;

    /// <summary>
    /// Duration a the end of which the transition is canceled.
    /// </summary>
    private float _timeLimit = 0f;

    /// <summary>
    /// Speed of the rotation.
    /// </summary>
    private float _rotationSpeed;

    /// <summary>
    /// A value indicating if the transition has succed.
    /// </summary>
    private bool _transitionSuccess;

    /// <summary>
    /// A value indicating that the transition has been canceled.
    /// </summary>
    private bool _transitionCancel;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void CancelAll()
    {
        _transitionCancel = true;

        if (!_navMeshAgent.enabled)
            return;

        _navMeshAgent.ResetPath();
        _navMeshAgent.velocity = Vector3.zero;
    }

    /// <summary>
    /// Called to transition to a position and a rotation.
    /// </summary>
    /// <param name="destination"> Destination to reach. </param>
    /// <param name="targetRotation"> Rotation to reach. </param>
    /// <param name="speed"> Speed of the movement. </param>
    /// <param name="isBlended"> A value indicating if movement and rotation are blended or not. </param>
    /// <returns></returns>
    public IEnumerator TransitionTo(Vector3 destination, Quaternion targetRotation, float speed, float acceleration, float rotationSpeed, bool isBlended, bool mustResetAnim, Action<bool> onTransitionComplete)
    {
        _navMeshAgent.enabled = true;
        _navMeshAgent.speed = speed;
        _navMeshAgent.acceleration = acceleration;
        _rotationSpeed = rotationSpeed;

        _navMeshAgent.SetDestination(destination);
        _elapsedTime = 0f;
        _transitionSuccess = true;
        _transitionCancel = false;

        yield return StartCoroutine(WaitForPathAndEstimateTime());

        if (mustResetAnim)
        {
            _animationController.ResetAnimation();
        }

        if (isBlended)
        {
            // Launch parallel movement and rotation
            _navMeshAgent.updateRotation = false;
            yield return StartCoroutine(RunParallel(WaitUntilArrived(), RotateToTarget(targetRotation)));
        }
        else
        {
            // Run sequentially
            _navMeshAgent.updateRotation = true;
            yield return StartCoroutine(WaitUntilArrived());
            yield return StartCoroutine(RotateToTarget(targetRotation));
        }

        _navMeshAgent.ResetPath();
        _navMeshAgent.velocity = Vector3.zero;
        _navMeshAgent.enabled = false;

        if (!_transitionCancel)
        {
            onTransitionComplete?.Invoke(_transitionSuccess);
        }
        else
        {
            onTransitionComplete?.Invoke(true);
        }
    }

    private IEnumerator WaitForPathAndEstimateTime()
    {
        // On attend que le chemin soit calculé
        yield return new WaitUntil(() => !_navMeshAgent.pathPending);

        // Calcule la longueur réelle du chemin
        float pathLength = GetPathLength(_navMeshAgent.path);

        float estimatedTime = pathLength / _navMeshAgent.speed;
        _timeLimit = estimatedTime + _cancelDelayMargin;
    }

    private float GetPathLength(NavMeshPath path)
    {
        float length = 0f;
        if (path.corners.Length < 2) return length;

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return length;
    }

    /// <summary>
    /// Runs two coroutines in parallel and waits for both to complete.
    /// </summary>
    private IEnumerator RunParallel(IEnumerator coroutine1, IEnumerator coroutine2)
    {
        bool c1Finished = false, c2Finished = false;

        StartCoroutine(RunAndFlag(coroutine1, () => c1Finished = true));
        StartCoroutine(RunAndFlag(coroutine2, () => c2Finished = true));

        // Wait until both are finished
        while (!c1Finished || !c2Finished)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Runs a coroutine and triggers a callback at the end.
    /// </summary>
    private IEnumerator RunAndFlag(IEnumerator coroutine, Action onComplete)
    {
        yield return StartCoroutine(coroutine);
        onComplete?.Invoke();
    }

    /// <summary>
    /// Called to wait until the agent reachs his target.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitUntilArrived()
    {
        while (_transitionSuccess && !_navMeshAgent.pathPending && _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance && !_transitionCancel)
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime > _timeLimit)
            {
                FailTransition();
            }
            _animationController.SetWalkSpeed(_navMeshAgent.velocity.magnitude / _navMeshAgent.speed);
            yield return null;
        }
    }

    /// <summary>
    /// Called to rotate the agent to a targeted rotation.
    /// </summary>
    /// <param name="targetRotation"> Targeted rotation to reach. </param>
    /// <returns></returns>
    private IEnumerator RotateToTarget(Quaternion targetRotation)
    {
        Quaternion startRotation = transform.rotation;
        float angle = Quaternion.Angle(startRotation, targetRotation);
        float duration = angle / _rotationSpeed;
        float elapsed = 0f;

        while (_transitionSuccess && elapsed < duration && !_transitionCancel)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            _animationController.SetWalkSpeed(_navMeshAgent.velocity.magnitude / _navMeshAgent.speed);
            yield return null;
        }

        if (_transitionSuccess && !_transitionCancel)
        {
            transform.rotation = targetRotation;
        }
    }

    /// <summary>
    /// Called to fail a transition.
    /// </summary>
    private void FailTransition()
    {
        _transitionSuccess = false;
    }
}
