using System;
using System.Collections;
using UnityEngine;

public class StickedState : IState
{
    /// <summary>
    /// A value indicating whether the player is transitioning or not.
    /// </summary>
    public bool IsTransitioning;

    /// <summary>
    /// A value indicating whether the player is holding breath or not.
    /// </summary>
    private bool _isHoldingBreath;

    /// <summary>
    /// A value indicating whether the player is holding breath or not.
    /// </summary>
    public bool IsOutOfBreath;

    /// <summary>
    /// Target velocity of the player.
    /// </summary>
    private Vector3 _targetVelocity;

    /// <summary>
    /// Current velocity of the player.
    /// </summary>
    private Vector3 _currentVelocity;

    /// <summary>
    /// Current direction projected on the wall represented by 1 for the right and -1 for the left.
    /// </summary>
    private float _directionFactor;

    /// <summary>
    /// Current vertical velocity of the player.
    /// </summary>
    private Vector3 _gravityVelocity;

    /// <summary>
    /// The coroutine during which the player can hold his breath.
    /// </summary>
    private Coroutine _holdBreathCoroutine;

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
        if (!_isHoldingBreath && !IsOutOfBreath)
        {
            Move();
            CorrectPosition();
        }
        Zoom();
    }

    public IEnumerator OnExit(StateManager stateManager)
    {
        _stateManager.InputManager.OnMove -= CalculateVelocity;
        _stateManager.InputManager.OnLookWithMouse -= LookWithMouse;
        _stateManager.InputManager.OnLookWithGamepad -= LookWithGamepad;
        _stateManager.InputManager.OnZoomWithMouse -= CalculateZoomValueWithMouse;
        _stateManager.InputManager.OnZoomWithGamepad -= CalculateZoomValueWithGamepad;
        _stateManager.InputManager.OnStartHoldingBreath -= StartToHoldBreath;
        _stateManager.InputManager.OnStopHoldingBreath -= StopToHoldBreath;

        _targetVelocity = Vector3.zero;
        _currentVelocity = Vector3.zero;
        _gravityVelocity = Vector3.zero;

        StopToHoldBreath();
        IsOutOfBreath = false;

        yield return _stateManager.StartCoroutine(InitTransitionToExitWall());

        _stateManager.IsSticking = false;
    }

    private void CancelState()
    {
        _stateManager.StopAllCoroutines();

        _stateManager.InputManager.OnMove -= CalculateVelocity;
        _stateManager.InputManager.OnLookWithMouse -= LookWithMouse;
        _stateManager.InputManager.OnLookWithGamepad -= LookWithGamepad;
        _stateManager.InputManager.OnZoomWithMouse -= CalculateZoomValueWithMouse;
        _stateManager.InputManager.OnZoomWithGamepad -= CalculateZoomValueWithGamepad;
        _stateManager.InputManager.OnStartHoldingBreath -= StartToHoldBreath;
        _stateManager.InputManager.OnStopHoldingBreath -= StopToHoldBreath;

        _targetVelocity = Vector3.zero;
        _currentVelocity = Vector3.zero;
        _gravityVelocity = Vector3.zero;

        StopToHoldBreath();
        IsOutOfBreath = false;

        _stateManager.AnimationController.StopStick();

        IsTransitioning = false;
        _stateManager.IsSticking = false;

        _stateManager.CancelCurrentState();
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

        _stateManager.AnimationController.StartStick();

        yield return _stateManager.StartCoroutine(_stateManager.NavMeshController.TransitionTo(targetPosition, targetRotation, 5f, 25f, true, false, success => { if (!success) CancelState(); }));

        IsTransitioning = false;

        _stateManager.InputManager.OnMove += CalculateVelocity;
        _stateManager.InputManager.OnStartHoldingBreath += StartToHoldBreath;
        _stateManager.InputManager.OnStopHoldingBreath += StopToHoldBreath;
    }

    /// <summary>
    /// Called to initialize a transition to exit the wall.
    /// </summary>
    private IEnumerator InitTransitionToExitWall()
    {
        IsTransitioning = true;

        _stateManager.AnimationController.StopStick();

        // Definition of targets
        Vector3 targetPosition = _stateManager.transform.position + _stateManager.transform.forward * 0.5f;
        Quaternion targetRotation = _stateManager.transform.rotation;
        float speed = _stateManager.WalkSpeed;

        yield return _stateManager.StartCoroutine(_stateManager.NavMeshController.TransitionTo(targetPosition, targetRotation, speed, 10f, true, true, success => { if (!success) CancelState(); }));

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

        // Detect if moving to the left or right relative to character
        float sideFactor = Vector3.Dot(projectedDirection, _stateManager.transform.right);

        if (sideFactor > 0f)
        {
            _directionFactor = 1;
        }
        else if (sideFactor < 0f)
        {
            _directionFactor = -1;
        }
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
            _gravityVelocity.y = _gravityVelocity.y = 0f;
        }
        else
        {
            _gravityVelocity.y -= _stateManager.GravityForce * Time.deltaTime;
        }

        // Application of movement + gravity
        _stateManager.CharacterController.Move((_currentVelocity + _gravityVelocity) * Time.deltaTime);
        _stateManager.transform.position = new Vector3(_stateManager.transform.position.x, MathF.Round(_stateManager.transform.position.y, 3), _stateManager.transform.position.z);
        _stateManager.AnimationController.SetWalkSpeed((_currentVelocity.magnitude / _stateManager.StickSpeed) * _directionFactor);
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

    /// <summary>
    /// Called to start to hold breath.
    /// </summary>
    private void StartToHoldBreath()
    {
        _isHoldingBreath = true;
        _stateManager.AnimationController.StartHoldingBreath();
        _holdBreathCoroutine = _stateManager.StartCoroutine(HoldingBreath());
    }

    /// <summary>
    /// Called to start to hold breath.
    /// </summary>
    private void StopToHoldBreath()
    {
        CancelCoroutine(_holdBreathCoroutine);
        _holdBreathCoroutine = null;

        _stateManager.AnimationController.StopHoldingBreath();
        _isHoldingBreath = false;
    }

    /// <summary>
    /// Called when the player is holding his breath to check if he is out of breath.
    /// </summary>
    /// <returns></returns>
    private IEnumerator HoldingBreath()
    {
        float duration = _stateManager.HoldBreathTime;
        float elapsed = 0f;

        while (duration < elapsed)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        _stateManager.StartCoroutine(OutOfBreath());
        yield return null;
    }

    /// <summary>
    /// Called to wait a delay when player is out of breath.
    /// </summary>
    /// <returns></returns>
    private IEnumerator OutOfBreath()
    {
        _stateManager.AnimationController.OutOfBreath();
        IsOutOfBreath = true;
        StopToHoldBreath();

        float duration = _stateManager.OutOfBreathCooldown;
        float elapsed = 0f;

        while (duration < elapsed)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        _stateManager.AnimationController.StopOutOfBreath();
    }

    /// <summary>
    /// Called to cancel a coroutine.
    /// </summary>
    /// <param name="coroutine"> The coroutine to cancel. </param>
    private void CancelCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            _stateManager.StopCoroutine(coroutine);
        }
    }
}