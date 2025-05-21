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
}
