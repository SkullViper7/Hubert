using System.Collections;

public interface IState
{
    /// <summary>
    /// Called at the enter of a state.
    /// </summary>
    /// <param name="stateManager"> Manager of all states. </param>
    public IEnumerator OnEnter(StateManager stateManager);

    /// <summary>
    /// Called continuously during the state.
    /// </summary>
    /// <param name="stateManager"> Manager of all states. </param>
    public void UpdateState(StateManager stateManager);

    /// <summary>
    /// Called at the exit of a state.
    /// </summary>
    /// <param name="stateManager"> Manager of all states. </param>
    public IEnumerator OnExit(StateManager stateManager);
}
