using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform _target;

    private float _speed;

    private float _hitThreshold;

    public void InitBullet(Transform target, float speed, float hitThreshold)
    {
        _speed = speed;
        _target = target;
        _hitThreshold = hitThreshold;
    }

    private void Update()
    {
        if (_target == null) return;

        // Calcul de la direction vers la cible
        Vector3 direction = (_target.position - transform.position).normalized;

        // Déplacement vers la cible
        transform.position += direction * _speed * Time.deltaTime;

        // Orientation : aligne l'axe UP vers la cible
        transform.up = direction;

        // Vérifie si la balle est proche de la cible
        if (Vector3.Distance(transform.position, _target.position) <= _hitThreshold)
        {
            Debug.Log("hit"); // Déclenche l'événement
            Destroy(gameObject); // Détruit la balle
        }
    }
}
