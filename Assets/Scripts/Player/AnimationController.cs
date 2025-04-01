using System;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public event Action MustShoot, HasShot;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
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
    }
}
