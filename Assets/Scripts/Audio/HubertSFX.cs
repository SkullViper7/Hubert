using System.Collections.Generic;
using UnityEngine;

public class HubertSFX : MonoBehaviour
{
    /// <summary>
    /// Fotsteps SFXs on wood.
    /// </summary>
    [SerializeField, Header("SFX")] 
    private List<AudioClip> _walkWoodFootsteps;

    /// <summary>
    /// Fotsteps SFXs on tiles.
    /// </summary>
    [SerializeField] 
    private List<AudioClip> _walkTileFootsteps;

    /// <summary>
    /// SFX when hubert shoots.
    /// </summary>
    [SerializeField]
    private AudioClip _shoot;

    /// <summary>
    /// SFX when hubert hits.
    /// </summary>
    [SerializeField]
    private AudioClip _hit;

    /// <summary>
    /// SFXs when hubert falls.
    /// </summary>
    [SerializeField]
    private List<AudioClip> _fall;

    /// <summary>
    /// Player state manager.
    /// </summary>
    [Space, SerializeField]
    private PlayerStateManager _playerStateManager;

    /// <summary>
    /// Animation controller of the player.
    /// </summary>
    private PlayerAnimationController _animationController;

    /// <summary>
    /// Audio source.
    /// </summary>
    private AudioSource _audioSource;

    /// <summary>
    /// Sound emitter.
    /// </summary>
    private SoundEmitter _soundEmitter;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _soundEmitter = GetComponent<SoundEmitter>();
        _animationController = GetComponent<PlayerAnimationController>();
    }

    private void Start()
    {
        _animationController.OnLeftStep += PlayWalkFootstep;
        _animationController.OnRightStep += PlayWalkFootstep;
        _animationController.MustShoot += PlayShoot;
        _animationController.MustHit += PlayHit;
        _animationController.OnFall += PlayFall;
    }

    private void PlayWalkFootstep()
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

        _soundEmitter.EmitSound(transform.position, _playerStateManager.WalkSoundRange, SoundType.Continue, false);
    }

    private void PlayShoot()
    {
        _soundEmitter.EmitSound(transform.position, _playerStateManager.ShotSoundRange, SoundType.OneShot, false);
        _audioSource.PlayOneShot(_shoot);
    }

    private void PlayHit()
    {
        _soundEmitter.EmitSound(transform.position, _playerStateManager.HitSoundRange, SoundType.OneShot, false);
        _audioSource.PlayOneShot(_hit);
    }

    private void PlayFall()
    {
        _audioSource.PlayOneShot(_fall[Random.Range(0, _fall.Count)]);
    }
}
