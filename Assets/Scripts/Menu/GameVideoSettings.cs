using TMPro;
using UnityEngine;

public class GameVideoSettings : MonoBehaviour
{
    [SerializeField] TMP_Dropdown _resolutionDropdown;
    [SerializeField] TMP_Dropdown _displayDropdown;
    [SerializeField] TMP_Dropdown _framerateDropdown;
    [SerializeField] TMP_Dropdown _qualityDropdown;

    int _screenIndex = 0; // 0 = Windowed, 1 = FullscreenWindow, 2 = Exclusive

    int _screenHeight;
    int _screenWidth;

    int _selectedResolutionIndex;
    int _selectedDisplayIndex;
    int _selectedFramerateIndex;
    int _selectedQualityIndex;

    void Start()
    {
        _resolutionDropdown.onValueChanged.AddListener(OnResolutionDropdownChanged);
        _displayDropdown.onValueChanged.AddListener(OnDisplayDropdownChanged);
        _framerateDropdown.onValueChanged.AddListener(OnFramerateDropdownChanged);
        _qualityDropdown.onValueChanged.AddListener(OnQualityDropdownChanged);

        _selectedDisplayIndex = PlayerPrefs.GetInt("OutputType", 0);
        _selectedResolutionIndex = PlayerPrefs.GetInt("Resolution", 0);
        _selectedFramerateIndex = PlayerPrefs.GetInt("Framerate", 0);
        _selectedQualityIndex = PlayerPrefs.GetInt("Quality", 0);

        AutoSetResolution();

        ApplySettings();
    }

    void AutoSetResolution()
    {
        _screenHeight = Screen.height;
        _screenWidth = Screen.width;

        if (_screenWidth == 1920 && _screenHeight == 1080)
        {
            _selectedResolutionIndex = 0;
            _resolutionDropdown.value = 0;
        }
        else if (_screenWidth == 2560 && _screenHeight == 1440)
        {
            _resolutionDropdown.value = 1;
            _selectedResolutionIndex = 1;
        }
        else if (_screenWidth == 3840 && _screenHeight == 2160)
        {
            _resolutionDropdown.value = 2;
            _selectedResolutionIndex = 2;
        }
        else
        {
            _resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(_screenWidth + "x" + _screenHeight));
            _resolutionDropdown.value = 3;
        }
        _resolutionDropdown.RefreshShownValue();
    }

    void OnResolutionDropdownChanged(int resolutionIndex)
    {
        _selectedResolutionIndex = resolutionIndex;
    }

    void OnDisplayDropdownChanged(int displayIndex)
    {
        _selectedDisplayIndex = displayIndex;
    }

    void OnFramerateDropdownChanged(int framerateIndex)
    {
        _selectedFramerateIndex = framerateIndex;
    }

    void OnQualityDropdownChanged(int qualityIndex)
    {
        _selectedQualityIndex = qualityIndex;
    }

    public void ApplySettings()
    {
        ChangeDisplay(_selectedDisplayIndex, _selectedResolutionIndex);
        ChangeFramerate(_selectedFramerateIndex);
        ChangeQuality(_selectedQualityIndex);

        PlayerPrefs.SetInt("OutputType", _selectedDisplayIndex);
        PlayerPrefs.SetInt("Resolution", _selectedResolutionIndex);
        PlayerPrefs.SetInt("Framerate", _selectedFramerateIndex);
        PlayerPrefs.SetInt("Quality", _selectedQualityIndex);
    }

    void ChangeDisplay(int screenIndex, int resolutionIndex)
    {
        FullScreenMode mode = screenIndex switch
        {
            0 => FullScreenMode.ExclusiveFullScreen,
            1 => FullScreenMode.FullScreenWindow,
            _ => FullScreenMode.Windowed
        };

        Vector2Int res = resolutionIndex switch
        {
            0 => new Vector2Int(1920, 1080),
            1 => new Vector2Int(2560, 1440),
            2 => new Vector2Int(3840, 2160),
            3 => new Vector2Int(_screenWidth, _screenHeight),
            _ => throw new System.NotImplementedException(),
        };

        Screen.SetResolution(res.x, res.y, mode);
    }

    void ChangeFramerate(int index)
    {
        switch (index)
        {
            case 0:
                Application.targetFrameRate = 60;
                break;
            case 1:
                Application.targetFrameRate = 144;
                break;
            case 2:
                Application.targetFrameRate = 0;
                break;
        }
    }

    void ChangeQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}


