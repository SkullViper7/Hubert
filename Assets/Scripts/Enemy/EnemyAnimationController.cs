using System;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    /// <summary>
    /// Animator component of the player.
    /// </summary>
    protected Animator _animator;

    /// <summary>
    /// Event triggered at the end of some aniamtion.
    /// </summary>
    public event Action OnFinishToLookAround, OnFinishAstonishment;

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

    public void PlayAstonishmentAnim(string trigger)
    {
        _animator.SetTrigger(trigger);
        _animator.Update(0);
    }

    public void HasFinishedAstonishment()
    {
        OnFinishAstonishment?.Invoke();
    }
}
