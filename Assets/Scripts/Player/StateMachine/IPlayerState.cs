using System.Collections;

public interface IPlayerState
{
    /// <summary>
    /// Called at the enter of a state.
    /// </summary>
    /// <param name="stateManager"> Manager of all states. </param>
    public IEnumerator OnEnter(PlayerStateManager stateManager);

    /// <summary>
    /// Called continuously during the state.
    /// </summary>
    public void UpdateState();

    /// <summary>
    /// Called at the exit of a state.
    /// </summary>
    public IEnumerator OnExit();

    /// <summary>
    /// Called to cancel a state without any transition or wathever as the exit.
    /// </summary>
    /// <returns></returns>
    public void CancelState();
}
