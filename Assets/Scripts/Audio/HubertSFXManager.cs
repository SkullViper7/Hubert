using UnityEngine;

public class HubertSFXManager : MonoBehaviour
{
    [SerializeField] AudioClip _shoot;
    [SerializeField] AudioClip _hit;
    [SerializeField] AudioClip _electrified;

    AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayShoot() => _audioSource.PlayOneShot(_shoot);
    public void PlayHit() => _audioSource.PlayOneShot(_hit);
    public void PlayElectrified() => _audioSource.PlayOneShot(_electrified);
    public void StopElectrified() => _audioSource.Stop();
}
