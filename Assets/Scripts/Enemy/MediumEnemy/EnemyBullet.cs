using System.Collections;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float _speed;

    private float _lifeTime = 30f;

    public void InitBullet(float speed)
    {
        _speed = speed;

        StartCoroutine(LifeTime());
    }

    private void Update()
    {
        // Move forward
        transform.position += _speed * Time.deltaTime * transform.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (other.TryGetComponent<PlayerStateManager>(out PlayerStateManager playerStateManager))
            {
                playerStateManager.Death();
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called to destroy the bullet after a delay.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(_lifeTime);

        Destroy(gameObject);
    }
}
