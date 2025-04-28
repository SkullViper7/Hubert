using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    // Singleton
    private static StateManager _instance = null;
    public static StateManager Instance => _instance;

    /// <summary>
    /// Speed of the player when he walks normally.
    /// </summary>
    [field: SerializeField, Header("Default State")]
    public float WalkSpeed { get; private set; }

    /// <summary>
    /// Default state of the player.
    /// </summary>
    public DefaultState DefaultState { get; private set; } = new();

    /// <summary>
    /// Speed of the player when he crawls.
    /// </summary>
    [field: SerializeField, Space, Header("Crawling State")]
    public float CrawlSpeed { get; private set; }

    /// <summary>
    /// A value indicating if the player is crawling or not.
    /// </summary>
    public bool IsCrawling { get; set; }

    /// <summary>
    /// State where player is crawling.
    /// </summary>
    public CrawlingState CrawlingState { get; private set; } = new();

    /// <summary>
    /// Speed of the player when he is sticked.
    /// </summary>
    [field: SerializeField, Space, Header("Sticked State")]
    public float StickSpeed { get; private set; }

    /// <summary>
    /// Radius to check walls around.
    /// </summary>
    [SerializeField]
    private float _wallRadius;

    /// <summary>
    /// Wall on which the player is sticked.
    /// </summary>
    public BoxCollider StickedWall { get; private set; }

    /// <summary>
    /// Position on the wall where player must stick.
    /// </summary>
    public Vector3 StickedPosition { get; private set; }

    /// <summary>
    /// Normal of the wall on which the player is sticked.
    /// </summary>
    public Vector3 StickedNormal { get; private set; }

    /// <summary>
    /// A value indicating if the player is sticking on a wall or not.
    /// </summary>
    public bool IsSticking { get; set; }

    /// <summary>
    /// State where player is sticked on a wall.
    /// </summary>
    public StickedState StickedState { get; private set; } = new();

    /// <summary>
    /// Speed of the player when he aims.
    /// </summary>
    [field: SerializeField, Space, Header("Aiming State")]
    public float AimSpeed { get; private set; }

    /// <summary>
    /// Range of the aim of the player.
    /// </summary>
    [field: SerializeField]
    public float AimRange { get; private set; }

    /// <summary>
    /// Time during which the camera transitions to its highest position.
    /// </summary>
    [field: SerializeField]
    public float CameraUnzoomTime { get; private set; }

    /// <summary>
    /// Prefab of a bullet.
    /// </summary>
    [field: SerializeField]
    public GameObject BulletPrefab { get; private set; }

    /// <summary>
    /// Socket where bullets are instantiated.
    /// </summary>
    [field: SerializeField]
    public Transform BulletSocket { get; private set; }

    /// <summary>
    /// Speed of the bullet.
    /// </summary>
    [field: SerializeField]
    public float BulletSpeed { get; private set; }

    /// <summary>
    /// Minimum distance to consider the ball arrived.
    /// </summary>
    [field: SerializeField]
    public float HitThreshold { get; private set; }

    /// <summary>
    /// The cooldown duration of the shot.
    /// </summary>
    [field: SerializeField]
    public float ShotCooldownDuration { get; private set; }

    /// <summary>
    /// A value indicating if the player is aiming targets or not.
    /// </summary>
    public bool IsAiming { get; set; }

    /// <summary>
    /// A value indicating if the player is shooting or not.
    /// </summary>
    public bool IsShooting { get; set; }

    /// <summary>
    /// State where player is aiming on a target.
    /// </summary>
    public AimingState AimingState { get; private set; } = new();

    /// <summary>
    /// A value indicating if there is still a cooldown for the shot.
    /// </summary>
    private bool _isThereShotCooldown;

    /// <summary>
    /// Range where the player can hit an enemy;
    /// </summary>
    [SerializeField, Space, Header("Hitting State")]
    private float _hitRange;

    /// <summary>
    /// Angle in the back of the enemy where player must be to hit the enemy.
    /// </summary>
    [SerializeField]
    private float _enemyBackAngle;

    /// <summary>
    /// Angle in front of the player where enemy must be to be hit by the player.
    /// </summary>
    [SerializeField]
    private float _playerFrontAngleForHit;

    /// <summary>
    /// The enemy to hit.
    /// </summary>
    public Transform EnemyToHit { get; private set; }

    /// <summary>
    /// A value indicating if the player is hitting or not.
    /// </summary>
    public bool IsHitting { get; set; }

    /// <summary>
    /// State where player is hitting an enemy.
    /// </summary>
    public HitState HitState { get; private set; } = new();

    /// <summary>
    /// Range where the player can hide to a place;
    /// </summary>
    [SerializeField, Space, Header("Hidden State")]
    private float _hideRange;

    /// <summary>
    /// Angle in front of the player where hidden place must be.
    /// </summary>
    [SerializeField]
    private float _playerFrontAngleToHide;

    /// <summary>
    /// The place where player must hide.
    /// </summary>
    public HiddenPlace PlaceToHide { get; private set; }

    /// <summary>
    /// State where player is hidden.
    /// </summary>
    public HiddenState HiddenState { get; private set; } = new();

    /// <summary>
    /// A value indicating if the player is hidden or not.
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// A value to add smoothness to the movement.
    /// </summary>
    [field: SerializeField, Space, Header("General")]
    public float MoveSmoothness { get; private set; }

    /// <summary>
    /// Speed of the rotation of the player.
    /// </summary>
    [field: SerializeField]
    public float RotationSpeed { get; private set; }

    /// <summary>
    /// Force applied on the player to stick the ground.
    /// </summary>
    [field: SerializeField]
    public float GravityForce { get; private set; }

    /// <summary>
    /// Camera of the player.
    /// </summary>
    [field: SerializeField, Space, Header("Camera")]
    public CinemachineFreeLook Camera { get; private set; }

    /// <summary>
    /// Sensitivity of the mouse to look around the player.
    /// </summary>
    [field: SerializeField]
    public float MouseSensitivityX { get; private set; }

    /// <summary>
    /// Sensitivity of the mouse to zoom on the player.
    /// </summary>
    [field: SerializeField]
    public float MouseSensitivityY { get; private set; }

    /// <summary>
    /// Sensitivity of the gamepad to look around the player.
    /// </summary>
    [field: SerializeField]
    public float GamepadSensitivityX { get; private set; }

    /// <summary>
    /// Sensitivity of the gamepad to zoom on the player.
    /// </summary>
    [field: SerializeField]
    public float GamepadSensitivityY { get; private set; }

    /// <summary>
    /// A value to add smoothness to the zoom.
    /// </summary>
    [field: SerializeField]
    public float ZoomSmoothness { get; private set; }

    /// <summary>
    /// Targeted value of the Y axis.
    /// </summary>
    public float TargetYAxis { get; set; } = 1;

    /// <summary>
    /// The script which manages animations.
    /// </summary>
    [field: SerializeField, Space]
    public AnimationController AnimationController { get; private set; }

    /// <summary>
    /// Controller component of the player.
    /// </summary>
    public CharacterController CharacterController { get; private set; }

    /// <summary>
    /// Manager of the player inputs.
    /// </summary>
    public InputManager InputManager { get; private set; }

    /// <summary>
    /// The nav mesh agent of the player. 
    /// </summary>
    public NavMeshController NavMeshController { get; private set; }

    private IState _currentState;

    private void Awake()
    {
        // Singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _instance = this;
        }

        CharacterController = GetComponent<CharacterController>();

        InputManager = GetComponent<InputManager>();

        NavMeshController = GetComponent<NavMeshController>();
    }

    private void Start()
    {
        InputManager.OnCrawl += ManageCrawl;
        InputManager.OnStick += ManageStick;
        InputManager.OnAim += ManageAim;
        AnimationController.HasShot += ExitAim;
        InputManager.OnHit += ManageHit;
        AnimationController.HasHit += ExitHit;
        InputManager.OnHide += ManageHide;

        // Start with default state.
        StartCoroutine(ChangeState(DefaultState));
    }

    /// <summary>
    /// Called to execute the current state behaviour.
    /// </summary>
    private void Update()
    {
        _currentState?.UpdateState(this);
    }

    /// <summary>
    /// Called to switch to a new state.
    /// </summary>
    /// <param name="newState"> The new state to switch. </param>
    private IEnumerator ChangeState(IState newState)
    {
        if (_currentState != null)
            yield return StartCoroutine(_currentState.OnExit(this));

        _currentState = newState;

        if (_currentState != null)
            yield return StartCoroutine(_currentState.OnEnter(this));
    }

    #region Crawl
    /// <summary>
    /// Called to manage the crawl when the input is triggered.
    /// </summary>
    private void ManageCrawl()
    {
        if (StickedState.IsTransitioning || AimingState.IsShooting || IsHitting || HiddenState.IsTransitioning) return;

        if (IsCrawling)
        {
            StartCoroutine(ChangeState(DefaultState));
        }
        else
        {
            StartCoroutine(ChangeState(CrawlingState));
        }
    }
    #endregion

    #region Stick
    /// <summary>
    /// Called to manage the stick when the input is triggered.
    /// </summary>
    private void ManageStick()
    {
        if (StickedState.IsTransitioning || AimingState.IsShooting || IsHitting) return;

        if (IsSticking && !StickedState.IsTransitioning)
        {
            StickedWall = null;
            StickedPosition = Vector3.zero;
            StartCoroutine(ChangeState(DefaultState));
        }
        else if (!IsSticking && !StickedState.IsTransitioning)
        {
            // Check walls
            int wallLayerMask = LayerMask.GetMask("Wall");
            List<BoxCollider> walls = Physics.OverlapSphere(transform.position, _wallRadius, wallLayerMask).Where(c => c is BoxCollider).Select(c => c as BoxCollider).ToList();

            if (walls.Count == 0)
                return;

            walls = Utilities.SortWalls(walls, transform, CharacterController);

            if (walls.Count == 0)
                return;

            BoxCollider nearestWall = null;
            float minDistance = float.MaxValue;
            Vector3 closestPoint = Vector3.zero;

            for (int i = 0; i < walls.Count; i++)
            {
                Vector3 pointOnWall = walls[i].ClosestPoint(transform.position);
                float distance = Vector3.Distance(transform.position, pointOnWall);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestWall = walls[i];
                    closestPoint = pointOnWall;
                }
            }

            StickedWall = nearestWall;
            StickedNormal = Utilities.GetWallSide((transform.position - closestPoint).normalized, StickedWall.transform);
            StickedPosition = Utilities.GetCorrectPosition(closestPoint, StickedNormal, (BoxCollider)nearestWall, CharacterController);

            if (Utilities.IsWayClear(StickedWall, StickedPosition, transform, CharacterController))
            {
                StartCoroutine(ChangeState(StickedState));
            }
        }
    }
    #endregion

    #region Aim
    /// <summary>
    /// Called to manage the aim when the input is triggered.
    /// </summary>
    private void ManageAim()
    {
        if (StickedState.IsTransitioning || AimingState.IsShooting || _isThereShotCooldown || IsHitting || HiddenState.IsTransitioning) return;

        if (IsAiming)
        {
            StartCoroutine(ChangeState(DefaultState));
        }
        else
        {
            StartCoroutine(ChangeState(AimingState));
        }
    }

    /// <summary>
    /// Called to exit the aiming state.
    /// </summary>
    private void ExitAim()
    {
        StartCoroutine(ShotCooldown());
        StartCoroutine(ChangeState(DefaultState));
    }

    /// <summary>
    /// Called to wait before a new shot.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShotCooldown()
    {
        _isThereShotCooldown = true;
        yield return new WaitForSeconds(ShotCooldownDuration);
        _isThereShotCooldown = false;
    }
    #endregion

    #region Hit
    /// <summary>
    /// Called to manage the hit when the input is triggered.
    /// </summary>
    private void ManageHit()
    {
        if (StickedState.IsTransitioning || AimingState.IsShooting || IsHitting || HiddenState.IsTransitioning) return;

        // Get all enemies in the layer within a given radius
        List<Collider> enemiesAround = Physics.OverlapSphere(transform.position, _hitRange, LayerMask.GetMask("Enemy")).ToList();

        if (enemiesAround.Count == 0) return;

        Collider enemyToHit = Utilities.SortEnemiesForHit(enemiesAround, _playerFrontAngleForHit, _enemyBackAngle, transform);

        if (enemyToHit == null) return;

        EnemyToHit = enemyToHit.transform;

        StartCoroutine(ChangeState(HitState));
    }

    /// <summary>
    /// Called to exit the hitting state.
    /// </summary>
    private void ExitHit()
    {
        StartCoroutine(ChangeState(DefaultState));
    }
    #endregion

    #region Hide
    /// <summary>
    /// Called to manage the hide when the input is triggered.
    /// </summary>
    private void ManageHide()
    {
        if (StickedState.IsTransitioning || AimingState.IsShooting || IsHitting || HiddenState.IsTransitioning) return;

        if (IsHidden && !HiddenState.IsTransitioning)
        {
            PlaceToHide = null;
            StartCoroutine(ChangeState(DefaultState));
        }
        else if (!IsHidden && !HiddenState.IsTransitioning)
        {
            // Get all hidden places
            List<Collider> hiddenPlacesAround = Physics.OverlapSphere(transform.position, _hitRange, LayerMask.GetMask("HiddenPlace")).ToList();

            if (hiddenPlacesAround.Count == 0) return;

            Collider placeToHide = Utilities.SortPlacesToHide(hiddenPlacesAround, _playerFrontAngleToHide, transform);

            if (placeToHide == null) return;

            PlaceToHide = placeToHide.GetComponent<HiddenPlace>();

            StartCoroutine(ChangeState(HiddenState));
        }
    }
    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        int segments = 30;

        // Draw range
        Gizmos.color = Color.green;

        Vector3 LeftPoint = transform.position + Quaternion.AngleAxis(-_playerFrontAngleForHit / 2, transform.up) * transform.forward * _hitRange;
        Vector3 RightPoint = transform.position + Quaternion.AngleAxis(_playerFrontAngleForHit / 2, transform.up) * transform.forward * _hitRange;

        Gizmos.DrawLine(transform.position, LeftPoint);
        Gizmos.DrawLine(transform.position, RightPoint);

        // Draw horizontal circle of the sphere
        // Vision segment
        float angleStep = _playerFrontAngleForHit / segments;

        Vector3 firstPoint = LeftPoint;
        Vector3 previousPoint = firstPoint;

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i;
            Vector3 nextPoint = transform.position + Quaternion.AngleAxis(angle, transform.up) * (LeftPoint - transform.position).normalized * _hitRange;
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }

        // Not in vision segment
        Gizmos.color = Color.red;

        angleStep = (360 - _playerFrontAngleForHit) / segments;

        firstPoint = RightPoint;
        previousPoint = firstPoint;

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i;
            Vector3 nextPoint = transform.position + Quaternion.AngleAxis(angle, transform.up) * (RightPoint - transform.position).normalized * _hitRange;
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
#endif
}