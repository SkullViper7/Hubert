using UnityEngine;

public class MediumEnemyAnimationController : EnemyAnimationController
{
    public void PlayPatrolAnim()
    {
        _animator.SetTrigger("Patrol");
    }

    public override void PlayLookAroundAnim()
    {
        AnimatorStateInfo currentState = _animator.GetCurrentAnimatorStateInfo(0);
        _previousState = currentState.IsName("Patrol") ? "Patrol" :
                         currentState.IsName("LookAround") ? "LookAround" :
                         currentState.IsName("Research") ? "Research" :
                         "DefaultState";

        base.PlayLookAroundAnim();
    }

    public void PlayResearchAnim()
    {
        _animator.SetTrigger("Research");
    }
}
