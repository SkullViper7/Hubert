using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    /// <summary>
    /// A reference to the player.
    /// </summary>
    private PlayerStateManager _player;

    /// <summary>
    /// Transform of the targeted enemy.
    /// </summary>
    private Transform _targetedEnemy;

    /// <summary>
    /// Image of the target.
    /// </summary>
    private Image _image;

    /// <summary>
    /// Animator of the target.
    /// </summary>
    private Animator _animator;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.Instance.OnPlayerInstanciated += (PlayerStateManager player) =>
        {
            _player = player;
            InitListeners(_player);
        };
    }

    /// <summary>
    /// Called to init all listeners.
    /// </summary>
    /// <param name="player"> The reference to the player. </param>
    private void InitListeners(PlayerStateManager player)
    {
        player.AimingState.OnNewEnemyTargeted += InitTarget;
        player.AimingState.OnAimStop += StopTarget;
        player.AimingState.OnTargetEleminated += StopTarget;
    }

    public void InitTarget(EnemyBrain targetedEnemy)
    {
        if (targetedEnemy != null)
        {
            _targetedEnemy = targetedEnemy.TargetTransform;
            transform.position = Camera.main.WorldToScreenPoint(_targetedEnemy.position);
            _image.enabled = true;
            _animator.SetTrigger("AimLock");
        }
        else
        {
            StopTarget();
        }
    }

    public void StopTarget()
    {
        _image.enabled = false;
        _targetedEnemy = null;
    }

    private void Update()
    {
        if (_targetedEnemy != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(_targetedEnemy.position);
        }
    }
}
