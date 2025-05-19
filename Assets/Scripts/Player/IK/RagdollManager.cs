using System.Collections.Generic;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    List<Rigidbody> _rigidbodies;
    List<Collider> _colliders;
    [SerializeField] Rigidbody _headRb;
    [SerializeField] CharacterController _playerController;
    Animator _animator;

    [SerializeField] float _initialForce;

    private void Start()
    {
        _rigidbodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
        _colliders = new List<Collider>(GetComponentsInChildren<Collider>());
        _animator = GetComponent<Animator>();
    }

    public void EnableRagdoll()
    {
        foreach (Rigidbody rb in _rigidbodies)
        {
            rb.isKinematic = false;
        }

        foreach (Collider collider in _colliders)
        {
            collider.enabled = true;
        }

        _playerController.enabled = false;

        _animator.enabled = false;

        //_headRb.AddForce(Vector3.back * _initialForce, ForceMode.Impulse);
    }
}
