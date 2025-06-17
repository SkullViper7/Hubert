using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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
    /// Actions when the room is changed.
    /// </summary>
    private Action<Room> _roomChanged;

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

        // Action when player position is updated
        _goToPlayerPos = (PlayerPosition position) =>
        {
            // Go to player position
            CancelGoingToPlayerPos();
            _goToPlayerCoroutine = _brain.StartCoroutine(GoToPlayerPos(position));
        };

        // Action when enemy is enough close to aim
        _aimExited = () =>
        {
            _brain.StartCoroutine(_brain.ChangeState(_brain.MediumAlerteState, EnemyStateEnterType.HasAGoalButNoAstonishment));
        };
        // Listener when enemy is enough close to aim
        _brain.OnAimExited += _aimExited;

        // Play taking out the gun
        yield return _goToPlayerCoroutine = _brain.StartCoroutine(PlayGunAction("AimStart"));

        _brain.CurrentRoom.OnPlayerPosUpdated += _goToPlayerPos;

        yield return null;
    }

    public void UpdateState()
    {
        if (_isTransitionning) return;
        _brain.AnimationController.SetWalkSpeed(_brain.NavMeshAgent.velocity.magnitude / _brain.NavMeshAgent.speed);
        _brain.TryTransmiteState();
    }

    public IEnumerator OnExit()
    {
        _isTransitionning = true;

        _brain.OnRoomChanged -= _roomChanged;
        _brain.CurrentRoom.OnPlayerPosUpdated -= _goToPlayerPos;
        _brain.OnAimExited -= _aimExited;

        CancelCoroutine(_goToPlayerCoroutine);
        _brain.StopMovement();
        _brain.StopLookingAround();
        _brain.StopAstonishment();
        _brain.StopGunAction();

        // Play taking out the gun
        yield return _goToPlayerCoroutine = _brain.StartCoroutine(PlayGunAction("AimEnd"));

        _brain.StopGunAction();

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
        _brain.StopLookingAround();
        _brain.StopAstonishment();
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
        _brain.StopLookingAround();
        _brain.StopAstonishment();
        _brain.StopGunAction();

        // Subscribe to the new player position
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
        _brain.StopLookingAround();
        _brain.StopAstonishment();
        _brain.StopGunAction();
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