using UnityEngine;

public class MediumEnemyAnimationController : MonoBehaviour
{
    /// <summary>
    /// Animator component of the player
    /// </summary>
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
}
