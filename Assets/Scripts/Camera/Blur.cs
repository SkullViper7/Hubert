using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Blur : MonoBehaviour
{
    DepthOfField _blur;

    [SerializeField, Range(0f, 8f)] float _focalDistance = 8f;
    [SerializeField, Range(3f, 6f)] float _apertureSize;

    CinemachineFreeLook _camera;
    float _zoomValue;

    void Awake()
    {
        GameManager.Instance.OnPlayerInstanciated += player =>
        {
            _camera = player.Camera;
        };
    }

    void Start()
    {
        if (GetComponent<Volume>().profile.TryGet<DepthOfField>(out DepthOfField depthOfField))
        {
            _blur = depthOfField;
        }
    }

    void Update()
    {
        _blur.focusDistance.value = _focalDistance;
        _blur.aperture.value = _apertureSize;
        _zoomValue = _camera.m_YAxis.Value;
        _apertureSize = Mathf.Lerp(3f, 6f, _zoomValue);
    }
}
