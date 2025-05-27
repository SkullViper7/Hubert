public class MediumEnemyAnimationController : EnemyAnimationController
{
    public void PlayPatrolAnim()
    {
        _animator.SetTrigger("Patrol");
    }

    public void PlayResearchAnim()
    {
        _animator.SetTrigger("Research");
    }

    public void PlayStartHitAnim()
    {
        _animator.SetTrigger("StartHit");
    }

    public void PlayHitAnim()
    {
        _animator.SetTrigger("Hit");
    }

    public void PlayShotAnim()
    {
        _animator.SetTrigger("Shot");
    }
}
