using UnityEngine;

[ExecuteAlways]
public class LooneyTunesManager : MonoBehaviour
{
    public static LooneyTunesManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    [SerializeField] Material _material;
    [SerializeField, Range(0, 5)] float _elipseSize;

    public Animator Animator { get; private set; }
    public AnimationClip CloseHole;
    public AnimationClip DeathClose;

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
}
