using System;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    /// <summary>
    /// Animator component of the player
    /// </summary>
    protected Animator _animator;

    public event Action OnFinishToLookAround;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetWalkSpeed(float speed)
    {
        _animator.SetFloat("Speed", speed);
    }

    public void PlayLookAroundAnim()
    {
        _animator.SetTrigger("LookAround");
    }

    public void HasFinishedToLookAround()
    {
        OnFinishToLookAround?.Invoke();
    }
}
