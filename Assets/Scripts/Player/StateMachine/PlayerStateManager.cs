using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    #region General
    /// <summary>
    /// A value to add smoothness to the movement.
    /// </summary>
    [field: SerializeField, Header("General")]
    public float MoveSmoothness { get; private set; }

    /// <summary>
    /// Force applied on the player to stick the ground.
    /// </summary>
    [field: SerializeField]
    public float GravityForce { get; private set; }

    /// <summary>
    /// The script which manages animations.
    /// </summary>
    [field: SerializeField]
    public PlayerAnimationController AnimationController { get; private set; }

    /// <summary>
    /// A value indicating if the player is dead.
    /// </summary>
    public bool IsDead { get; set; }

    /// <summary>
    /// An event to indicate that the player is dead.
    /// </summary>
    public event Action OnDeath;

    /// <summary>
    /// State where player is dead.
    /// </summary>
    private readonly DeadState _deadState = new();

    /// <summary>
    /// The current room in which player is.
    /// </summary>
    public Room CurrentRoom { get; private set; }

    /// <summary>
    /// An event to indicate that the current room has changed. (first room, is the old, second is the new)
    /// </summary>
    public event Action<Room, Room> OnRoomChanged;

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

    /// <summary>
    /// The current state of the player.
    /// </summary>
    private IPlayerState _currentState;

    /// <summary>
    /// A value indicating that the player is already changing to a new state.
    /// </summary>
    private bool _isAlreadyChangingState;
    #endregion

    #region Default
    /// <summary>
    /// Speed of the player when he walks in default state.
    /// </summary>
    [field: SerializeField, Space, Header("Default State")]
    public float DefaultWalkSpeed { get; private set; }

    /// <summary>
    /// Rotation speed of the player when he rotates in default state.
    /// </summary>
    [field: SerializeField]
    public float DefaultRotationSpeed { get; private set; }

    /// <summary>
    /// Range of the sound emitted by the walk.
    /// </summary>
    [field: SerializeField]
    public float WalkSoundRange { get; private set; }

    /// <summary>
    /// Default state of the player.
    /// </summary>
    public DefaultState DefaultState { get; private set; } = new();
    #endregion

    #region Crawl
    /// <summary>
    /// Speed of the player when he crawls.
    /// </summary>
    [field: SerializeField, Space, Header("Crawling State")]
    public float CrawlSpeed { get; private set; }

    /// <summary>
    /// Rotation speed of the player when he rotates in crawling state.
    /// </summary>
    [field: SerializeField]
    public float CrawlRotationSpeed { get; private set; }

    /// <summary>
    /// A value indicating if the player is crawling or not.
    /// </summary>
    public bool IsCrawling { get; set; }

    /// <summary>
    /// State where player is crawling.
    /// </summary>
    public CrawlingState CrawlingState { get; private set; } = new();
    #endregion

    #region Stick
    /// <summary>
    /// Speed of the player when he is sticked.
    /// </summary>
    [field: SerializeField, Space, Header("Sticked State")]
    public float StickSpeed { get; private set; }

    /// <summary>
    /// Speed of the player when he transitions to sticked state.
    /// </summary>
    [field: SerializeField]
    public float StickedTransitionInSpeed { get; private set; }

    /// <summary>
    /// Acceleration of the player when he transitions to sticked state.
    /// </summary>
    [field: SerializeField]
    public float StickedTransitionInAcceleration { get; private set; }

    /// <summary>
    /// Rotation speed of the player when he transitions to sticked state.
    /// </summary>
    [field: SerializeField]
    public float StickedTransitionInRotationSpeed { get; private set; }

    /// <summary>
    /// Speed of the player when he transitions out sticked state.
    /// </summary>
    [field: SerializeField]
    public float StickedTransitionOutSpeed { get; private set; }

    /// <summary>
    /// Acceleration of the player when he transitions out sticked state.
    /// </summary>
    [field: SerializeField]
    public float StickedTransitionOutAcceleration { get; private set; }

    /// <summary>
    /// Rotation speed of the player when he transitions out sticked state.
    /// </summary>
    [field: SerializeField]
    public float StickedTransitionOutRotationSpeed { get; private set; }

    /// <summary>
    /// The amount of time the player can hold their breath.
    /// </summary>
    [field: SerializeField]
    public float HoldBreathTime { get; private set; }

    /// <summary>
    /// The cooldown after out breathing.
    /// </summary>
    [field: SerializeField]
    public float OutOfBreathCooldown { get; private set; }

    /// <summary>
    /// The manager of the arm IK.
    /// </summary>
    [field: SerializeField]
    public ArmIKManager ArmIKManager { get; private set; }

    /// <summary>
    /// The material of the player when he is sticked and holding breath.
    /// </summary>
    [field: SerializeField]
    public Material RedHeadMaterial { get; private set; }

    /// <summary>
    /// The material of the player when he is sticked and not holding breath.
    /// </summary>
    [field: SerializeField]
    public Material RedTrunkMaterial { get; private set; }

    /// <summary>
    /// The renderer of the player.
    /// </summary>
    [field: SerializeField]
    public Renderer PlayerRenderer;

    /// <summary>
    /// The materials of the player.
    /// </summary>
    [field: SerializeField]
    public List<Material> PlayerMaterials;

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
    #endregion

    #region Aim
    public event Action OnShootCooldownEnded;

    /// <summary>
    /// Speed of the player when he aims.
    /// </summary>
    [field: SerializeField, Space, Header("Aiming State")]
    public float AimSpeed { get; private set; }

    /// <summary>
    /// Rotation speed of the player when he rotates in aiming state.
    /// </summary>
    [field: SerializeField]
    public float AimRotationSpeed { get; private set; }

    /// <summary>
    /// Rotation speed of the player when he rotates to the target.
    /// </summary>
    [field: SerializeField]
    public float ShootingRotationSpeed { get; private set; }

    /// <summary>
    /// Range of the aim of the player.
    /// </summary>
    [field: SerializeField]
    public float AimRange { get; private set; }

    /// <summary>
    /// Range of the sound emitted by the shot.
    /// </summary>
    [field: SerializeField]
    public float ShotSoundRange { get; private set; }

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
    /// Transform of the head.
    /// </summary>
    [field: SerializeField]
    public Transform Head { get; private set; }

    /// <summary>
    /// Speed of the bullet.
    /// </summary>
    [field: SerializeField]
    public float BulletSpeed { get; private set; }

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
    public PlayerAimingState AimingState { get; private set; } = new();

    /// <summary>
    /// A value indicating if there is still a cooldown for the shot.
    /// </summary>
    private bool _isThereShotCooldown;
    #endregion

    #region Hit
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
    /// Range of the sound emitted by the hit.
    /// </summary>
    [field: SerializeField]
    public float HitSoundRange { get; private set; }

    /// <summary>
    /// Speed of the player when he transitions to hit.
    /// </summary>
    [field: SerializeField]
    public float HitTransitionSpeed { get; private set; }

    /// <summary>
    /// Acceleration of the player when he transitions to hit.
    /// </summary>
    [field: SerializeField]
    public float HitTransitionAcceleration { get; private set; }

    /// <summary>
    /// Rotation speed of the player when he transitions to hit.
    /// </summary>
    [field: SerializeField]
    public float HitTransitionRotationSpeed { get; private set; }

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
    #endregion

    #region Hide
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
    /// Speed of the player when he transitions to hidden state.
    /// </summary>
    [field: SerializeField]
    public float HideTransitionInSpeed { get; private set; }

    /// <summary>
    /// Acceleration of the player when he transitions to hidden state.
    /// </summary>
    [field: SerializeField]
    public float HideTransitionInAcceleration { get; private set; }

    /// <summary>
    /// Rotation speed of the player when he transitions to hidden state.
    /// </summary>
    [field: SerializeField]
    public float HideTransitionInRotationSpeed { get; private set; }

    /// <summary>
    /// Speed of the player when he transitions out hidden state.
    /// </summary>
    [field: SerializeField]
    public float HideTransitionOutSpeed { get; private set; }

    /// <summary>
    /// Acceleration of the player when he transitions out hidden state.
    /// </summary>
    [field: SerializeField]
    public float HideTransitionOutAcceleration { get; private set; }

    /// <summary>
    /// Rotation speed of the player when he transitions out hidden state.
    /// </summary>
    [field: SerializeField]
    public float HideTransitionOutRotationSpeed { get; private set; }

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
    #endregion

    #region Camera
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
    #endregion

    #region Vase
    /// <summary>
    /// When the player gets the vase
    /// </summary>
    public bool HasVase { get; set; }
    #endregion

    #region Gizmos
    /// <summary>
    /// A value to show gizmos.
    /// </summary>
    [Space, SerializeField]
    private bool _showGizmos = true;
    #endregion

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();

        InputManager = GetComponent<InputManager>();

        NavMeshController = GetComponent<NavMeshController>();
    }

    private void Start()
    {
        InputManager.OnCrawl += ManageCrawl;
        InputManager.OnStick += ManageStick;
        InputManager.OnAim += ManageAim;
        AnimationController.OnShot += ExitAim;
        InputManager.OnHit += ManageHit;
        AnimationController.OnHit += ExitHit;
        InputManager.OnHide += ManageHide;

        PlayerMaterials = PlayerRenderer.materials.ToList();

        // Start with default state.
        StartCoroutine(ChangeState(DefaultState));
    }

    /// <summary>
    /// Called to execute the current state behaviour.
    /// </summary>
    private void FixedUpdate()
    {
        _currentState?.UpdateState();
    }

    /// <summary>
    /// Called to switch to a new state.
    /// </summary>
    /// <param name="newState"> The new state to switch. </param>
    private IEnumerator ChangeState(IPlayerState newState)
    {
        if (!_isAlreadyChangingState)
        {
            _isAlreadyChangingState = true;
            if (_currentState != null)
                yield return StartCoroutine(_currentState.OnExit());

            _currentState = newState;

            if (_currentState != null)
                yield return StartCoroutine(_currentState.OnEnter(this));
            _isAlreadyChangingState = false;
        }
    }

    /// <summary>
    /// Called to cancel any state and return to default state.
    /// </summary>
    public IEnumerator ResetCurrentState()
    {
        _isAlreadyChangingState = true;
        CancelCurrentState();

        _currentState = DefaultState;
        _isAlreadyChangingState = false;
        yield return StartCoroutine(_currentState.OnEnter(this));
    }

    /// <summary>
    /// Called to cancel any state and return to default state.
    /// </summary>
    public void CancelCurrentState()
    {
        _currentState.CancelState();
    }

    /// <summary>
    /// Called to indicate to the player that he is in a new room.
    /// </summary>
    /// <param name="newRoom"> The new room. </param>
    public void IsInNewRoom(Room newRoom)
    {
        OnRoomChanged?.Invoke(CurrentRoom, newRoom);
        CurrentRoom = newRoom;
    }

    #region Crawl
    /// <summary>
    /// Called to manage the crawl when the input is triggered.
    /// </summary>
    private void ManageCrawl()
    {
        if (_isAlreadyChangingState || StickedState.IsTransitioning || StickedState.IsOutOfBreath || AimingState.IsShooting || IsHitting || HiddenState.IsTransitioning || IsDead) return;

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
        if (_isAlreadyChangingState || StickedState.IsTransitioning || StickedState.IsOutOfBreath || AimingState.IsShooting || IsHitting || IsDead) return;

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

            StartCoroutine(ChangeState(StickedState));
            //if (Utilities.IsWayClear(StickedWall, StickedPosition, transform, CharacterController))
            //{
            //    StartCoroutine(ChangeState(StickedState));
            //}
        }
    }
    #endregion

    #region Aim
    /// <summary>
    /// Called to manage the aim when the input is triggered.
    /// </summary>
    private void ManageAim(bool isStarted)
    {
        if (_isAlreadyChangingState || StickedState.IsTransitioning || StickedState.IsOutOfBreath || AimingState.IsShooting || _isThereShotCooldown || IsHitting || HiddenState.IsTransitioning || IsDead) return;

        if (!isStarted && IsAiming)
        {
            StartCoroutine(ChangeState(DefaultState));
        }
        else if (isStarted && !IsAiming)
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

        float currentRedValue = 0f;
        float startRedValue = PlayerMaterials.FirstOrDefault(m => m.name.Contains("RedTrunk")).GetFloat("_Height");

        float duration = ShotCooldownDuration;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            currentRedValue = Mathf.Lerp(startRedValue, -3f, elapsed / duration);
            PlayerMaterials.FirstOrDefault(m => m.name.Contains("RedTrunk")).SetFloat("_Height", currentRedValue);
            PlayerRenderer.materials = PlayerMaterials.ToArray();

            elapsed += Time.deltaTime;
            yield return null;
        }

        OnShootCooldownEnded?.Invoke();
        PlayerMaterials.FirstOrDefault(m => m.name.Contains("RedTrunk")).SetFloat("_Height", 0f);
        PlayerMaterials.Remove(RedTrunkMaterial);
        PlayerRenderer.materials = PlayerMaterials.ToArray();
        _isThereShotCooldown = false;
    }
    #endregion

    #region Hit
    /// <summary>
    /// Called to manage the hit when the input is triggered.
    /// </summary>
    private void ManageHit()
    {
        if (_isAlreadyChangingState || StickedState.IsTransitioning || StickedState.IsOutOfBreath || AimingState.IsShooting || IsHitting || HiddenState.IsTransitioning || IsDead) return;

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
        if (_isAlreadyChangingState || StickedState.IsTransitioning || StickedState.IsOutOfBreath || AimingState.IsShooting || IsHitting || HiddenState.IsTransitioning || IsDead) return;

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

    #region Death
    /// <summary>
    /// Called to death.
    /// </summary>
    public void Death()
    {
        if (IsDead) return;

        IsDead = true;
        CancelCurrentState();

        _currentState = _deadState;

        AnimationController.OnDead += () => OnDeath?.Invoke();

        StartCoroutine(_currentState.OnEnter(this));
    }
    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_showGizmos)
        {
            // Draw shot sound radius
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, ShotSoundRange);

            Handles.Label(transform.position + transform.forward * ShotSoundRange, "Shot Sound Radius");
            Handles.Label(transform.position - transform.forward * ShotSoundRange, "Shot Sound Radius");
            Handles.Label(transform.position + transform.right * ShotSoundRange, "Shot Sound Radius");
            Handles.Label(transform.position - transform.right * ShotSoundRange, "Shot Sound Radius");

            // Draw hit sound radius
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, HitSoundRange);

            Handles.Label(transform.position + transform.forward * HitSoundRange, "Hit Sound Radius");
            Handles.Label(transform.position - transform.forward * HitSoundRange, "Hit Sound Radius");
            Handles.Label(transform.position + transform.right * HitSoundRange, "Hit Sound Radius");
            Handles.Label(transform.position - transform.right * HitSoundRange, "Hit Sound Radius");

            // Draw walk sound radius
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, WalkSoundRange);

            Handles.Label(transform.position + transform.forward * WalkSoundRange, "Walk Sound Radius");
            Handles.Label(transform.position - transform.forward * WalkSoundRange, "Walk Sound Radius");
            Handles.Label(transform.position + transform.right * WalkSoundRange, "Walk Sound Radius");
            Handles.Label(transform.position - transform.right * WalkSoundRange, "Walk Sound Radius");
        }
    }
#endif
}