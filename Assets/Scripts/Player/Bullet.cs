using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public event Action OnTargetShot;

    private Transform _target;

    private float _speed;

    private float _hitThreshold;

    public void InitBullet(Transform target, float speed, float hitThreshold)
    {
        _speed = speed;
        _target = target;
        _hitThreshold = hitThreshold;

        transform.up = _target.position - transform.position;

        GetComponent<MeshRenderer>().enabled = true;
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

        // Check if the ball is close to the target
        if (Vector3.Distance(transform.position, _target.position) <= _hitThreshold)
        {
            OnTargetShot?.Invoke();
            _target.GetComponent<EnemyBrain>().Death(EnemyStateEnterType.IsShot);
            Destroy(gameObject);
        }
    }
}
