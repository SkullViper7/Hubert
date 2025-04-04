using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoundDetection))]
public class BreakableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject _fullObject;

    [SerializeField]
    private List<Rigidbody> _fragments;

    [SerializeField]
    private float _breakForceThreshold;

    [SerializeField]
    private float _explosionForceMultiplier;

    [SerializeField]
    private float _explosionRadius;

    private Rigidbody _rigidbody;

    private Collider _collider;

    [Header("Audio")]
    [SerializeField] float _soundRadius = 10f;
    SoundDetection _soundDetection;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        _soundDetection = GetComponent<SoundDetection>();
    }

    private void Explosion(Vector3 position, float explosionForce)
    {
        _fullObject.SetActive(false);
        _collider.enabled = false;
        _rigidbody.isKinematic = true;

        _soundDetection.SendTrigger(_soundRadius, transform.position);

        for (int i = 0; i < _fragments.Count; ++i)
        {
            _fragments[i].gameObject.SetActive(true);
            _fragments[i].isKinematic = false;
            _fragments[i].AddExplosionForce(explosionForce, position, _explosionRadius);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        float impactForce = collision.relativeVelocity.magnitude;

        if (impactForce >= _breakForceThreshold)
        {
            Explosion(collision.contacts[0].point, impactForce * _explosionForceMultiplier);
        }
    }
}
