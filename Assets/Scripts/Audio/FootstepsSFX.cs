using System.Collections.Generic;
using UnityEngine;

public class FootstepsSFX : MonoBehaviour
{
    AudioSource _audioSource;
    SoundEmitter _soundEmitter;

    [SerializeField] float _soundDistance = 10f;

    [Header("Walk Footsteps")]
    [SerializeField] List<AudioClip> _walkWoodFootsteps;
    [SerializeField] List<AudioClip> _walkTileFootsteps;

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
            case "Tile":
                _audioSource.PlayOneShot(_walkTileFootsteps[Random.Range(0, _walkTileFootsteps.Count)]);
                break;
            case "Wood":
                _audioSource.PlayOneShot(_walkWoodFootsteps[Random.Range(0, _walkWoodFootsteps.Count)]);
                break;
        }

        _soundEmitter.EmitSound(transform.position, _soundDistance, SoundType.Continue, false);
    }
}
