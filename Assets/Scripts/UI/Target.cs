using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    private Transform _targetedEnemy;

    private Image _image;

    private Animator _animator;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        PlayerStateManager.Instance.AimingState.OnNewEnemyTargeted += InitTarget;
        PlayerStateManager.Instance.AimingState.OnAimStop += StopTarget;
        PlayerStateManager.Instance.AimingState.OnTargetEleminated += StopTarget;
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
