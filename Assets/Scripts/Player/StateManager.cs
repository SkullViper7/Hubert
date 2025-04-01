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
    /// Time during which the player transitions to sticked state.
    /// </summary>
    [field: SerializeField]
    public float TransitionTime { get; private set; }

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
    /// Time during which the player rotats to the direction of the target.
    /// </summary>
    [field: SerializeField]
    public float RotationTimebeforeShoot { get; private set; }

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

    private IState _currentState;

    //private HiddenState _hiddenState = new();

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
    }

    private void Start()
    {
        InputManager.OnCrawl += ManageCrawl;
        InputManager.OnStick += ManageStick;
        InputManager.OnAim += ManageAim;
        AnimationController.HasShot += ExitAim;

        // Start with default state.
        ChangeState(DefaultState);
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
    private void ChangeState(IState newState)
    {
        _currentState?.OnExit(this);

        _currentState = newState;
        _currentState?.OnEnter(this);
    }

    /// <summary>
    /// Called to manage the crawl when the input is triggered.
    /// </summary>
    private void ManageCrawl()
    {
        if (StickedState.IsTransitioning || AimingState.IsShooting) return;

        if (IsCrawling)
        {
            ChangeState(DefaultState);
        }
        else
        {
            ChangeState(CrawlingState);
        }
    }

    /// <summary>
    /// Called to manage the stick when the input is triggered.
    /// </summary>
    private void ManageStick()
    {
        if (StickedState.IsTransitioning || AimingState.IsShooting) return;

        if (IsSticking && !StickedState.IsTransitioning)
        {
            StickedWall = null;
            StickedPosition = Vector3.zero;
            ChangeState(DefaultState);
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
            StickedNormal = Utilities.GetCorrectedNormal((transform.position - closestPoint).normalized, StickedWall.transform);
            StickedPosition = Utilities.GetCorrectPosition(closestPoint + StickedNormal * CharacterController.radius, StickedNormal, (BoxCollider)nearestWall, CharacterController);

            if (Utilities.IsWayClear(StickedWall, StickedPosition, transform, CharacterController))
            {
                ChangeState(StickedState);
            }
        }
    }

    /// <summary>
    /// Called to manage the aim when the input is triggered.
    /// </summary>
    private void ManageAim()
    {
        if (StickedState.IsTransitioning || AimingState.IsShooting) return;

        if (IsAiming)
        {
            ChangeState(DefaultState);
        }
        else
        {
            ChangeState(AimingState);
        }
    }

    /// <summary>
    /// Called to exit the aiming state.
    /// </summary>
    private void ExitAim()
    {
        ChangeState(DefaultState);
    }
}
