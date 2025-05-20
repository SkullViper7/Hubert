using System.Collections.Generic;
using UnityEngine;

public class FootstepsSFX : MonoBehaviour
{
    AudioSource _audioSource;
    [SerializeField] List<AudioClip> _WoodFootsteps = new();
    [SerializeField] List<AudioClip> _ConcreteFootsteps = new();
    [SerializeField] List<AudioClip> _TileFootsteps = new();

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayFootstep()
    {
        Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 100f, LayerMask.GetMask("Ground"));
        switch (hit.collider.tag)
        {
            case "Concrete":
                _audioSource.PlayOneShot(_ConcreteFootsteps[Random.Range(0, _ConcreteFootsteps.Count)]);
                break;
            case "Tile":
                _audioSource.PlayOneShot(_TileFootsteps[Random.Range(0, _TileFootsteps.Count)]);
                break;
            case "Wood":
                _audioSource.PlayOneShot(_WoodFootsteps[Random.Range(0, _WoodFootsteps.Count)]);
                break;
        }
    }
}
