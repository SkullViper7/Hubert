using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class StickedState : IPlayerState
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
    private int _directionFactor;

    /// <summary>
    /// Current vertical velocity of the player.
    /// </summary>
    private Vector3 _gravityVelocity;

    /// <summary>
    /// The coroutine during which the player can hold his breath.
    /// </summary>
    private Coroutine _holdBreathCoroutine;

    /// <summary>
    /// Event to indicate that holding breath is almost finished.
    /// </summary>
    public event Action OnHoldAlmostFinished;

    private Action _onHoldStopped;

    /// <summary>
    /// Manager of all states.
    /// </summary>
    private PlayerStateManager _stateManager;

    /// <summary>
    /// Last position of the player.
    /// </summary>
    private Vector3 _lastPosition;

    /// <summary>
    /// Smooth velocity to avoid a brutal stop at the limit of a wall.
    /// </summary>
    private Vector3 _smoothedVelocity;

    /// <summary>
    /// A reference to the velocity.
    /// </summary>
    private Vector3 _velocityRef;

    /// <summary>
    /// Real velocity of the player, not the velocity that we try to applie to the character controller.
    /// </summary>
    public Vector3 RealVelocity { get; private set; }

    public IEnumerator OnEnter(PlayerStateManager stateManager)
    {
        _stateManager = stateManager;

        _stateManager.IsSticking = true;

        _onHoldStopped = () => { if (!IsOutOfBreath) _stateManager.StartCoroutine(CancelHoldBreath()); StopToHoldBreath(); };

        _stateManager.InputManager.OnLookWithMouse += LookWithMouse;
        _stateManager.InputManager.OnLookWithGamepad += LookWithGamepad;
        _stateManager.InputManager.OnZoomWithMouse += CalculateZoomValueWithMouse;
        _stateManager.InputManager.OnZoomWithGamepad += CalculateZoomValueWithGamepad;

        yield return _stateManager.StartCoroutine(InitTransitionToWall());

        yield return null;
    }

    public void UpdateState()
    {
        _stateManager.ArmIKManager.SetIKWeights();

        if (!_isHoldingBreath && !IsOutOfBreath && !IsTransitioning && _stateManager.IsSticking)
        {
            Move();
            CorrectPosition();

            Vector3 currentPosition = _stateManager.transform.position;
            RealVelocity = (currentPosition - _lastPosition) / Time.deltaTime;
            _lastPosition = currentPosition;
            _smoothedVelocity = Vector3.SmoothDamp(_smoothedVelocity, RealVelocity, ref _velocityRef, 0.1f);
            _stateManager.AnimationController.SetWalkSpeed((_smoothedVelocity.magnitude / _stateManager.StickSpeed) * _directionFactor);
        }

        Zoom();
    }

    public IEnumerator OnExit()
    {
        IsTransitioning = true;

        _stateManager.InputManager.OnMove -= CalculateVelocity;
        _stateManager.InputManager.OnLookWithMouse -= LookWithMouse;
        _stateManager.InputManager.OnLookWithGamepad -= LookWithGamepad;
        _stateManager.InputManager.OnZoomWithMouse -= CalculateZoomValueWithMouse;
        _stateManager.InputManager.OnZoomWithGamepad -= CalculateZoomValueWithGamepad;
        _stateManager.InputManager.OnStartHoldingBreath -= StartToHoldBreath;
        _stateManager.InputManager.OnStopHoldingBreath -= _onHoldStopped;

        _targetVelocity = Vector3.zero;
        _currentVelocity = Vector3.zero;
        _gravityVelocity = Vector3.zero;
        RealVelocity = Vector3.zero;
        _smoothedVelocity = Vector3.zero;
        _directionFactor = 0;
        _velocityRef = Vector3.zero;

        StopToHoldBreath();
        _stateManager.StartCoroutine(CancelHoldBreath());
        IsOutOfBreath = false;

        _stateManager.ArmIKManager.ResetIKWeights();

        _stateManager.IsSticking = false;
        IsTransitioning = false;

        yield return null;
    }

    public void CancelState()
    {
        _stateManager.StopAllCoroutines();
        _stateManager.NavMeshController.CancelAll();

        _stateManager.InputManager.OnMove -= CalculateVelocity;
        _stateManager.InputManager.OnLookWithMouse -= LookWithMouse;
        _stateManager.InputManager.OnLookWithGamepad -= LookWithGamepad;
        _stateManager.InputManager.OnZoomWithMouse -= CalculateZoomValueWithMouse;
        _stateManager.InputManager.OnZoomWithGamepad -= CalculateZoomValueWithGamepad;
        _stateManager.InputManager.OnStartHoldingBreath -= StartToHoldBreath;
        _stateManager.InputManager.OnStopHoldingBreath -= _onHoldStopped;

        _targetVelocity = Vector3.zero;
        _currentVelocity = Vector3.zero;
        _gravityVelocity = Vector3.zero;
        RealVelocity = Vector3.zero;
        _smoothedVelocity = Vector3.zero;
        _directionFactor = 0;
        _velocityRef = Vector3.zero;

        CancelCoroutine(_holdBreathCoroutine);
        _holdBreathCoroutine = null;
        _isHoldingBreath = false;
        IsOutOfBreath = false;

        _stateManager.ArmIKManager.ResetIKWeights();

        IsTransitioning = false;
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

        _stateManager.AnimationController.PlayStickAnim();

        yield return _stateManager.StartCoroutine(_stateManager.NavMeshController.TransitionTo(targetPosition, targetRotation,
            _stateManager.StickedTransitionInAcceleration, _stateManager.StickedTransitionInAcceleration, _stateManager.StickedTransitionInRotationSpeed,
            true, false, success => { if (!success) _stateManager.StartCoroutine(_stateManager.ResetCurrentState()); }));

        IsTransitioning = false;

        _stateManager.InputManager.OnMove += CalculateVelocity;
        _stateManager.InputManager.OnStartHoldingBreath += StartToHoldBreath;
        _stateManager.InputManager.OnStopHoldingBreath += _onHoldStopped;
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
        Vector3 movementDirection = (cameraDirection * direction.y + cameraRight * direction.x);

        // Project this direction onto the plane of the wall to stay stuck
        Vector3 projectedDirection = Vector3.ProjectOnPlane(movementDirection, _stateManager.StickedNormal).normalized;

        // Calculate the alignment between the input and the possible direction
        float alignmentFactor = Vector3.Dot(movementDirection, projectedDirection);
        alignmentFactor = Mathf.Max(0, alignmentFactor);

        // Apply velocity with a weighting factor
        _targetVelocity = _stateManager.StickSpeed * alignmentFactor * projectedDirection;

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
        _targetVelocity = Vector3.zero;
        _currentVelocity = Vector3.zero;

        _isHoldingBreath = true;
        _stateManager.AnimationController.PlayHoldBreathAnim();
        _holdBreathCoroutine = _stateManager.StartCoroutine(HoldingBreath());
    }

    /// <summary>
    /// Called to stop to hold breath.
    /// </summary>
    private void StopToHoldBreath()
    {
        CancelCoroutine(_holdBreathCoroutine);
        _holdBreathCoroutine = null;

        _stateManager.AnimationController.StopHoldBreathAnim();
        _isHoldingBreath = false;
    }

    private IEnumerator CancelHoldBreath()
    {
        float duration = 0.5f;

        float elapsed = 0f;

        float currentRedValue = 0f;
        if (_stateManager.PlayerMaterials.Count > 1)
        {
            float startRedValue = _stateManager.PlayerMaterials.FirstOrDefault(m => m.name.Contains("RedHead")).GetFloat("_Height");

            while (elapsed < duration)
            {
                currentRedValue = Mathf.Lerp(startRedValue, 0f, elapsed / duration);
                _stateManager.PlayerMaterials.FirstOrDefault(m => m.name.Contains("RedHead")).SetFloat("_Height", currentRedValue);
                _stateManager.PlayerRenderer.materials = _stateManager.PlayerMaterials.ToArray();
                elapsed += Time.deltaTime;
                yield return null;
            }

            _stateManager.PlayerMaterials.FirstOrDefault(m => m.name.Contains("RedHead")).SetFloat("_Height", 0f);
            _stateManager.PlayerMaterials.Remove(_stateManager.RedHeadMaterial);
            _stateManager.PlayerRenderer.materials = _stateManager.PlayerMaterials.ToArray();
        }
    }

    /// <summary>
    /// Called when the player is holding his breath to check if he is out of breath.
    /// </summary>
    /// <returns></returns>
    private IEnumerator HoldingBreath()
    {
        _stateManager.PlayerMaterials.Add(_stateManager.RedHeadMaterial);
        _stateManager.PlayerMaterials.FirstOrDefault(m => m.name.Contains("RedHead")).SetFloat("_Height", 0f);

        float duration = _stateManager.HoldBreathTime;
        float elapsed = 0f;

        float currentRedValue = 0f;
        float startRedValue = 0f;

        while (elapsed < duration)
        {
            currentRedValue = Mathf.Lerp(startRedValue, 3f, elapsed / duration);
            _stateManager.PlayerMaterials.FirstOrDefault(m => m.name.Contains("RedHead")).SetFloat("_Height", currentRedValue);
            _stateManager.PlayerRenderer.materials = _stateManager.PlayerMaterials.ToArray();
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
        _stateManager.AnimationController.PlayOutOfBreathAnim();
        IsOutOfBreath = true;
        StopToHoldBreath();

        float duration = _stateManager.OutOfBreathCooldown;
        float elapsed = 0f;

        float currentRedValue = 0f;
        float startRedValue = _stateManager.PlayerMaterials.FirstOrDefault(m => m.name.Contains("RedHead")).GetFloat("_Height");

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (currentRedValue >= 0f)
            {
                currentRedValue = Mathf.Lerp(startRedValue, 0f, elapsed / duration);
                _stateManager.PlayerMaterials.FirstOrDefault(m => m.name.Contains("RedHead")).SetFloat("_Height", currentRedValue);
                _stateManager.PlayerRenderer.materials = _stateManager.PlayerMaterials.ToArray();
            }

            if (elapsed > duration - 3f)
            {
                OnHoldAlmostFinished?.Invoke();
            }

            yield return null;
        }

        _stateManager.PlayerMaterials.FirstOrDefault(m => m.name.Contains("RedHead")).SetFloat("_Height", 0f);
        _stateManager.PlayerMaterials.Remove(_stateManager.RedHeadMaterial);
        _stateManager.PlayerRenderer.materials = _stateManager.PlayerMaterials.ToArray();

        _stateManager.AnimationController.StopOutOfBreathAnim();
        IsOutOfBreath = false;
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