using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField]
    private InputManager _inputManager;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _inputManager.OnCrowlStarted += () => _animator.SetBool("IsCrowling", true);
        _inputManager.OnCrowlCancelled += () => _animator.SetBool("IsCrowling", false);
    }
}
