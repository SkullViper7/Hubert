using UnityEngine;

public class OpacityCam : MonoBehaviour
{
    [SerializeField] Transform _player;

    void Update()
    {
        transform.LookAt(_player);
    }
}
