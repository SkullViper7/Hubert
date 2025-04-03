using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AimingState : IState
{
    public event Action OnAimStop;

    public event Action<GameObject> OnNewEnemyTargeted;

    /// <summary>
    /// A value indicating that the player is shooting.
    /// </summary>
    public bool IsShooting { get; private set; }

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
    /// List of all enemies visible on camera and by the player.
    /// </summary>
    private List<GameObject> _visibleEnemies = new();

    /// <summary>
    /// Current target selected.
    /// </summary>
    private GameObject _currentTarget;

    /// <summary>
    /// Current index of the target selected.
    /// </summary>
    private int _currentIndex;

    /// <summary>
    /// Manager of all states.
    /// </summary>
    private StateManager _stateManager;

    public IEnumerator OnEnter(StateManager stateManager)
    {
        _stateManager = stateManager;

        _stateManager.IsAiming = true;

        IsShooting = false;

        _stateManager.InputManager.OnMove += CalculateVelocity;
        _stateManager.InputManager.OnLookWithMouse += LookWithMouse;
        _stateManager.InputManager.OnLookWithGamepad += LookWithGamepad;
        _stateManager.InputManager.OnSwitchTarget += SwitchTarget;
        _stateManager.InputManager.OnShoot += InitShoot;
        _stateManager.AnimationController.MustShoot += Shoot;

        _visibleEnemies = GetVisibleEnemiesAroundPlayer();
        GetClosestEnemyInView();

        _stateManager.AnimationController.StartAim();

        // Launch a coroutine to manage the transition
        _stateManager.StartCoroutine(TransitionCameraY());

        yield return null;
    }

    public void UpdateState(StateManager stateManager)
    {
        _visibleEnemies = GetVisibleEnemiesAroundPlayer();
        Move();
    }

    public IEnumerator OnExit(StateManager stateManager)
    {
        _stateManager.InputManager.OnMove -= CalculateVelocity;
        _stateManager.InputManager.OnLookWithMouse -= LookWithMouse;
        _stateManager.InputManager.OnLookWithGamepad -= LookWithGamepad;
        _stateManager.InputManager.OnSwitchTarget -= SwitchTarget;
        _stateManager.InputManager.OnShoot -= InitShoot;
        _stateManager.AnimationController.MustShoot -= Shoot;

        _targetVelocity = Vector3.zero;
        _currentVelocity = Vector3.zero;
        _gravityVelocity = Vector3.zero;

        _visibleEnemies.Clear();
        _currentTarget = null;
        _currentIndex = 0;

        _stateManager.AnimationController.StopAim();

        OnAimStop?.Invoke();

        IsShooting = false;
        _stateManager.IsAiming = false;

        yield return null;
    }

    /// <summary>
    /// Called to unzoom the camera.
    /// </summary>
    /// <returns></returns>
    private IEnumerator TransitionCameraY()
    {
        float startY = _stateManager.Camera.m_YAxis.Value;
        float transitionProgress = 0f;

        while (transitionProgress < 1f)
        {
            transitionProgress += Time.deltaTime / _stateManager.CameraUnzoomTime;

            // Interpolation only on the Y axis
            float newY = Mathf.Lerp(startY, 1, transitionProgress);
            _stateManager.Camera.m_YAxis.Value = newY;

            yield return null;
        }
    }

    /// <summary>
    /// Called to calculate the velocity of the player.
    /// </summary>
    /// <param name="direction"> Direction of the movement. </param>
    private void CalculateVelocity(Vector2 direction)
    {
        if (_stateManager.Camera == null || IsShooting) return;

        // Calculate the camera direction relative to the player
        Vector3 cameraDirection = (_stateManager.transform.position - _stateManager.Camera.transform.position).normalized;

        // Cancel vertical axis to prevent player from moving up/down
        cameraDirection.y = 0;
        cameraDirection.Normalize();

        // Calculate a "straight" axis perpendicular to this direction
        Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraDirection).normalized;

        // Apply motion direction based on camera
        _targetVelocity = (cameraDirection * direction.y + cameraRight * direction.x) * _stateManager.AimSpeed;
    }

    /// <summary>
    /// Called to move the player and rotate him.
    /// </summary>
    private void Move()
    {
        if (IsShooting) return;

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

            GetClosestEnemyInView();
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
    /// Called to get visible enemies around the player and sort them by clockwise order.
    /// </summary>
    /// <returns></returns>
    private List<GameObject> GetVisibleEnemiesAroundPlayer()
    {
        List<GameObject> visibleEnemies = new();

        // Get all enemies in the layer within a given radius
        Collider[] colliders = Physics.OverlapSphere(_stateManager.transform.position, _stateManager.AimRange, LayerMask.GetMask("Enemy"));

        // Check if they are in the camera's field of view
        Plane[] cameraFrustum = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        foreach (Collider collider in colliders)
        {
            GameObject enemy = collider.gameObject;
            Bounds enemyBounds = collider.bounds;

            if (GeometryUtility.TestPlanesAABB(cameraFrustum, enemyBounds))
            {
                // Check if the object is in front of the camera
                Vector3 directionToObject = (enemy.transform.position - _stateManager.Camera.transform.position).normalized;
                if (Vector3.Dot(Camera.main.transform.forward, directionToObject) > 0)
                {
                    // Check walls between the player and the enemy
                    if (Physics.Linecast(_stateManager.transform.position, enemy.transform.position, out RaycastHit hit))
                    {
                        if (hit.collider.gameObject == enemy)
                        {
                            visibleEnemies.Add(enemy);
                        }
                    }
                }
            }
        }

        // Sorting objects by hour angle
        visibleEnemies = visibleEnemies.OrderByDescending(obj =>
        {
            Vector3 direction = obj.transform.position - _stateManager.transform.position;
            float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            return (angle + 360) % 360;
        }).ToList();

        return visibleEnemies;
    }

    /// <summary>
    /// Called to get the closest enemy to the player's orientation.
    /// </summary>
    private void GetClosestEnemyInView()
    {
        if (_visibleEnemies == null || _visibleEnemies.Count == 0) return;

        GameObject bestTarget = null;
        int bestIndex = 0;
        float maxDot = -1f;

        Vector3 playerForward = _stateManager.transform.forward;

        for (int i = 0; i < _visibleEnemies.Count; i++)
        {
            Vector3 directionToEnemy = (_visibleEnemies[i].transform.position - _stateManager.transform.position).normalized;

            // Dot product between player forward and direction toward enemy
            float dot = Vector3.Dot(playerForward, directionToEnemy);

            // The closer dot is to 1, the more the enemy is aligned with the gaze
            if (dot > maxDot)
            {
                maxDot = dot;
                bestTarget = _visibleEnemies[i];
                bestIndex = i;
            }
        }

        if (bestTarget != _currentTarget)
        {
            OnNewEnemyTargeted(bestTarget);
            _currentTarget = bestTarget;
        }

        _currentIndex = bestIndex;
    }

    /// <summary>
    /// Called to switch the target manually in clockwise.
    /// </summary>
    /// <param name="value"> Value of the switch. </param>
    private void SwitchTarget(int value)
    {
        if (_visibleEnemies.Count == 0 || IsShooting) return;

        // Calculation of the new index in a circular manner
        _currentIndex = (_currentIndex + value + _visibleEnemies.Count) % _visibleEnemies.Count;

        _currentTarget = _visibleEnemies[_currentIndex];

        OnNewEnemyTargeted(_currentTarget);
    }

    // Called to init the shoot.
    private void InitShoot()
    {
        if (_currentTarget == null || IsShooting) return;

        IsShooting = true;

        Quaternion targetDirection = Quaternion.LookRotation(_currentTarget.transform.position - _stateManager.transform.position);

        _stateManager.StartCoroutine(TransitionRotationBeforeShoot(targetDirection));
    }

    /// <summary>
    /// Called to launch a rotation to the enemy to shoot.
    /// </summary>
    /// <param name="targetRotation"> Direction to enemy. </param>
    /// <returns></returns>
    private IEnumerator TransitionRotationBeforeShoot(Quaternion targetRotation)
    {
        Vector3 targetPosition = _stateManager.transform.position;
        float speed = _stateManager.AimSpeed;

        yield return _stateManager.StartCoroutine(_stateManager.NavMeshController.TransitionTo(targetPosition, targetRotation, speed, true));

        _stateManager.AnimationController.PlayShootAnim();
    }

    /// <summary>
    /// Called to shoot a bullet.
    /// </summary>
    private void Shoot()
    {
        GameObject newBullet = GameObject.Instantiate(_stateManager.BulletPrefab, _stateManager.BulletSocket.position, Quaternion.identity);
        newBullet.GetComponent<Bullet>().InitBullet(_currentTarget.transform, _stateManager.BulletSpeed, _stateManager.HitThreshold);
    }
}
