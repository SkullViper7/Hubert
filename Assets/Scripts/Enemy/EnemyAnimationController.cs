using System;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    /// <summary>
    /// Animator component of the player
    /// </summary>
    protected Animator _animator;

    /// <summary>
    /// The previous state when you enter il look around state.
    /// </summary>
    protected string _previousState;

    public event Action OnFinishToLookAround;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetWalkSpeed(float speed)
    {
        _animator.SetFloat("Speed", speed);
    }

    public virtual void PlayLookAroundAnim()
    {
        _animator.SetTrigger("LookAround");
    }

    public void HasFinishedToLookAround()
    {
        OnFinishToLookAround?.Invoke();
    }

    public void ReturnToPreviousState()
    {
        _animator.SetTrigger(_previousState);
    }
}
