using UnityEngine;

public class DynamicHole : MonoBehaviour
{
    [SerializeField] Transform _player;
    [SerializeField] float _sizeFactor = 0.15f;
    Material _material;
    [SerializeField] float _smoothFactor = 3f;

    float _currentSize;
    float _targetSize;

    void Update()
    {
        if (Physics.Raycast(transform.position, _player.position - transform.position, out RaycastHit hit,
        Vector3.Distance(transform.position, _player.position)))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                // _targetSize = 1 / (Vector3.Distance(transform.position, _player.position) * _sizeFactor);
                _targetSize = 1f;
            }
            else
            {
                _targetSize = 0f;
            }
        }

        _material = hit.transform.GetComponent<Renderer>().material;

        _currentSize = Mathf.Lerp(_currentSize, _targetSize, Time.deltaTime * _smoothFactor);
        _material.SetFloat("_ElipseSize", _currentSize);
    }
}
