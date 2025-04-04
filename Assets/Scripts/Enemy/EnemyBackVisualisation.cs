using UnityEngine;

public class EnemyBackVisualisation : MonoBehaviour
{
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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        int segments = 30;

        // Draw range
        Gizmos.color = Color.green;

        Vector3 LeftPoint = transform.position + Quaternion.AngleAxis(-_enemyBackAngle / 2, transform.up) * -transform.forward * _hitRange;
        Vector3 RightPoint = transform.position + Quaternion.AngleAxis(_enemyBackAngle / 2, transform.up) * -transform.forward * _hitRange;

        Gizmos.DrawLine(transform.position, LeftPoint);
        Gizmos.DrawLine(transform.position, RightPoint);

        // Draw horizontal circle of the sphere
        // Vision segment
        float angleStep = _enemyBackAngle / segments;

        Vector3 firstPoint = LeftPoint;
        Vector3 previousPoint = firstPoint;

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i;
            Vector3 nextPoint = transform.position + Quaternion.AngleAxis(angle, transform.up) * (LeftPoint - transform.position).normalized * _hitRange;
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }

        // Not in vision segment
        Gizmos.color = Color.red;

        angleStep = (360 - _enemyBackAngle) / segments;

        firstPoint = RightPoint;
        previousPoint = firstPoint;

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i;
            Vector3 nextPoint = transform.position + Quaternion.AngleAxis(angle, transform.up) * (RightPoint - transform.position).normalized * _hitRange;
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
#endif
}
