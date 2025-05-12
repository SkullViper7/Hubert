using System.Collections;

public class PatrolState : IEnemyState
{
    public IEnumerator OnEnter(EnemyBrain enemyBrain)
    {
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
