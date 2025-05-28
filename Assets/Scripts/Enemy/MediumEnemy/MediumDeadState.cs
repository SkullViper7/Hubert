using System.Collections;
using UnityEngine;

public class MediumDeadState : IEnemyState
{
    /// <summary>
    /// Brain of the enemy.
    /// </summary>
    private MediumEnemyBrain _brain;

    public IEnumerator OnEnter(EnemyBrain enemyBrain, EnemyStateEnterType enemyStateEnterType)
    {
        // Get components
        _brain = (MediumEnemyBrain)enemyBrain;
        _brain.NavMeshAgent.enabled = false;
        _brain.gameObject.layer = LayerMask.NameToLayer("Default");

        if (enemyStateEnterType == EnemyStateEnterType.IsShot)
        {
            _brain.GetComponent<CapsuleCollider>().isTrigger = true;

            _brain.MediumAnimationController.PlayShotAnim();
        }
        else if (enemyStateEnterType == EnemyStateEnterType.IsHit)
        {
            _brain.MediumAnimationController.PlayStartHitAnim();
            _brain.OnHit += () =>
            {
                _brain.MediumAnimationController.PlayHitAnim();
                _brain.GetComponent<CapsuleCollider>().isTrigger = true;
            };
        }

        yield return null;
    }

    public void UpdateState()
    {

    }

    public IEnumerator OnExit()
    {
        yield return null;
    }

    public void CancelState()
    {

    }
}
