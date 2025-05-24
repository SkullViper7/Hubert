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

    public void PlayLookAroundAnim(string trigger)
    {
        _animator.SetTrigger(trigger);
        _animator.Update(0);
    }

    public void HasFinishedToLookAround()
    {
        OnFinishToLookAround?.Invoke();
    }
}
