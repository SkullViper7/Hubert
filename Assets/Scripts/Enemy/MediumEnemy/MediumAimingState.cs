using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MediumAimingState : IEnemyState
{
    public IEnumerator OnEnter(EnemyBrain enemyBrain, EnemyStateEnterType enemyStateEnterType)
    {
        Debug.Log("enter aim");
        yield return null;
    }

    public void UpdateState()
    {
        //_brain.AnimationController.SetWalkSpeed(_brain.NavMeshAgent.velocity.magnitude / _brain.NavMeshAgent.speed);
        //_brain.TryTransmiteState();
    }

    public IEnumerator OnExit()
    {
        yield return null;
    }
}
