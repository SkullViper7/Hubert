using UnityEngine;

public class MinimapFOV : MonoBehaviour
{
    /// <summary>
    /// Brain of the enemy.
    /// </summary>
    private EnemyBrain _enemyBrain;

    /// <summary>
    /// Vision of the camera.
    /// </summary>
    private EnemyVision _enemyVision;

    /// <summary>
    /// Animator of the minimap FOV.
    /// </summary>
    private Animator _animator;

    /// <summary>
    /// A value to indicate that the object is destroyed.
    /// </summary>
    private bool _isDestroyed;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _animator.speed = 1;
    }

    private void OnDestroy()
    {
        _isDestroyed = true;
    }

    public void InitForEnemy(EnemyBrain enemyBrain)
    {
        _enemyBrain = enemyBrain;
        _enemyBrain.OnAlerteLevelChanged += ChangeAnimationSpeedForEnemy;
    }

    public void InitForCamera(EnemyVision enemyVision)
    {
        _enemyVision = enemyVision;
        _enemyVision.OnPlayerSeen += ChangeAnimationSpeedForCamera;
    }

    /// <summary>
    /// Called to change the speed of the animation depending of the alerte level.
    /// </summary>
    /// <param name="oldAlerteLevel"> Old alerte level of the enemy. </param>
    /// <param name="newAlerteLevel"> New alerte level of the enemy. </param>
    private void ChangeAnimationSpeedForEnemy(AlerteLevel oldAlerteLevel, AlerteLevel newAlerteLevel)
    {
        if (_isDestroyed) return;

        switch (newAlerteLevel)
        {
            case AlerteLevel.Patrol:
                _animator.speed = 1;
                break;
            case AlerteLevel.Research:
                _animator.speed = 2f;
                break;
            case AlerteLevel.Alerte:
                _animator.speed = 3f;
                break;
        }
    }

    /// <summary>
    /// Called to change the speed of the animation depending of the alerte level.
    /// </summary>
    /// <param name="context"> The context of the vision. </param>
    private void ChangeAnimationSpeedForCamera(Vector3 ignore, PlayerSeenContext context)
    {
        if (_isDestroyed) return;

        if (context == PlayerSeenContext.FirstTime || context == PlayerSeenContext.Continue)
        {
            _animator.speed = 3f;
        }
        else if (context == PlayerSeenContext.LastTime)
        {
            _animator.speed = 1;
        }
    }
}
