using System.Collections.Generic;
using UnityEngine;

public class HubertSFXManager : MonoBehaviour
{
    [SerializeField] AudioClip _shoot;
    [SerializeField] AudioClip _hit;

    [SerializeField] List<AudioClip> _fall;

    AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayShoot() => _audioSource.PlayOneShot(_shoot);
    public void PlayHit() => _audioSource.PlayOneShot(_hit);
    public void PlayFall() => _audioSource.PlayOneShot(_fall[Random.Range(0, _fall.Count)]);
}
