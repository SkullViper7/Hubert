using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SoundEmitter))]
public class BreakableObject : MonoBehaviour
{
    /// <summary>
    /// The game object not break.
    /// </summary>
    [SerializeField, Space, Header("Object")]
    private GameObject _fullObject;

    /// <summary>
    /// List of all fragments of the object.
    /// </summary>
    [SerializeField]
    private List<Rigidbody> _fragments;

    /// <summary>
    /// Treshold at which the object explode.
    /// </summary>
    [SerializeField]
    private float _breakForceThreshold;

    /// <summary>
    /// Multiplier added to the force to make the explosion more impressive.
    /// </summary>
    [SerializeField]
    private float _explosionForceMultiplier;

    /// <summary>
    /// Radius of the explosion.
    /// </summary>
    [SerializeField]
    private float _explosionRadius;

    /// <summary>
    /// Life time of a fragment before the explosion.
    /// </summary>
    [SerializeField]
    private float _fragmentLifetime;

    /// <summary>
    /// Time during which the fragment gradually disappears.
    /// </summary>
    [SerializeField]
    private float _vanishTime;

    /// <summary>
    /// Rigidbody of the object.
    /// </summary>
    private Rigidbody _rigidbody;

    /// <summary>
    /// Collider of the object.
    /// </summary>
    private Collider _collider;

    /// <summary>
    /// Navmesh obstacle component.
    /// </summary>
    private NavMeshObstacle _obstacle;

    /// <summary>
    /// A value indicating if the last entity who pushes the object is an enemy.
    /// </summary>
    [SerializeField]
    private bool _isPushedByAnEnemy;

    /// <summary>
    /// Radius of the sound when the object explodes.
    /// </summary>
    [SerializeField, Space, Header("Audio")]
    private float _soundRadius;

    /// <summary>
    /// Sfx played when it breaks.
    /// </summary>
    [SerializeField]
    private AudioClip _breakSFX;

    /// <summary>
    /// Audio source of the object.
    /// </summary>
    private AudioSource _audioSource;

    /// <summary>
    /// Component which emites the sound.
    /// </summary>
    private SoundEmitter _soundEmitter;

    /// <summary>
    /// VFX played when it breaks.
    /// </summary>
    [SerializeField, Space, Header("Visual")]
    private GameObject _vfx;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _obstacle = GetComponent<NavMeshObstacle>();
        _soundEmitter = GetComponent<SoundEmitter>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        LayerMask layerMask = collision.gameObject.layer;

        if (layerMask == LayerMask.NameToLayer("Player") || layerMask == LayerMask.NameToLayer("Breakable") || layerMask == LayerMask.NameToLayer("PushableObject"))
        {
            if (_obstacle != null)
            {
                _obstacle.enabled = false;
            }

            _rigidbody.isKinematic = false;
        }

        if (layerMask == LayerMask.NameToLayer("Enemy"))
        {
            _isPushedByAnEnemy = true;
        }
        else if (layerMask == LayerMask.NameToLayer("Player") || layerMask == LayerMask.NameToLayer("Breakable") || layerMask == LayerMask.NameToLayer("PushableObject"))
        {
            _isPushedByAnEnemy = false;
        }

        float impactForce = collision.relativeVelocity.magnitude;

        if (impactForce >= _breakForceThreshold)
        {
            Explosion(collision.contacts[0].point, impactForce * _explosionForceMultiplier);
        }
    }

    /// <summary>
    /// Called to explode the object.
    /// </summary>
    /// <param name="position"> Position of the explosion. </param>
    /// <param name="explosionForce"> Force of the explosion. </param>
    private void Explosion(Vector3 position, float explosionForce)
    {
        if (_obstacle != null)
        {
            _obstacle.enabled = false;
        }
        _fullObject.SetActive(false);
        _collider.enabled = false;
        _rigidbody.isKinematic = true;

        for (int i = 0; i < _fragments.Count; ++i)
        {
            _fragments[i].gameObject.SetActive(true);
            _fragments[i].isKinematic = false;
            _fragments[i].AddExplosionForce(explosionForce, position, _explosionRadius);
        }

        _soundEmitter.EmitSound(transform.position, _soundRadius, SoundType.OneShot, _isPushedByAnEnemy);

        if (_vfx != null)
        {
            _vfx.SetActive(true);
        }

        _audioSource.PlayOneShot(_breakSFX);

        StartCoroutine(Vanish(_fragmentLifetime));
    }

    /// <summary>
    /// Called to 
    /// </summary>
    /// <param name="waitingTime"></param>
    /// <returns></returns>
    private IEnumerator Vanish(float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);

        for (int i = 0; i < _fragments.Count; i++)
        {
            yield return new WaitForSeconds(_vanishTime);

            if (_fragments[i].TryGetComponent<Animator>(out Animator animator))
            {
                animator.SetBool("IsVanishing", true);
            }
        }

        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }
}
