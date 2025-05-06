using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class HitState : IState
{
    /// <summary>
    /// Manager of all states.
    /// </summary>
    private StateManager _stateManager;

    /// <summary>
    /// The enemy to hit.
    /// </summary>
    private Transform _enemyToHit;

    public IEnumerator OnEnter(StateManager stateManager)
    {
        _stateManager = stateManager;

        _stateManager.IsHitting = true;

        _enemyToHit = _stateManager.EnemyToHit;

        _stateManager.InputManager.OnLookWithMouse += LookWithMouse;
        _stateManager.InputManager.OnLookWithGamepad += LookWithGamepad;
        _stateManager.InputManager.OnZoomWithMouse += CalculateZoomValueWithMouse;
        _stateManager.InputManager.OnZoomWithGamepad += CalculateZoomValueWithGamepad;
        _stateManager.AnimationController.MustHit += KillEnemy;

        // Launch a coroutine to manage the transition
        Vector3 enemyToPlayer = (_stateManager.transform.position - _enemyToHit.position).normalized;
        enemyToPlayer.y = 0;
        Vector3 hitPosition = _enemyToHit.position + (enemyToPlayer * (_enemyToHit.GetComponent<NavMeshAgent>().radius + _stateManager.CharacterController.radius + 0.1f));
        Quaternion rotationToEnemy = Quaternion.LookRotation(-enemyToPlayer);
        float speed = _stateManager.WalkSpeed;

        yield return _stateManager.StartCoroutine(_stateManager.NavMeshController.TransitionTo(hitPosition, rotationToEnemy, speed, 10f, true, true, success => { if (!success) _stateManager.StartCoroutine(_stateManager.ResetCurrentState()); }));

        _stateManager.AnimationController.PlayHitAnim();
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
        _stateManager.AnimationController.MustHit -= KillEnemy;

        _stateManager.AnimationController.StopHitAnim();

        _stateManager.IsHitting = false;

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
        _stateManager.AnimationController.MustHit -= KillEnemy;

        _stateManager.AnimationController.StopHitAnim();

        _stateManager.IsHitting = false;
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
    /// Called to kill the enemy.
    /// </summary>
    private void KillEnemy()
    {
        _enemyToHit.GetComponent<Enemy>().Death();
    }
}
