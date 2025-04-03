using System.Collections;
using UnityEngine;

public class StickedState : IState
{
    /// <summary>
    /// A value indicating whether the player is transitioning or not.
    /// </summary>
    public bool IsTransitioning;

    /// <summary>
    /// Target velocity of the player.
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

    public IEnumerator OnEnter(StateManager stateManager)
    {
        _stateManager = stateManager;

        _stateManager.IsSticking = true;

        _stateManager.InputManager.OnLookWithMouse += LookWithMouse;
        _stateManager.InputManager.OnLookWithGamepad += LookWithGamepad;
        _stateManager.InputManager.OnZoomWithMouse += CalculateZoomValueWithMouse;
        _stateManager.InputManager.OnZoomWithGamepad += CalculateZoomValueWithGamepad;

        yield return _stateManager.StartCoroutine(InitTransitionToWall());

        yield return null;
    }

    public void UpdateState(StateManager stateManager)
    {
        Move();
        CorrectPosition();
        Zoom();
    }

    public IEnumerator OnExit(StateManager stateManager)
    {
        _stateManager.InputManager.OnMove -= CalculateVelocity;
        _stateManager.InputManager.OnLookWithMouse -= LookWithMouse;
        _stateManager.InputManager.OnLookWithGamepad -= LookWithGamepad;
        _stateManager.InputManager.OnZoomWithMouse -= CalculateZoomValueWithMouse;
        _stateManager.InputManager.OnZoomWithGamepad -= CalculateZoomValueWithGamepad;

        _targetVelocity = Vector3.zero;
        _currentVelocity = Vector3.zero;
        _gravityVelocity = Vector3.zero;

        yield return _stateManager.StartCoroutine(InitTransitionToExitWall());

        _stateManager.IsSticking = false;
    }

    /// <summary>
    /// Called to initialize a transition to the wall.
    /// </summary>
    private IEnumerator InitTransitionToWall()
    {
        IsTransitioning = true;

        // Definition of targets
        Vector3 targetPosition = _stateManager.StickedPosition;
        Quaternion targetRotation = Quaternion.LookRotation(_stateManager.StickedNormal);
        float speed = _stateManager.WalkSpeed;

        yield return _stateManager.StartCoroutine(_stateManager.NavMeshController.TransitionTo(targetPosition, targetRotation, speed, true));

        IsTransitioning = false;

        _stateManager.AnimationController.StartStick();

        _stateManager.InputManager.OnMove += CalculateVelocity;
    }

    /// <summary>
    /// Called to initialize a transition to exit the wall.
    /// </summary>
    private IEnumerator InitTransitionToExitWall()
    {
        IsTransitioning = true;

        _stateManager.AnimationController.StopStick();

        // Definition of targets
        Vector3 targetPosition = _stateManager.transform.position + _stateManager.transform.forward * 1f;
        Quaternion targetRotation = _stateManager.transform.rotation;
        float speed = _stateManager.WalkSpeed;

        yield return _stateManager.StartCoroutine(_stateManager.NavMeshController.TransitionTo(targetPosition, targetRotation, speed, true));

        IsTransitioning = false;
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
