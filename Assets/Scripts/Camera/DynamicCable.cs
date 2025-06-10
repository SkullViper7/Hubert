using UnityEngine;

public class DynamicCable : MonoBehaviour
{
    [SerializeField] Transform _player;
    [SerializeField] float _sizeFactor = 0.15f;
    [SerializeField] Material _cableOffMat;
    [SerializeField] Material _cableOnMat;
    [SerializeField] float _smoothFactor = 3f;

    float _currentSize;
    float _targetSize;

    void Update()
    {
        _targetSize = 1 / (Vector3.Distance(transform.position, _player.position) * _sizeFactor);

        _currentSize = Mathf.Lerp(_currentSize, _targetSize, Time.deltaTime * _smoothFactor);
        _cableOffMat.SetFloat("_ElipseSize", _currentSize);
        _cableOnMat.SetFloat("_ElipseSize", _currentSize);
    }
}
