using System;

public class MediumEnemyAnimationController : EnemyAnimationController
{
    /// <summary>
    /// Event triggered at the end of some aniamtion.
    /// </summary>
    public event Action OnFinishToStartAim, OnFinishToStopAim, OnFinishToShoot;

    public void PlayPatrolAnim()
    {
        _animator.SetTrigger("Patrol");
        _animator.Update(0);
    }

    public void PlayResearchAnim()
    {
        _animator.SetTrigger("Research");
        _animator.Update(0);
    }

    public void PlayAlerteAnim()
    {
        _animator.SetTrigger("Alerte");
        _animator.Update(0);
    }

    public void PlayAimAnim()
    {
        _animator.SetTrigger("Aim");
        _animator.Update(0);
    }

    public void PlayStartHitAnim()
    {
        _animator.SetTrigger("StartHit");
        _animator.Update(0);
    }

    public void PlayHitAnim()
    {
        _animator.SetTrigger("Hit");
        _animator.Update(0);
    }

    public void PlayShotAnim()
    {
        _animator.SetTrigger("Shot");
        _animator.Update(0);
    }

    public void PlayStartAimAnim()
    {
        _animator.SetTrigger("StartAim");
        _animator.Update(0);
    }

    public void HasFinishedToStartAim()
    {
        OnFinishToStartAim?.Invoke();
    }

    public void PlayStopAimAnim()
    {
        _animator.SetTrigger("StopAim");
        _animator.Update(0);
    }

    public void HasFinishedToStopAim()
    {
        OnFinishToStopAim?.Invoke();
    }

    public void PlayShootAnim()
    {
        _animator.SetTrigger("Shoot");
        _animator.Update(0);
    }

    public void HasFinishedToShoot()
    {
        OnFinishToShoot?.Invoke();
    }
}
