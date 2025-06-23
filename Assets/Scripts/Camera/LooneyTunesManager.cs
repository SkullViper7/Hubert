using UnityEngine;

[ExecuteAlways]
public class LooneyTunesManager : MonoBehaviour
{
    [SerializeField] Material _material;
    [SerializeField, Range(0, 5)] float _elispeSize;

    private void Update()
    {
        _material.SetFloat("_ElispeSize", _elispeSize);
    }
}
