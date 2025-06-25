using System;
using System.Collections;
using UnityEngine;

public class HiddenState : IPlayerState
{
    /// <summary>
    /// A value indicating whether the player is transitioning or not.
    /// </summary>
    public bool IsTransitioning;

    /// <summary>
    /// Events to indicate when the player start and stop hide.
    /// </summary>
    public event Action OnHiddenStart, OnHiddenStop;

    /// <summary>
    /// The place to hide.
    /// </summary>
    private HiddenPlace _placeToHide;

    /// <summary>
    /// Manager of all states.
    /// </summary>
    private PlayerStateManager _stateManager;

    public IEnumerator OnEnter(PlayerStateManager stateManager)
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

    public void UpdateState()
    {
        Zoom();
    }

    public IEnumerator OnExit()
    {
        _stateManager.InputManager.OnLookWithMouse -= LookWithMouse;
        _stateManager.InputManager.OnLookWithGamepad -= LookWithGamepad;
        _stateManager.InputManager.OnZoomWithMouse -= CalculateZoomValueWithMouse;
        _stateManager.InputManager.OnZoomWithGamepad -= CalculateZoomValueWithGamepad;

        _stateManager.IsHidden = false;

        yield return null;
    }

    public void CancelState()
    {
        _stateManager.StopAllCoroutines();
        _stateManager.NavMeshController.CancelAll();

        _stateManager.InputManager.OnLookWithMouse -= LookWithMouse;
        _stateManager.InputManager.OnLookWithGamepad -= LookWithGamepad;
        _stateManager.InputManager.OnZoomWithMouse -= CalculateZoomValueWithMouse;
        _stateManager.InputManager.OnZoomWithGamepad -= CalculateZoomValueWithGamepad;

        IsTransitioning = false;
        _stateManager.IsHidden = false;
    }

    /// <summary>
    /// Called to initialize a transition to the hidden place.
    /// </summary>
    private IEnumerator InitTransitionToHiddenPlace()
    {
        IsTransitioning = true;
        OnHiddenStart?.Invoke();

        // Definition of targets
        Vector3 targetPosition = _placeToHide.HidingPosition;
        Quaternion targetRotation = _placeToHide.HidingRotation;

        yield return _stateManager.StartCoroutine(_stateManager.NavMeshController.TransitionTo(targetPosition, targetRotation,
            _stateManager.HideTransitionInSpeed, _stateManager.HideTransitionInAcceleration, _stateManager.HideTransitionInRotationSpeed,
            false, true, success => { if (!success) _stateManager.StartCoroutine(_stateManager.ResetCurrentState()); }));

        IsTransitioning = false;
        OnHiddenStop?.Invoke();

        _stateManager.AnimationController.PlayAnimationWithName(_placeToHide.PlayerAnimation);
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
