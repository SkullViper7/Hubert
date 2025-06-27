using System;
using UnityEngine;

public class LooneyTunesManager : MonoBehaviour
{
    // Singleton
    private static LooneyTunesManager _instance = null;
    public static LooneyTunesManager Instance => _instance;

    private void Awake()
    {
        // Singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _instance = this;
        }
    }

    [SerializeField] Material _material;
    [SerializeField, Range(0, 5)] float _elipseSize;

    public Animator Animator { get; private set; }
    public AnimationClip CloseHole;
    public AnimationClip DeathClose;

    public event Action OnRoundClose;

    private void Start()
    {
        Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _material.SetFloat("_ElipseSize", _elipseSize);
    }

    public void PlayOpenHoleAnim() => Animator.Play("RoundOpen");
    public void PlayCloseHoleAnim() => Animator.Play(CloseHole.name);
    public void PlayDeathCloseAnim() => Animator.Play(DeathClose.name);

    public void RoundClosed() => OnRoundClose?.Invoke();
}
