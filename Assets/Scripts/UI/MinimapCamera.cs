using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    /// <summary>
    /// Vision of the camera.
    /// </summary>
    [SerializeField]
    private EnemyVision _enemyVision;

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

        _enemyVision.OnPlayerSeen += ChangeAnimationSpeedForCamera;
    }


    /// <summary>
    /// Called to change the speed of the animation depending of the alerte level.
    /// </summary>
    /// <param name="context"> The context of the vision. </param>
    private void ChangeAnimationSpeedForCamera(Vector3 ignore, PlayerSeenContext context)
    {
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
