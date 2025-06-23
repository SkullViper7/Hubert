using UnityEngine;

[ExecuteAlways]
public class LooneyTunesManager : MonoBehaviour
{
    [SerializeField] Material _material;
    [SerializeField, Range(0, 5)] float _elipseSize;

    private void Update()
    {
        _material.SetFloat("_ElipseSize", _elipseSize);
    }
}
