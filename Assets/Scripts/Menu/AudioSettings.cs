using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] AudioMixer _mixer;
    [SerializeField] Slider _masterSlider;
    [SerializeField] Slider _musicSlider;
    [SerializeField] Slider _sfxSlider;

    void Start()
    {
        _masterSlider.onValueChanged.AddListener(SetMasterVolume);
        _musicSlider.onValueChanged.AddListener(SetMusicVolume);
        _sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float volume)
    {
        _mixer.SetFloat("Master", volume);
    }

    public void SetMusicVolume(float volume)
    {
        _mixer.SetFloat("Music", volume);
    }

    public void SetSFXVolume(float volume)
    {
        _mixer.SetFloat("SFX", volume);
    }
}
