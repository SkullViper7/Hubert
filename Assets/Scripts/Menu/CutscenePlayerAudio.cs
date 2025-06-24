using UnityEngine;

public class CutscenePlayerAudio : MonoBehaviour
{
    [Header("Door")]
    AudioSource _audioSource;
    [SerializeField] AudioClip _stretchShort;
    [SerializeField] AudioClip _stretchLong;
    [SerializeField] AudioClip _pop;
    [SerializeField] AudioClip _fall;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayShortStretch() => _audioSource.PlayOneShot(_stretchShort);
    public void PlayLongStretch() => _audioSource.PlayOneShot(_stretchLong);
    public void PlayPop() => _audioSource.PlayOneShot(_pop);
    public void PlayFall() => _audioSource.PlayOneShot(_fall); 
}
