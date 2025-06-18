using System;

public class MediumEnemyAnimationController : EnemyAnimationController
{
    /// <summary>
    /// Events triggered at the end of some animation.
    /// </summary>
    public event Action OnFinishGunAction, OnMustShoot, OnFinishToShoot;

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

    public void PlayGunActionAnim(string trigger)
    {
        _animator.SetTrigger(trigger);
        _animator.Update(0);
    }

    public void HasFinishedGunAction()
    {
        OnFinishGunAction?.Invoke();
    }

    public void PlayShootAnim()
    {
        _animator.SetTrigger("Shoot");
        _animator.Update(0);
    }

    public void MustShoot()
    {
        OnMustShoot?.Invoke();
    }

    public void HasFinishedToShoot()
    {
        OnFinishToShoot?.Invoke();
    }
}
