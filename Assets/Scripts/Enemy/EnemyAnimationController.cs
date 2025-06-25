using System;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyAnimationController : MonoBehaviour
{
    /// <summary>
    /// Animator component of the player.
    /// </summary>
    [SerializeField]
    protected Animator _animator;

    /// <summary>
    /// VFX to play when the enemy is alerted.
    /// </summary>
    [SerializeField] private VisualEffect _questionVFX;

    /// <summary>
    /// VFX to play when the enemy is chasing.
    /// </summary>
    [SerializeField] private VisualEffect _exclamationVFX;

    /// <summary>
    /// Brain of the enemy.
    /// </summary>
    private EnemyBrain _enemyBrain;

    /// <summary>
    /// Event triggered at the end of some aniamtion.
    /// </summary>
    public event Action OnFinishToLookAround, OnFinishAstonishment;

    private void Awake()
    {
        _enemyBrain = GetComponentInParent<EnemyBrain>();

        _enemyBrain.OnQuestion += PlayQuestionVFX;
        _enemyBrain.OnExclamation += PlayExclamationVFX;
    }

    public void SetWalkSpeed(float speed)
    {
        _animator.SetFloat("Speed", speed);
    }

    public void PlayLookAroundAnim(string trigger)
    {
        _animator.SetTrigger(trigger);
        _animator.Update(0);
    }

    public void HasFinishedToLookAround()
    {
        OnFinishToLookAround?.Invoke();
    }

    public void PlayAstonishmentAnim(string trigger)
    {
        _animator.SetTrigger(trigger);
        _animator.Update(0);
    }

    public void HasFinishedAstonishment()
    {
        OnFinishAstonishment?.Invoke();
    }

    private void PlayQuestionVFX()
    {
        _questionVFX.Play();
    }

    private void PlayExclamationVFX()
    {
        _exclamationVFX.Play();
    }
}
