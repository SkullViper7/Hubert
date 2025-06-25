using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] AudioSource _calmSource;
    [SerializeField] AudioSource _searchingSource;
    [SerializeField] AudioSource _trackedSource;

    [Header("SFX")]
    [SerializeField] AudioSource _sfxSource;
    [SerializeField] AudioClip _detected;
    [SerializeField] AudioClip _lost;

    bool _canSwitch;
    bool _isPlayerDetected;
    bool _isPlayerSearched;

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
            }
            else if (_isPlayerSearched)
            {
            }
            else
            {
                _sfxSource.PlayOneShot(_lost);
            }
        }
    }
}
