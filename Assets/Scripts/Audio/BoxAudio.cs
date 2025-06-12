using System.Collections.Generic;
using UnityEngine;

public class BoxAudio : MonoBehaviour
{
    [SerializeField] List<AudioClip> _audioClips;
    AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _audioSource.PlayOneShot(_audioClips[Random.Range(0, _audioClips.Count)]);
        }
    }
}
