using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MediumAlerteState : IEnemyState
{
    /// <summary>
    /// Brain of the enemy.
    /// </summary>
    private MediumEnemyBrain _brain;

    /// <summary>
    /// Navmesh agent of the enemy.
    /// </summary>
    private NavMeshAgent _agent;

    /// <summary>
    /// Coroutine of the patrol.
    /// </summary>
    private Coroutine _patrolCoroutine;

    /// <summary>
    /// The manager of all enemies.
    /// </summary>
    private EnemyManager _enemyManager;

    public IEnumerator OnEnter(EnemyBrain enemyBrain, EnemyStateEnterType enemyStateEnterType)
    {
        // Get components
        _brain = (MediumEnemyBrain)enemyBrain;
        _agent = _brain.NavMeshAgent;
        _enemyManager = EnemyManager.Instance;

        // Get values
        _agent.speed = _brain.AlerteWalkSpeed;
        _agent.acceleration = _brain.AlerteAcceleration;
        _brain.EnemyVision.DetectionRange = _brain.AlerteVisionRange;

        //// Set listeners
        //// Action when a sound is heared
        //_goToSoundSource = (SoundSource source) =>
        //{
        //    CancelGoingToSoundSource();
        //    _goToSoundCoroutine = _brain.StartCoroutine(GoToSoundSource(source));
        //};
        //// Listener when the sound is heared
        //_brain.EnemyHearing.OnSoundHeard += _goToSoundSource;
        //// Action when going to a sound is canceled
        //_onGoingToSoundCanceled = () => _patrolCoroutine = _brain.StartCoroutine(SoundHasAlreadyBeenChecked());
        //// Action when the research time is ended
        //_onResearchEnded = () => _brain.StartCoroutine(_brain.ChangeState(_brain.MediumPatrolState, EnemyStateEnterType.Null));
        //// Listener when the research is ended
        //_enemyManager.OnResearchEnded += _onResearchEnded;
        ////_onPlayerSeen = () => _brain.StartCoroutine(_brain.ChangeState(_brain.MediumAlerteState, EnemyStateEnterType.HasNoGoal));
        ////_brain.EnemyVision.OnPlayerSeen += _onPlayerSeen;

        //// If enemy has goal it means that he has to go to the last sound position
        //if (enemyStateEnterType == EnemyStateEnterType.HasAGoal)
        //{
        //    CancelGoingToSoundSource();
        //    _goToSoundCoroutine = _brain.StartCoroutine(GoToSoundSource(_brain.LastSoundHeared));
        //}
        //else if (enemyStateEnterType == EnemyStateEnterType.HasNoGoal)
        //{
        //    // Launch a patrol
        //    StartPatrol();
        //}

        yield return null;
    }

    public void UpdateState()
    {
        _brain.AnimationController.SetWalkSpeed(_brain.NavMeshAgent.velocity.magnitude / _brain.NavMeshAgent.speed);
        _brain.TryTransmiteState();
    }

    public IEnumerator OnExit()
    {
        //_brain.EnemyHearing.OnSoundHeard -= _goToSoundSource;
        //_enemyManager.OnResearchEnded -= _onResearchEnded;
        //// _brain.EnemyVision.OnPlayerSeen -= _onPlayerSeen;

        //// Unsubscribe to the last source
        //_enemyManager.Unsubscribe(_currentSoundSource, _onGoingToSoundCanceled);

        //CancelCoroutine(_patrolCoroutine);
        //CancelCoroutine(_goToSoundCoroutine);
        //_brain.StopMovement();
        //_brain.StopLookingAround();
        //_brain.StopAstonishment();
        yield return null;
    }

    public void CancelState()
    {
        //_brain.EnemyHearing.OnSoundHeard -= _goToSoundSource;
        //_enemyManager.OnResearchEnded -= _onResearchEnded;
        //// _brain.EnemyVision.OnPlayerSeen -= _onPlayerSeen;

        //// Unsubscribe to the last source
        //_enemyManager.Unsubscribe(_currentSoundSource, _onGoingToSoundCanceled);

        //CancelCoroutine(_patrolCoroutine);
        //CancelCoroutine(_goToSoundCoroutine);
        //_brain.StopMovement();
        //_brain.StopLookingAround();
        //_brain.StopAstonishment();
    }
}
