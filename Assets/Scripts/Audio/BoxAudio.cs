using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxAudio : MonoBehaviour
{
    [SerializeField] List<AudioClip> _audioClips;
    AudioSource _audioSource;
    bool _canPlay = true;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (_canPlay)
            {
                _audioSource.PlayOneShot(_audioClips[Random.Range(0, _audioClips.Count)]);
                _canPlay = false;
                StartCoroutine(WaitCoroutine(0.15f));
            }
        }
    }

    IEnumerator WaitCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        _canPlay = true;
    }
}
