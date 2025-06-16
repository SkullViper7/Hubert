using UnityEngine;

public class DoorVFX : MonoBehaviour
{
    [SerializeField] GameObject _vfx;
    [SerializeField] GameObject _door;

    [Header("Audio")]
    [SerializeField] AudioClip _openDoorSound;
    AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayVFX()
    {
        _vfx.SetActive(true);
    }

    public void DestroyDoor()
    {
        Destroy(_door);
    }

    public void PlayOpenDoorSound()
    {
        _audioSource.Play();
    }
}
