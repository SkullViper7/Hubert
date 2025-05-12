using System;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public event Action MustShoot, HasShot, MustHit, HasHit;

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
        _animator.SetBool("IsCrawling", true);
    }

    public void StopCrawlAnim()
    {
        _animator.SetBool("IsCrawling", false);
    }

    public void PlayStickAnim()
    {
        _animator.SetBool("IsSticked", true);
    }

    public void StopStickAnim()
    {
        _animator.SetBool("IsSticked", false);
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
        _animator.SetBool("IsAiming", true);
    }

    public void StopAimAnim()
    {
        _animator.SetBool("IsAiming", false);
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
        StopAimAnim();
    }

    public void PlayHitAnim()
    {
        _animator.SetBool("IsHiting", true);
    }

    public void StopHitAnim()
    {
        _animator.SetBool("IsHiting", false);
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
}