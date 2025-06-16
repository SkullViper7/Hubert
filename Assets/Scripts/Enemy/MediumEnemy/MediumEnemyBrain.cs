using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MediumEnemyBrain : EnemyBrain
{
    #region General
    /// <summary>
    /// Component which manages animations.
    /// </summary>
    public MediumEnemyAnimationController MediumAnimationController { get; private set; }

    /// <summary>
    /// The minimum distance to reach before to come back to the originx (in steps).
    /// </summary>
    [field: SerializeField]
    public int MinDistance { get; private set; }

    /// <summary>
    /// The maximum distance reachable to do a loop (in steps).
    /// </summary>
    [field: SerializeField]
    public int MaxDistance { get; private set; }

    /// <summary>
    /// The distance for a ping-pong if no loop is found.
    /// </summary>
    [field: SerializeField]
    public int PingPongDistance { get; private set; }
    #endregion

    #region Patrol
    /// <summary>
    /// Range of the vision in patrol state.
    /// </summary>
    [field: SerializeField, Header("Patrol")]
    public float PatrolVisionRange { get; private set; }

    /// <summary>
    /// Walk speed of the enemy when he is in patrol state.
    /// </summary>
    [field: SerializeField]
    public float PatrolWalkSpeed { get; private set; }

    /// <summary>
    /// Acceleration of the enemy when he is in patrol state.
    /// </summary>
    [field: SerializeField]
    public float PatrolAcceleration { get; private set; }

    /// <summary>
    /// Type of the patrol of the enemy.
    /// </summary>
    [field: SerializeField]
    public PatrolType PatrolType { get; private set; }

    /// <summary>
    /// Path of the enemy during his patrol.
    /// </summary>
    [field: SerializeField]
    public List<Waypoint> Path { get; private set; }

    /// <summary>
    /// The range of time during which the enemy is looking a fixed point before to look around.
    /// </summary>
    [field: SerializeField]
    public MinMaxInt FixedWaypointDuration { get; private set; }

    /// <summary>
    /// A value indicating if the gizmos are visibles or not.
    /// </summary>
    [SerializeField]
    private bool _showGizmos = true;

    /// <summary>
    /// Color of the gizmos.
    /// </summary>
    [SerializeField]
    private Color _gizmoColor = Color.green;

    /// <summary>
    /// Patrol state of the medium enemy.
    /// </summary>
    public MediumPatrolState MediumPatrolState { get; private set; } = new();
    #endregion

    #region Research
    /// <summary>
    /// Range of the vision in research state.
    /// </summary>
    [field: SerializeField, Header("Research")]
    public float ResearchVisionRange { get; private set; }

    /// <summary>
    /// Walk speed of the enemy when he is in research state.
    /// </summary>
    [field: SerializeField]
    public float ResearchWalkSpeed { get; private set; }

    /// <summary>
    /// Acceleration of the enemy when he is in research state.
    /// </summary>
    [field: SerializeField]
    public float ResearchAcceleration { get; private set; }

    /// <summary>
    /// Research state of the medium enemy.
    /// </summary>
    public MediumResearchState MediumResearchState { get; private set; } = new();
    #endregion

    #region Alerte
    /// <summary>
    /// Range of the vision in Alerte state.
    /// </summary>
    [field: SerializeField, Header("Alerte")]
    public float AlerteVisionRange { get; private set; }

    /// <summary>
    /// Walk speed of the enemy when he is in alerte state.
    /// </summary>
    [field: SerializeField]
    public float AlerteWalkSpeed { get; private set; }

    /// <summary>
    /// Acceleration of the enemy when he is in alerte state.
    /// </summary>
    [field: SerializeField]
    public float AlerteAcceleration { get; private set; }

    /// <summary>
    /// Alerte state of the medium enemy.
    /// </summary>
    public MediumAlerteState MediumAlerteState { get; private set; } = new();
    #endregion

    #region Aim
    /// <summary>
    /// Range around the player that an enemy as to reach to start aiming the player.
    /// </summary>
    [field: SerializeField, Header("Aim")]
    public float StartAimTreshold { get; private set; }

    /// <summary>
    /// Range around the player that an enemy as to reach to stop aiming the player.
    /// </summary>
    [field: SerializeField]
    public float StopAimTreshold { get; private set; }

    /// <summary>
    /// Walk speed of the enemy when he is in aiming state.
    /// </summary>
    [field: SerializeField]
    public float AimWalkSpeed { get; private set; }

    /// <summary>
    /// Acceleration of the enemy when he is in aiming state.
    /// </summary>
    [field: SerializeField]
    public float AimAcceleration { get; private set; }


    /// <summary>
    /// Aiming state of the medium enemy.
    /// </summary>
    public MediumAimingState MediumAimingState { get; private set; } = new();
    #endregion

    protected override void Awake()
    {
        base.Awake();
        MediumAnimationController = (MediumEnemyAnimationController)base.AnimationController;
    }

    protected override void Start()
    {
        base.Start();

        // Start with default state.
        StartCoroutine(ChangeState(MediumPatrolState, EnemyStateEnterType.Null));
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void TransmitState(IEnemyState stateToTransmite)
    {
        switch (stateToTransmite)
        {
            // If an oter enemy tries to transmite research state
            case MediumResearchState mediumResearchState:
                // Check if enemy is in patrol state
                if (CurrentState is MediumPatrolState)
                {
                    StartCoroutine(ChangeState(MediumResearchState, EnemyStateEnterType.HasNoGoal));
                }
                break;
            // If an oter enemy tries to transmite alerte state
            case MediumAlerteState mediumAlerteState:
                // Check if enemy is in patrol or research state
                if (CurrentState is MediumPatrolState || CurrentState is MediumResearchState)
                {
                    StartCoroutine(ChangeState(MediumAlerteState, EnemyStateEnterType.HasNoGoal));
                }
                break;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_showGizmos)
        {
            if (Path == null || Path.Count == 0)
                return;

            Handles.color = _gizmoColor;
            Gizmos.color = _gizmoColor;

            if (PatrolType == PatrolType.Fixed)
            {
                Vector3 currentPos = Path[0].transform.position;

                Gizmos.DrawSphere(currentPos, 0.3f);
                Handles.Label(currentPos + Vector3.up * 0.5f, 1.ToString());
            }
            else
            {
                for (int i = 0; i < Path.Count; i++)
                {
                    if (Path[i] == null)
                        continue;

                    Vector3 currentPos = Path[i].transform.position;

                    Gizmos.DrawSphere(currentPos, 0.3f);
                    Handles.Label(currentPos + Vector3.up * 0.5f, (i + 1).ToString());

                    if (i < Path.Count - 1 && Path[i + 1] != null)
                    {
                        Vector3 nextPos = Path[i + 1].transform.position;
                        Gizmos.DrawLine(currentPos, nextPos);

                        DrawArrow(currentPos, nextPos);

                    }
                }

                if (PatrolType == PatrolType.LoopPatrol)
                {
                    Gizmos.DrawLine(Path[^1].transform.position, Path[0].transform.position);
                    DrawArrow(Path[^1].transform.position, Path[0].transform.position);
                }
            }
        }
    }

    /// <summary>
    /// Called to draw an arrow.
    /// </summary>
    /// <param name="from"> Start position. </param>
    /// <param name="to"> End position. </param>
    private void DrawArrow(Vector3 from, Vector3 to)
    {
        Vector3 direction = (to - from).normalized;
        Vector3 arrowPos = Vector3.Lerp(from, to, 0.5f);

        Handles.ArrowHandleCap(0, arrowPos, Quaternion.LookRotation(direction), 1f, EventType.Repaint);
    }
#endif
}
