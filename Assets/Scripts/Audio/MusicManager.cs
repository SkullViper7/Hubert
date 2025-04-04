using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource _calmSource;
    [SerializeField] AudioSource _searchingSource;
    [SerializeField] AudioSource _trackedSource;
    [SerializeField] AudioSource _sfxSource;
    [SerializeField] AudioClip _detected;
    [SerializeField] AudioClip _lost;

    bool _canSwitch;
    bool _isPlayerDetected;
    bool _isPlayerSearched;

    private void Awake()
    {
        EnemyVision.OnPlayerDetected += Detected;
        EnemyVision.OnPlayerLost += Lost;
        BreakableObject.OnSearch += Searched;
        Enemy.OnStopSearching += Lost;
    }

    void Detected()
    {
        _canSwitch = true;
        _isPlayerDetected = true;
        _sfxSource.PlayOneShot(_detected);
    }

    void Searched()
    {
        _canSwitch = true;
        _isPlayerSearched = true;
    }

    void Lost()
    {
        _canSwitch = true;
        _isPlayerDetected = false;
        _isPlayerSearched = false;
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
            else if (_isPlayerSearched)
            {
                _calmSource.volume = 0;
                _searchingSource.volume = 1;
                _trackedSource.volume = 0;
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
