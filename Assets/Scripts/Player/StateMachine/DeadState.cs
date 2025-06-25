using System.Collections;
using System.Linq;
using UnityEngine;

public class DeadState : IPlayerState
{
    /// <summary>
    /// Manager of all states.
    /// </summary>
    private PlayerStateManager _stateManager;

    public IEnumerator OnEnter(PlayerStateManager stateManager)
    {
        _stateManager = stateManager;

        _stateManager.AnimationController.PlayDeathAnim();

        if (_stateManager.PlayerMaterials.Count > 1)
        {
            _stateManager.PlayerMaterials.FirstOrDefault(m => m.name.Contains("RedTrunk")).SetFloat("_Height", 0f);
            _stateManager.PlayerMaterials.Remove(_stateManager.RedTrunkMaterial);
            _stateManager.PlayerRenderer.materials = _stateManager.PlayerMaterials.ToArray();
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
