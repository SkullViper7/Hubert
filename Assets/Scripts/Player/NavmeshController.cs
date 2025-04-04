using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshController : MonoBehaviour
{
    /// <summary>
    /// Animation controller of the player.
    /// </summary>
    [SerializeField]
    private AnimationController _animationController;

    /// <summary>
    /// The nav mesh agent of the player. 
    /// </summary>
    private NavMeshAgent _navMeshAgent;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Called to transition to a position and a rotation.
    /// </summary>
    /// <param name="destination"> Destination to reach. </param>
    /// <param name="targetRotation"> Rotation to reach. </param>
    /// <param name="speed"> Speed of the movement. </param>
    /// <param name="isBlended"> A value indicating if movement and rotation are blended or not. </param>
    /// <returns></returns>
    public IEnumerator TransitionTo(Vector3 destination, Quaternion targetRotation, float speed, bool isBlended)
    {
        _navMeshAgent.enabled = true;
        _navMeshAgent.speed = speed;
        _navMeshAgent.SetDestination(destination);

        _animationController.ResetAnimation();

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
    private IEnumerator RunAndFlag(IEnumerator coroutine, System.Action onComplete)
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
        while (_navMeshAgent.pathPending || _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance)
        {
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
        float duration = 0.2f;
        float elapsed = 0f;
        Quaternion startRotation = transform.rotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            _animationController.SetWalkSpeed(_navMeshAgent.velocity.magnitude / _navMeshAgent.speed);
            yield return null;
        }

        transform.rotation = targetRotation;
    }
}
