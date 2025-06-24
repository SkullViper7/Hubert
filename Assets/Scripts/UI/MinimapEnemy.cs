using UnityEngine;

public class MinimapEnemy : MonoBehaviour
{
    /// <summary>
    /// Brain of the enemy.
    /// </summary>
    [SerializeField]
    private EnemyBrain _enemyBrain;

    /// <summary>
    /// Animator of the minimap enemy.
    /// </summary>
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _animator.speed = 1;

        _enemyBrain.OnAlerteLevelChanged += ChangeAnimationSpeed;
    }

    /// <summary>
    /// Called to change the speed of the animation depending of the alerte level.
    /// </summary>
    /// <param name="oldAlerteLevel"> Old alerte level of the enemy. </param>
    /// <param name="newAlerteLevel"> New alerte level of the enemy. </param>
    private void ChangeAnimationSpeed(AlerteLevel oldAlerteLevel, AlerteLevel newAlerteLevel)
    {
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
}
