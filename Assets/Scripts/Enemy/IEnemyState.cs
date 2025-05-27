using System.Collections;

public interface IEnemyState
{
    /// <summary>
    /// Called at the enter of a state.
    /// </summary>
    /// <param name="enemyBrain"> Brain which manages states of the enemy. </param>
    /// <param name="enemyStateEnterType"> A value to know of the enemy has directly a goal when he enter a state. </param>
    public IEnumerator OnEnter(EnemyBrain enemyBrain, EnemyStateEnterType enemyStateEnterType);

    /// <summary>
    /// Called continuously during the state.
    /// </summary>
    public void UpdateState();

    /// <summary>
    /// Called at the exit of a state.
    /// </summary>
    public IEnumerator OnExit();
}
