using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class MediumAimingState : IEnemyState
{
    #region General
    /// <summary>
    /// Brain of the enemy.
    /// </summary>
    private MediumEnemyBrain _brain;

    /// <summary>
    /// Navmesh agent of the enemy.
    /// </summary>
    private NavMeshAgent _agent;

    /// <summary>
    /// A value indicating if the enemy has to rotate to the player.
    /// </summary>
    private bool _hasToRotate;

    /// <summary>
    /// A value to indicate that the enemy is transitionning.
    /// </summary>
    private bool _isTransitionning;

    /// <summary>
    /// The manager of all enemies.
    /// </summary>
    private EnemyManager _enemyManager;
    #endregion

    #region Vision
    /// <summary>
    /// Coroutine of going to player.
    /// </summary>
    private Coroutine _goToPlayerCoroutine;

    /// <summary>
    /// Action to update the player position.
    /// </summary>
    private Action<PlayerPosition> _updatePlayerPos;

    /// <summary>
    /// Action to go to the player position.
    /// </summary>
    private Action<PlayerPosition> _goToPlayerPos;

    /// <summary>
    /// The current player pos followed by the enemy.
    /// </summary>
    private PlayerPosition _currentPlayerPos;
    #endregion

    #region Aim
    /// <summary>
    /// Action when enemy is to far to aim.
    /// </summary>
    private Action _aimExited;

    /// <summary>
    /// A value to indicate if the state has to exit with the animation.
    /// </summary>
    private bool _exitWithAnim;
    #endregion

    #region Shot
    /// <summary>
    /// Coroutine of shot.
    /// </summary>
    private Coroutine _shotCoroutine;
    #endregion

    public IEnumerator OnEnter(EnemyBrain enemyBrain, EnemyStateEnterType enemyStateEnterType)
    {
        // Get components
        _brain = (MediumEnemyBrain)enemyBrain;
        _agent = _brain.NavMeshAgent;
        _enemyManager = EnemyManager.Instance;

        // Get values
        _agent.speed = _brain.AimWalkSpeed;
        _agent.acceleration = _brain.AimAcceleration;
        _brain.EnemyVision.DetectionRange = _brain.AlerteVisionRange;

        // Action to update the player position.
        _updatePlayerPos = (PlayerPosition position) =>
        {
            _currentPlayerPos = position;
        };
        // Event to update the player position.
        _brain.CurrentRoom.OnPlayerPosUpdated += _updatePlayerPos;

        // Action when player position is updated
        _goToPlayerPos = (PlayerPosition position) =>
        {
            // Go to player position
            CancelGoingToPlayerPos();
            _goToPlayerCoroutine = _brain.StartCoroutine(GoToPlayerPos(position));
        };

        // Listener when enemy must shoot
        _brain.MediumAnimationController.OnMustShoot += Shoot;

        // Listener when enemy has shot
        _brain.MediumAnimationController.OnFinishToShoot += HasShot;

        // Action when enemy is to far to aim
        _aimExited = () =>
        {
            _exitWithAnim = true;
            _brain.StartCoroutine(_brain.ChangeState(_brain.MediumAlerteState, EnemyStateEnterType.HasAGoalButNoAstonishment));
        };
        // Listener when enemy is to far to aim
        _brain.OnAimExited += _aimExited;

        // Play taking out the gun
        Vector3 direction = (_brain.CurrentRoom.LastKnownPlayerPos.Position - _brain.transform.position).normalized;
        direction.y = 0f;
        _brain.transform.rotation = Quaternion.LookRotation(direction);
        _hasToRotate = true;
        yield return _goToPlayerCoroutine = _brain.StartCoroutine(PlayGunAction("AimStart"));
        _hasToRotate = false;

        _brain.CurrentRoom.OnPlayerPosUpdated += _goToPlayerPos;
        CancelGoingToPlayerPos();
        _goToPlayerCoroutine = _brain.StartCoroutine(GoToPlayerPos(_brain.CurrentRoom.LastKnownPlayerPos));

        CancelCoroutine(_shotCoroutine);
        _shotCoroutine = _brain.StartCoroutine(ShotCooldown());

        yield return null;
    }

    public void UpdateState()
    {
        if (_isTransitionning) return;

        if (_currentPlayerPos != null)
        {
            // Don't move if player is to close but rotate
            if (Vector3.Distance(_brain.transform.position, _currentPlayerPos.Position) <= _brain.MinDistanceToThePlayer || _hasToRotate)
            {
                _brain.StopMovement();

                // Direction of motion on the XZ plane only
                Vector3 direction = (new Vector3(_currentPlayerPos.Position.x, 0, _currentPlayerPos.Position.z) - new Vector3(_brain.transform.position.x, 0, _brain.transform.position.z)).normalized;
                Debug.DrawRay(_brain.TargetTransform.position, direction * 10f, Color.red);

                if (direction == Vector3.zero)
                    return;

                // Calculate target rotation on Y axis only
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

                // Smooth rotation with Slerp
                _brain.transform.rotation = Quaternion.Slerp(
                    _brain.transform.rotation,
                    targetRotation,
                    _brain.AngularSpeed * Time.deltaTime
                );
            }
        }

        _brain.AnimationController.SetWalkSpeed(_brain.NavMeshAgent.velocity.magnitude / _brain.NavMeshAgent.speed);
        _brain.TryTransmiteState();
    }

    public IEnumerator OnExit()
    {
        _isTransitionning = true;

        _brain.CurrentRoom.OnPlayerPosUpdated -= _updatePlayerPos;
        _brain.CurrentRoom.OnPlayerPosUpdated -= _goToPlayerPos;
        _brain.MediumAnimationController.OnMustShoot -= Shoot;
        _brain.MediumAnimationController.OnFinishToShoot -= HasShot;
        _brain.OnAimExited -= _aimExited;

        CancelCoroutine(_goToPlayerCoroutine);
        CancelCoroutine(_shotCoroutine);
        _brain.StopMovement();
        _brain.StopLookingAround();
        _brain.StopAstonishment();
        _brain.StopGunAction();

        if (_exitWithAnim)
        {
            _exitWithAnim = false;

            // Play taking out the gun
            _hasToRotate = true;
            yield return _goToPlayerCoroutine = _brain.StartCoroutine(PlayGunAction("AimEnd"));
            _hasToRotate = false;
            _brain.StopGunAction();
        }

        _isTransitionning = false;

        yield return null;
    }

    /// <summary>
    /// Called to play a gun action animation.
    /// </summary>
    /// <param name="trigger"> Trigger of the animation. </param>
    /// <returns></returns>
    private IEnumerator PlayGunAction(string trigger)
    {
        // Remove event when player is seen
        _brain.CurrentRoom.OnPlayerPosUpdated -= _goToPlayerPos;

        _brain.StopMovement();
        _brain.StopGunAction();

        // Play gun action animation
        yield return _brain.TakeOutOrPutAwayGun(trigger);

        _goToPlayerCoroutine = _brain.StartCoroutine(GoToPlayerPos(_brain.CurrentRoom.LastKnownPlayerPos));
    }

    #region Vision
    /// <summary>
    /// Called to go to a player position.
    /// </summary>
    /// <param name="playerPos"> Player position to go. </param>
    /// <returns></returns>
    private IEnumerator GoToPlayerPos(PlayerPosition playerPos)
    {
        _brain.StopMovement();
        _brain.StopGunAction();

        _currentPlayerPos = playerPos;

        // Launch timer
        _brain.CurrentRoom.StartAlerteChrono(_enemyManager.AlerteTimer);

        // Launch animation
        _brain.MediumAnimationController.PlayAimAnim();

        // Go to the last player position
        bool reached = false;
        yield return _brain.SetDestination(_currentPlayerPos.Position, success => reached = success);
    }

    /// <summary>
    /// Called to cancel going to a player position.
    /// </summary>
    private void CancelGoingToPlayerPos()
    {
        CancelCoroutine(_goToPlayerCoroutine);
        _brain.StopMovement();
        _brain.StopGunAction();
    }
    #endregion

    #region Shot
    /// <summary>
    /// Called to wait before shoot.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShotCooldown()
    {
        float waitTime = UnityEngine.Random.Range(_brain.ShotDelay.Min, _brain.ShotDelay.Max);
        yield return new WaitForSeconds(waitTime);

        // Remove event when player is seen
        _brain.CurrentRoom.OnPlayerPosUpdated -= _goToPlayerPos;
        _hasToRotate = true;

        _brain.StopMovement();
        _brain.StopGunAction();

        _brain.MediumAnimationController.PlayShootAnim();
    }

    /// <summary>
    /// Called to shoot a bullet.
    /// </summary>
    private void Shoot()
    {
        GameObject newBullet = GameObject.Instantiate(_brain.BulletPrefab, _brain.BulletSocket.position, Quaternion.LookRotation(_brain.transform.forward));
        newBullet.GetComponent<EnemyBullet>().InitBullet(_brain.BulletSpeed);
    }

    /// <summary>
    /// Called when enemy has shot to relaunch a cooldown.
    /// </summary>
    private void HasShot()
    {
        _hasToRotate = false;
        _brain.CurrentRoom.OnPlayerPosUpdated += _goToPlayerPos;
        CancelGoingToPlayerPos();
        _goToPlayerCoroutine = _brain.StartCoroutine(GoToPlayerPos(_brain.CurrentRoom.LastKnownPlayerPos));

        CancelCoroutine(_shotCoroutine);
        _shotCoroutine = _brain.StartCoroutine(ShotCooldown());
    }
    #endregion

    /// <summary>
    /// Called to cancel a coroutine.
    /// </summary>
    /// <param name="coroutine"> The coroutine to cancel. </param>
    private void CancelCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            _brain.StopCoroutine(coroutine);
        }
    }
}