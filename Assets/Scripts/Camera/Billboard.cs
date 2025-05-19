using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] Transform _target;

    void Update()
    {
        transform.LookAt(_target);
    }
}
