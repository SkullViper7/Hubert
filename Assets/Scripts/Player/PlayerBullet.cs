using System;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public event Action OnTargetShot;

    private EnemyBrain _enemyTargeted;

    private Transform _target;

    private float _speed;

    public void InitBullet(EnemyBrain enemyTargeted, Transform target, float speed)
    {
        _enemyTargeted = enemyTargeted;
        _speed = speed;
        _target = target;

        transform.up = _target.position - transform.position;
    }

    private void Update()
    {
        if (_target == null) return;

        // Calculate the direction to the target
        Vector3 direction = (_target.position - transform.position).normalized;

        // Move to the target
        transform.position += direction * _speed * Time.deltaTime;

        // Aligne the UP axis towards the target
        transform.up = direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTargetShot?.Invoke();
        _enemyTargeted.Death(EnemyStateEnterType.IsShot);
        Destroy(gameObject);
    }
}
