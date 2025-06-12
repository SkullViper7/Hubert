using System.Collections.Generic;
using UnityEngine;

public class HubertSFXManager : MonoBehaviour
{
    [SerializeField] AudioClip _shoot;
    [SerializeField] AudioClip _hit;
    [SerializeField] List<AudioClip> _electrified;
    [SerializeField] List<AudioClip> _beforeHit;
    [SerializeField] List<AudioClip> _beforeShoot;
    [SerializeField] List<AudioClip> _buttonPress;
    [SerializeField] List<AudioClip> _crawl;

    [Header("OutOfBreath")]
    [SerializeField] List<AudioClip> _inHale;
    [SerializeField] List<AudioClip> _exHale;
    [SerializeField] List<AudioClip> _outOfBreath;
    [SerializeField] List<AudioClip> _holdBreath;

    [SerializeField] List<AudioClip> _stick;

    [SerializeField] List<AudioClip> _fall;

    AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayShoot() => _audioSource.PlayOneShot(_shoot);
    public void PlayHit() => _audioSource.PlayOneShot(_hit);
    public void PlayElectrified() => _audioSource.PlayOneShot(_electrified[Random.Range(0, _electrified.Count)]);
    public void StopElectrified() => _audioSource.Stop();

    public void PlayBeforeHit() => _audioSource.PlayOneShot(_beforeHit[Random.Range(0, _beforeHit.Count)]);
    public void PlayBeforeShoot() => _audioSource.PlayOneShot(_beforeShoot[Random.Range(0, _beforeShoot.Count)]);
    public void PlayCrawl() => _audioSource.PlayOneShot(_crawl[Random.Range(0, _crawl.Count)]);

    public void PlayInHale() => _audioSource.PlayOneShot(_inHale[Random.Range(0, _inHale.Count)]);
    public void PlayExHale() => _audioSource.PlayOneShot(_exHale[Random.Range(0, _exHale.Count)]);

    public void PlayOutOfBreath() => _audioSource.PlayOneShot(_outOfBreath[Random.Range(0, _outOfBreath.Count)]);
    public void PlayHoldBreath() => _audioSource.PlayOneShot(_holdBreath[Random.Range(0, _holdBreath.Count)]);

    public void PlayStick() => _audioSource.PlayOneShot(_stick[Random.Range(0, _stick.Count)]);

    public void PlayFall() => _audioSource.PlayOneShot(_fall[Random.Range(0, _fall.Count)]);
}
