using System.Collections;
using UnityEngine;

public class StickedState : IState
{
    /// <summary>
    /// A value indicating whether the player is transitioning or not.
    /// </summary>
    public bool IsTransitioning;

    /// <summary>
    /// Target velocity of the velocity.
    /// </summary>
    private Vector3 _targetVelocity;

    /// <summary>
    /// Current velocity of the player.
    /// </summary>
    private Vector3 _currentVelocity;

    /// <summary>
    /// Current vertical velocity of the player.
    /// </summary>
    private Vector3 _gravityVelocity;

    /// <summary>
    /// Manager of all states.
    /// </summary>
    private StateManager _stateManager;

    public void OnEnter(StateManager stateManager)
    {
        _stateManager = stateManager;

        _stateManager.IsSticking = true;

        _stateManager.InputManager.OnLookWithMouse += LookWithMouse;
        _stateManager.InputManager.OnLookWithGamepad += LookWithGamepad;
        _stateManager.InputManager.OnZoomWithMouse += CalculateZoomValueWithMouse;
        _stateManager.InputManager.OnZoomWithGamepad += CalculateZoomValueWithGamepad;

        _stateManager.AnimationController.StartStick();

        // Definition of targets
        Vector3 targetPosition = _stateManager.StickedPosition;
        Quaternion targetRotation = Quaternion.LookRotation(_stateManager.StickedNormal);

        // Launch a coroutine to manage the transition
        _stateManager.StartCoroutine(TransitionToWall(targetPosition, targetRotation, false));
    }

    public void UpdateState(StateManager stateManager)
    {
        Move();
        CorrectPosition();
        Zoom();
    }

    public void OnExit(StateManager stateManager)
    {
        _stateManager.InputManager.OnMove -= CalculateVelocity;
        _stateManager.InputManager.OnLookWithMouse -= LookWithMouse;
        _stateManager.InputManager.OnLookWithGamepad -= LookWithGamepad;
        _stateManager.InputManager.OnZoomWithMouse -= CalculateZoomValueWithMouse;
        _stateManager.InputManager.OnZoomWithGamepad -= CalculateZoomValueWithGamepad;

        _targetVelocity = Vector3.zero;
        _currentVelocity = Vector3.zero;
        _gravityVelocity = Vector3.zero;

        _stateManager.AnimationController.StopStick();

        // Definition of targets
        Vector3 targetPosition = _stateManager.transform.position + _stateManager.transform.forward * 1f;
        Quaternion targetRotation = _stateManager.transform.rotation;

        // Launch a coroutine to manage the transition
        _stateManager.StartCoroutine(TransitionToWall(targetPosition, targetRotation, true));
    }

    private IEnumerator TransitionToWall(Vector3 targetPosition, Quaternion targetRotation, bool isExitTransition)
    {
        IsTransitioning = true;
        float _transitionProgress = 0f;

        // Saves the current position and rotation
        Vector3 startPosition = _stateManager.transform.position;
        Quaternion startRotation = _stateManager.transform.rotation;

        while (_transitionProgress < 1f)
        {
            _transitionProgress += Time.deltaTime / _stateManager.TransitionTime;

            // Progressive movement with CharacterController
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, _transitionProgress);
            _stateManager.CharacterController.Move(newPosition - _stateManager.transform.position);

            // Smooth rotation
            _stateManager.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, _transitionProgress);

            yield return null;
        }

        // End of transition
        IsTransitioning = false;

        if (!isExitTransition)
        {
            // We reactivate the movement inputs
            _stateManager.InputManager.OnMove += CalculateVelocity;
        }
        else
        {
            _stateManager.IsSticking = false;
        }
    }

    /// <summary>
    /// Called to calculate the velocity of the player.
    /// </summary>
    /// <param name="direction"> Direction of the movement. </param>
    private void CalculateVelocity(Vector2 direction)
    {
        if (IsTransitioning) return;

        // Get the camera direction relative to the player
        Vector3 cameraDirection = (_stateManager.transform.position - _stateManager.Camera.transform.position).normalized;

        // Cancel the vertical axis to avoid height movements
        cameraDirection.y = 0;
        cameraDirection.Normalize();

        // Calculate a perpendicular axis (camera line)
        Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraDirection).normalized;

        // Determine the direction of movement BEFORE projection
        Vector3 movementDirection = (cameraDirection * direction.y + cameraRight * direction.x).normalized;

        // Project this direction onto the plane of the wall to stay stuck
        Vector3 projectedDirection = Vector3.ProjectOnPlane(movementDirection, _stateManager.StickedNormal).normalized;

        // Calculate the alignment between the input and the possible direction
        float alignmentFactor = Vector3.Dot(movementDirection, projectedDirection);
        alignmentFactor = Mathf.Max(0, alignmentFactor);

        // Apply velocity with a weighting factor
        _targetVelocity = projectedDirection * _stateManager.StickSpeed * alignmentFactor;
    }

    /// <summary>
    /// Called to move the player and rotate him.
    /// </summary>
    private void Move()
    {
        if (IsTransitioning) return;

        // Calculate velocity with acceleration and deceleration
        _currentVelocity = Vector3.Lerp(_currentVelocity, _targetVelocity, _stateManager.MoveSmoothness * Time.deltaTime);

        // Avoid residual speed that would prevent a complete stop
        if (_targetVelocity.sqrMagnitude == 0 && _currentVelocity.sqrMagnitude < 0.01f)
        {
            _currentVelocity = Vector3.zero;
        }

        // Gravity management
        if (_stateManager.CharacterController.isGrounded)
        {
            _gravityVelocity.y = -_stateManager.GravityForce * Time.deltaTime;
        }
        else
        {
            _gravityVelocity.y -= _stateManager.GravityForce * Time.deltaTime;
        }

        // Application of movement + gravity
        _stateManager.CharacterController.Move((_currentVelocity + _gravityVelocity) * Time.deltaTime);
    }

    /// <summary>
    /// Called to correct the position of the player when he is on the wall.
    /// </summary>
    private void CorrectPosition()
    {
        if (IsTransitioning) return;

        _stateManager.transform.position = Utilities.GetCorrectPosition(_stateManager.transform.position, _stateManager.StickedNormal,
                                                                        (BoxCollider)_stateManager.StickedWall, _stateManager.CharacterController);
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
