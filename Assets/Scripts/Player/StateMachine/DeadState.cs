using System.Collections;
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

        _stateManager.InputManager.OnLookWithMouse += LookWithMouse;
        _stateManager.InputManager.OnLookWithGamepad += LookWithGamepad;
        _stateManager.InputManager.OnZoomWithMouse += CalculateZoomValueWithMouse;
        _stateManager.InputManager.OnZoomWithGamepad += CalculateZoomValueWithGamepad;

        _stateManager.AnimationController.PlayDeathAnim();

        yield return null;
    }

    public void UpdateState()
    {
        Zoom();
    }

    public IEnumerator OnExit()
    {
        yield return null;
    }

    public void CancelState()
    {
        
    }

    /// <summary>
    /// Called to look around the player with the mouse.
    /// </summary>
    /// <param name="direction"> Direction of the look. </param>
    private void LookWithMouse(Vector2 direction)
    {
        if (_stateManager.Camera == null) return;

        // Horizontal rotation
        _stateManager.Camera.m_XAxis.Value += direction.x * _stateManager.MouseSensitivityX;
    }

    /// <summary>
    /// Called to look around the player with the gamepad.
    /// </summary>
    /// <param name="direction"> Direction of the look. </param>
    private void LookWithGamepad(Vector2 direction)
    {
        if (_stateManager.Camera == null) return;

        // Horizontal rotation
        _stateManager.Camera.m_XAxis.Value += direction.x * _stateManager.GamepadSensitivityX * Time.deltaTime;
    }

    /// <summary>
    /// Called to calculat the zoom value with the scroll wheel.
    /// </summary>
    /// <param name="value"> Value of the zoom. </param>
    private void CalculateZoomValueWithMouse(float value)
    {
        if (_stateManager.Camera == null) return;

        // Set a target instead of directly applying the value
        _stateManager.TargetYAxis = Mathf.Clamp01(_stateManager.TargetYAxis + value * _stateManager.MouseSensitivityY);
    }

    /// <summary>
    /// Called to calculat the zoom value with the gamepad.
    /// </summary>
    /// <param name="value"> Value of the zoom. </param>
    private void CalculateZoomValueWithGamepad(float value)
    {
        if (_stateManager.Camera == null) return;

        // Set a target instead of directly applying the value
        _stateManager.TargetYAxis = Mathf.Clamp01(_stateManager.TargetYAxis + value * _stateManager.GamepadSensitivityY);
    }

    /// <summary>
    /// Called to zoom on the player.
    /// </summary>
    private void Zoom()
    {
        if (_stateManager.Camera == null) return;

        // Lerp for a smooth transition
        _stateManager.Camera.m_YAxis.Value = Mathf.Lerp(_stateManager.Camera.m_YAxis.Value, _stateManager.TargetYAxis, _stateManager.ZoomSmoothness * Time.deltaTime);
    }
}
