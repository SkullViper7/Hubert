using System.Collections;
using UnityEngine;

public class HiddenState : IState
{
    /// <summary>
    /// A value indicating whether the player is transitioning or not.
    /// </summary>
    public bool IsTransitioning;

    /// <summary>
    /// The place to hide.
    /// </summary>
    private HiddenPlace _placeToHide;

    /// <summary>
    /// Manager of all states.
    /// </summary>
    private StateManager _stateManager;

    public IEnumerator OnEnter(StateManager stateManager)
    {
        _stateManager = stateManager;

        _stateManager.IsHidden = true;

        _placeToHide = _stateManager.PlaceToHide;

        _stateManager.InputManager.OnLookWithMouse += LookWithMouse;
        _stateManager.InputManager.OnLookWithGamepad += LookWithGamepad;
        _stateManager.InputManager.OnZoomWithMouse += CalculateZoomValueWithMouse;
        _stateManager.InputManager.OnZoomWithGamepad += CalculateZoomValueWithGamepad;

        yield return _stateManager.StartCoroutine(InitTransitionToHiddenPlace());

        yield return null;
    }

    public void UpdateState(StateManager stateManager)
    {
        Zoom();
    }

    public IEnumerator OnExit(StateManager stateManager)
    {
        _stateManager.InputManager.OnLookWithMouse -= LookWithMouse;
        _stateManager.InputManager.OnLookWithGamepad -= LookWithGamepad;
        _stateManager.InputManager.OnZoomWithMouse -= CalculateZoomValueWithMouse;
        _stateManager.InputManager.OnZoomWithGamepad -= CalculateZoomValueWithGamepad;

        yield return _stateManager.StartCoroutine(InitTransitionToExitHiddenPlace());

        _stateManager.IsHidden = false;
    }

    /// <summary>
    /// Called to initialize a transition to the hidden place.
    /// </summary>
    private IEnumerator InitTransitionToHiddenPlace()
    {
        IsTransitioning = true;

        // Definition of targets
        Vector3 targetPosition = _placeToHide.HidingPosition;
        Quaternion targetRotation = _placeToHide.HidingRotation;
        float speed = _stateManager.WalkSpeed;

        yield return _stateManager.StartCoroutine(_stateManager.NavMeshController.TransitionTo(targetPosition, targetRotation, speed, false));

        IsTransitioning = false;

        _stateManager.AnimationController.PlayAnimationWithName(_placeToHide.PlayerAnimation);
    }

    /// <summary>
    /// Called to initialize a transition to exit the hidden place.
    /// </summary>
    private IEnumerator InitTransitionToExitHiddenPlace()
    {
        IsTransitioning = true;

        // Definition of targets
        Vector3 targetPosition = _placeToHide.ExitPosition;
        Quaternion targetRotation = _placeToHide.ExitRotation;
        float speed = _stateManager.WalkSpeed;

        yield return _stateManager.StartCoroutine(_stateManager.NavMeshController.TransitionTo(targetPosition, targetRotation, speed, true));

        IsTransitioning = false;
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
