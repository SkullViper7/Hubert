using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private void Awake()
    {
        EnemyVision.OnPlayerDetected += Detected;
        EnemyVision.OnPlayerLost += Lost;
    }

    [SerializeField] AudioSource _calmSource;
    [SerializeField] AudioSource _searchingSource;
    [SerializeField] AudioSource _trackedSource;
    [SerializeField] AudioSource _sfxSource;
    [SerializeField] AudioClip _detected;
    [SerializeField] AudioClip _lost;

    bool _canSwitch;
    bool _isPlayerDetected;

    void Detected()
    {
        _canSwitch = true;
        _isPlayerDetected = true;
        _sfxSource.PlayOneShot(_detected);
    }

    void Lost()
    {
        _canSwitch = true;
        _isPlayerDetected = false;
    }

    public void Switch()
    {
        if (_canSwitch)
        {
            _canSwitch = false;

            if (_isPlayerDetected)
            {
                _calmSource.volume = 0;
                _searchingSource.volume = 0;
                _trackedSource.volume = 1;
            }
            else
            {
                _calmSource.volume = 1;
                _searchingSource.volume = 0;
                _trackedSource.volume = 0;
                _sfxSource.PlayOneShot(_lost);
            }
        }
    }
}
