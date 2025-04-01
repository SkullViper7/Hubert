using UnityEngine;

public class CrawlingState : IState
{
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

        _stateManager.IsCrawling = true;

        _stateManager.InputManager.OnMove += CalculateVelocity;
        _stateManager.InputManager.OnLookWithMouse += LookWithMouse;
        _stateManager.InputManager.OnLookWithGamepad += LookWithGamepad;
        _stateManager.InputManager.OnZoomWithMouse += CalculateZoomValueWithMouse;
        _stateManager.InputManager.OnZoomWithGamepad += CalculateZoomValueWithGamepad;

        _stateManager.AnimationController.StartCrawl();
    }

    public void UpdateState(StateManager stateManager)
    {
        Move();
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

        _stateManager.AnimationController.StopCrawl();

        _stateManager.IsCrawling = false;
    }

    /// <summary>
    /// Called to calculate the velocity of the player.
    /// </summary>
    /// <param name="direction"> Direction of the movement. </param>
    private void CalculateVelocity(Vector2 direction)
    {
        if (_stateManager.Camera == null) return;

        // Calculate the camera direction relative to the player
        Vector3 cameraDirection = (_stateManager.transform.position - _stateManager.Camera.transform.position).normalized;

        // Cancel vertical axis to prevent player from moving up/down
        cameraDirection.y = 0;
        cameraDirection.Normalize();

        // Calculate a "straight" axis perpendicular to this direction
        Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraDirection).normalized;

        // Apply motion direction based on camera
        _targetVelocity = (cameraDirection * direction.y + cameraRight * direction.x) * _stateManager.CrawlSpeed;
    }

    /// <summary>
    /// Called to move the player and rotate him.
    /// </summary>
    private void Move()
    {
        // Calculate velocity whith acceleration and deceleration
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

        // Apply rotation only if moving
        if (_currentVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(_currentVelocity.x, 0, _currentVelocity.z));
            _stateManager.transform.rotation = Quaternion.Lerp(_stateManager.transform.rotation, targetRotation, _stateManager.RotationSpeed * Time.deltaTime);
        }
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
