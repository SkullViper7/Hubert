using UnityEngine;

public class CutsceneDoorAudio : MonoBehaviour
{
    [Header("Door")]
    AudioSource _audioSource;
    [SerializeField] AudioClip _hit;
    [SerializeField] AudioClip _fall;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayHit() => _audioSource.PlayOneShot(_hit);
    public void PlayFall() => _audioSource.PlayOneShot(_fall); 
}
