using UnityEngine;

public class MediumEnemyAnimationController : EnemyAnimationController
{
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
}
