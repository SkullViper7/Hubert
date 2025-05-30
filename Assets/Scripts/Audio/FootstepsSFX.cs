using System.Collections.Generic;
using UnityEngine;

public class FootstepsSFX : MonoBehaviour
{
    AudioSource _audioSource;
    SoundEmitter _soundEmitter;

    [SerializeField] float _soundDistance = 10f;

    [Header("Walk Footsteps")]
    [SerializeField] List<AudioClip> _walkWoodFootsteps;
    [SerializeField] List<AudioClip> _walkConcreteFootsteps;
    [SerializeField] List<AudioClip> _walkTileFootsteps;

    [Header("Crawl Footsteps")]
    [SerializeField] List<AudioClip> _crawlWoodFootsteps;
    [SerializeField] List<AudioClip> _crawlConcreteFootsteps;
    [SerializeField] List<AudioClip> _crawlTileFootsteps;

    [Header("Aim Walk Footsteps")]
    [SerializeField] List<AudioClip> _aimWalkWoodFootsteps;
    [SerializeField] List<AudioClip> _aimWalkConcreteFootsteps;
    [SerializeField] List<AudioClip> _aimWalkTileFootsteps;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _soundEmitter = GetComponent<SoundEmitter>();
    }

    public void PlayWalkFootstep()
    {
        Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 100f, LayerMask.GetMask("Ground"));
        switch (hit.collider.tag)
        {
            case "Concrete":
                _audioSource.PlayOneShot(_walkConcreteFootsteps[Random.Range(0, _walkConcreteFootsteps.Count)]);
                break;
            case "Tile":
                _audioSource.PlayOneShot(_walkTileFootsteps[Random.Range(0, _walkTileFootsteps.Count)]);
                break;
            case "Wood":
                _audioSource.PlayOneShot(_walkWoodFootsteps[Random.Range(0, _walkWoodFootsteps.Count)]);
                break;
        }

        _soundEmitter.EmitSound(transform.position, _soundDistance, SoundType.Continue);
    }

    public void PlayCrawlFootstep()
    {
        Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 100f, LayerMask.GetMask("Ground"));
        switch (hit.collider.tag)
        {
            case "Concrete":
                _audioSource.PlayOneShot(_crawlConcreteFootsteps[Random.Range(0, _crawlConcreteFootsteps.Count)]);
                break;
            case "Tile":
                _audioSource.PlayOneShot(_crawlTileFootsteps[Random.Range(0, _crawlTileFootsteps.Count)]);
                break;
            case "Wood":
                _audioSource.PlayOneShot(_crawlWoodFootsteps[Random.Range(0, _crawlWoodFootsteps.Count)]);
                break;
        }
    }

    public void PlayAimWalkFootstep()
    {
        Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 100f, LayerMask.GetMask("Ground"));
        switch (hit.collider.tag)
        {
            case "Concrete":
                _audioSource.PlayOneShot(_aimWalkConcreteFootsteps[Random.Range(0, _aimWalkConcreteFootsteps.Count)]);
                break;
            case "Tile":
                _audioSource.PlayOneShot(_aimWalkTileFootsteps[Random.Range(0, _aimWalkTileFootsteps.Count)]);
                break;
            case "Wood":
                _audioSource.PlayOneShot(_aimWalkWoodFootsteps[Random.Range(0, _aimWalkWoodFootsteps.Count)]);
                break;
        }
    }
}
