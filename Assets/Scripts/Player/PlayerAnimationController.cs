using System;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public event Action MustShoot, HasShot, MustHit, HasHit, OnDead;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void ResetAnimation()
    {
        _animator.SetTrigger("Default");
    }

    public void SetWalkSpeed(float speed)
    {
        _animator.SetFloat("Speed", speed);
    }

    public void PlayCrawlAnim()
    {
        _animator.SetTrigger("IsCrawling");
    }

    public void PlayStickAnim()
    {
        _animator.SetTrigger("IsSticked");
    }

    public void PlayHoldBreathAnim()
    {
        _animator.SetBool("IsHoldingBreath", true);
    }

    public void StopHoldBreathAnim()
    {
        _animator.SetBool("IsHoldingBreath", false);
    }

    public void PlayOutOfBreathAnim()
    {
        _animator.SetBool("OutOfBreath", true);
    }

    public void StopOutOfBreathAnim()
    {
        _animator.SetBool("OutOfBreath", false);
    }

    public void PlayAimAnim()
    {
        _animator.SetTrigger("IsAiming");
    }

    public void PlayShootAnim()
    {
        _animator.SetTrigger("Shoot");
    }

    private void Shoot()
    {
        MustShoot?.Invoke();
    }

    private void EndOfTheShoot()
    {
        HasShot?.Invoke();
    }

    public void PlayHitAnim()
    {
        _animator.SetTrigger("IsHiting");
    }

    private void Hit()
    {
        MustHit?.Invoke();
    }

    private void EndOfTheHit()
    {
        HasHit?.Invoke();
    }

    public void PlayAnimationWithName(string name)
    {
        _animator?.SetTrigger(name);
    }

    public void PlayDeathAnim()
    {
        _animator.SetTrigger("Death");
    }

    private void IsDead()
    {
        OnDead?.Invoke();
    }
}