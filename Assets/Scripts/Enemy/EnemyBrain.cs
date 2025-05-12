using System.Collections;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    /// <summary>
    /// The current state of the enemy.
    /// </summary>
    protected IEnemyState _currentState;

    /// <summary>
    /// Called to execute the current state behaviour.
    /// </summary>
    protected void Update()
    {
        _currentState?.UpdateState();
    }

    /// <summary>
    /// Called to switch to a new state.
    /// </summary>
    /// <param name="newState"> The new state to switch. </param>
    protected IEnumerator ChangeState(IEnemyState newState)
    {
        if (_currentState != null)
            yield return StartCoroutine(_currentState.OnExit());

        _currentState = newState;

        if (_currentState != null)
            yield return StartCoroutine(_currentState.OnEnter(this));
    }

    /// <summary>
    /// Called to cancel any state and return to default state.
    /// </summary>
    public void CancelCurrentState()
    {
        _currentState.CancelState();
    }
}
