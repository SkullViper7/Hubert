using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] GameObject _vfx;

    /// <summary>
    /// Radius of the sound when the object explodes.
    /// </summary>
    [SerializeField, Space, Header("Audio")] 
    private float _soundRadius;
    [SerializeField] private AudioClip _breakSFX;
    private AudioSource _audioSource;
    

    /// <summary>
    /// Component which emites the sound.
    /// </summary>
    private SoundEmitter _soundEmitter;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _soundEmitter = GetComponent<SoundEmitter>();
        _audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Called to explode the object.
    /// </summary>
    /// <param name="position"> Position of the explosion. </param>
    /// <param name="explosionForce"> Force of the explosion. </param>
    private void Explosion(Vector3 position, float explosionForce)
    {
        _fullObject.SetActive(false);
        _collider.enabled = false;
        _rigidbody.isKinematic = true;

        for (int i = 0; i < _fragments.Count; ++i)
        {
            _fragments[i].gameObject.SetActive(true);
            _fragments[i].isKinematic = false;
            _fragments[i].AddExplosionForce(explosionForce, position, _explosionRadius);
        }

        _soundEmitter.EmitSound(transform.position, _soundRadius, SoundType.OneShot);
        _vfx.SetActive(true);

        _audioSource.PlayOneShot(_breakSFX);

        StartCoroutine(Vanish(_fragmentLifetime));
    }

    private void OnCollisionEnter(Collision collision)
    {
        float impactForce = collision.relativeVelocity.magnitude;

        if (impactForce >= _breakForceThreshold)
        {
            Explosion(collision.contacts[0].point, impactForce * _explosionForceMultiplier);
        }
    }

    /// <summary>
    /// Called to 
    /// </summary>
    /// <param name="waitingTime"></param>
    /// <returns></returns>
    private IEnumerator Vanish(float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);

        foreach (Rigidbody fragment in _fragments)
        {
            yield return new WaitForSeconds(_vanishTime);
            
            if (fragment.TryGetComponent<Animator>(out Animator animator))
            {
                animator.SetBool("IsVanishing", true);
            }
        }

        yield return new WaitForSeconds(5f);

        Destroy(gameObject);
    }
}
