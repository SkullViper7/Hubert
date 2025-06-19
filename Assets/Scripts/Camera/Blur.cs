using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class Blur : MonoBehaviour
{
    DepthOfField _blur;

    [SerializeField, Range(0.1f, 2.5f)] float _focalDistance = 2.5f;
    [SerializeField, Range(3f, 6f)] float _apertureSize;

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
    }
}
