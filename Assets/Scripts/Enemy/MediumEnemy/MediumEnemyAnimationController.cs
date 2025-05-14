using System;
using UnityEngine;

public class MediumEnemyAnimationController : MonoBehaviour
{
    /// <summary>
    /// Animator component of the player
    /// </summary>
    private Animator _animator;

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
        _animator.SetBool("IsLookingAround", true);
    }

    public void StopLookAroundAnim()
    {
        _animator.SetBool("IsLookingAround", false);
    }

    public void HasFinishedToLookAround()
    {
        OnFinishToLookAround?.Invoke();
    }

    public void PlayResearchAnim()
    {
        _animator.SetBool("IsResearching", true);
    }

    public void StopResearchAnim()
    {
        _animator.SetBool("IsResearching", false);
    }
}
