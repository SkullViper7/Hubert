using System;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public event Action MustShoot, HasShot, MustHit, HasHit;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetWalkSpeed(float speed)
    {
        _animator.SetFloat("Speed", speed);
    }

    public void StartCrawl()
    {
        _animator.SetBool("IsCrawling", true);
    }

    public void StopCrawl()
    {
        _animator.SetBool("IsCrawling", false);
    }

    public void StartStick()
    {
        _animator.SetBool("IsSticked", true);
    }

    public void StopStick()
    {
        _animator.SetBool("IsSticked", false);
    }

    public void StartAim()
    {
        _animator.SetBool("IsAiming", true);
    }

    public void StopAim()
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
        StopAim();
    }

    public void PlayHitAnim()
    {
        _animator.SetTrigger("Hit");
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

    public void ResetAnimation()
    {
        _animator.SetTrigger("Default");
    }
}