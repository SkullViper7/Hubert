using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PatrolType
{
    Patrol,
    Fixed
}

public class MediumEnemyBrain : EnemyBrain
{
    #region General
    /// <summary>
    /// Navmesh agent of the enemy.
    /// </summary>
    private NavMeshAgent _navMeshAgent;

    /// <summary>
    /// Component which manages animations.
    /// </summary>
    private MediumEnemyAnimationController _animationController;
    #endregion

    #region Patrol
    /// <summary>
    /// Walk speed of the enemy when he is in patrol state.
    /// </summary>
    [field: SerializeField, Header("Patrol")]
    public float PatrolWalkSpeed { get; private set; }

    /// <summary>
    /// Acceleration of the enemy when he is in patrol state.
    /// </summary>
    [field: SerializeField]
    public float PatrolAcceleration { get; private set; }

    /// <summary>
    /// Angular speed of the enemy when he is in patrol state.
    /// </summary>
    [field: SerializeField]
    public float PatrolAngularSpeed { get; private set; }

    /// <summary>
    /// Type of the patrol of the enemy.
    /// </summary>
    [SerializeField]
    private PatrolType _patrolType;

    /// <summary>
    /// Path of the enemy during his patrol.
    /// </summary>
    [SerializeField]
    private List<Waypoint> _path;
    #endregion

    #region Research
    /// <summary>
    /// Walk speed of the enemy when he is in research state.
    /// </summary>
    [field: SerializeField, Header("Research")]
    public float ResearchWalkSpeed { get; private set; }

    /// <summary>
    /// Acceleration of the enemy when he is in research state.
    /// </summary>
    [field: SerializeField]
    public float ResearchAcceleration { get; private set; }

    /// <summary>
    /// Angular speed of the enemy when he is in research state.
    /// </summary>
    [field: SerializeField]
    public float ResearchAngularSpeed { get; private set; }
    #endregion

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

}
