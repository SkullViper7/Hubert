using UnityEngine;

public class DynamicHole : MonoBehaviour
{
    [SerializeField] Transform _player;
    [SerializeField] float _sizeFactor = 0.15f;
    [SerializeField] Material _material;
    [SerializeField] float _smoothFactor = 3f;

    float _currentSize;
    float _targetSize;

    void Update()
    {
        Ray ray = new Ray(transform.position, _player.position - transform.position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == _player)
            {
                _targetSize = 0f;
            }
            else
            {
                _targetSize = 1 / (Vector3.Distance(transform.position, _player.position) * _sizeFactor);
            }
        }

        _currentSize = Mathf.Lerp(_currentSize, _targetSize, Time.deltaTime * _smoothFactor);
        _material.SetFloat("_ElipseSize", _currentSize);
    }
}
