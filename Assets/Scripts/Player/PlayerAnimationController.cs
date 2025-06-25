using System;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public event Action OnLeftStep, OnRightStep, MustShoot, OnShot, MustHit, OnHit, OnStartElectrified, OnEndElectrified, OnFall, OnDead;

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

    private void LeftStep()
    {
        OnLeftStep?.Invoke();
    }

    private void RightStep()
    {
        OnRightStep?.Invoke();
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
        OnShot?.Invoke();
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
        OnHit?.Invoke();
    }

    public void PlayAnimationWithName(string name)
    {
        _animator?.SetTrigger(name);
    }

    public void PlayDeathAnim()
    {
        _animator.SetTrigger("Death");
    }

    private void StartElectrified()
    {
        OnStartElectrified?.Invoke();
    }

    private void EndElectrified()
    {
        OnEndElectrified?.Invoke();
    }

    private void Fall()
    {
        OnFall?.Invoke();
    }

    private void IsDead()
    {
        OnDead?.Invoke();
    }
}