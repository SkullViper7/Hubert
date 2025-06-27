using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameAudioSettings : MonoBehaviour
{
    [SerializeField] AudioMixer _mixer;
    [SerializeField] Slider _masterSlider;
    [SerializeField] Slider _musicSlider;
    [SerializeField] Slider _sfxSlider;
    [SerializeField] TMP_Dropdown _outputDropdown;
    [SerializeField] GameObject _outputMessage;
    [SerializeField] GameObject _inputBlocker;

    [Space]
    [SerializeField] GameObject _outputButton;
    [SerializeField] GameObject _noButton;

    int _outputTypeIndex = 0;
    int _oldOutputValue;

    EventSystem _eventSystem;

    void Start()
    {
        _masterSlider.onValueChanged.AddListener(SetMasterVolume);
        _musicSlider.onValueChanged.AddListener(SetMusicVolume);
        _sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            _oldOutputValue = _outputDropdown.value;
        }

        _eventSystem = EventSystem.current;
    }

    float VolumeToDecibel(float volume)
    {
        return Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
    }

    public void SetMasterVolume(float volume)
    {
        _mixer.SetFloat("Master", VolumeToDecibel(volume));
    }

    public void SetMusicVolume(float volume)
    {
        _mixer.SetFloat("Music", VolumeToDecibel(volume));
    }

    public void SetSFXVolume(float volume)
    {
        _mixer.SetFloat("SFX", VolumeToDecibel(volume));
    }

    public void ChangeOutputType()
    {
        switch (_outputDropdown.value)
        {
            case 0:
                _outputTypeIndex = 0;
                break;
            case 1:
                _outputTypeIndex = 1;
                break;
            case 2:
                _outputTypeIndex = 2;
                break;
            case 3:
                _outputTypeIndex = 3;
                break;
        }

        _outputMessage.SetActive(true);
        _inputBlocker.SetActive(true);

        StartCoroutine(SwitchToNoButton());
    }

    IEnumerator SwitchToNoButton()
    {
        yield return null;
        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(_noButton);
    }

    public void ApplyOutput()
    {
        var config = AudioSettings.GetConfiguration();

        switch (_outputTypeIndex)
        {
            case 0:
                config.speakerMode = AudioSpeakerMode.Stereo;
                break;
            case 1:
                config.speakerMode = AudioSpeakerMode.Mono;
                break;
            case 2:
                config.speakerMode = AudioSpeakerMode.Mode5point1;
                break;
            case 3:
                config.speakerMode = AudioSpeakerMode.Mode7point1;
                break;
        }

        AudioSettings.Reset(config);

        Application.Quit();
    }

    public void HideOutputMessage()
    {
        _outputDropdown.value = _oldOutputValue;
        _outputMessage.SetActive(false);
        _inputBlocker.SetActive(false);

        StartCoroutine(SwitchToOutputButton());
    }

    IEnumerator SwitchToOutputButton()
    {
        yield return null;
        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(_outputButton);
    }
}
