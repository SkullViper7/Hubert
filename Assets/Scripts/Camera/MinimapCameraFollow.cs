using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    [SerializeField]
    private float _size;

    [SerializeField]
    private Transform _player;

    private void LateUpdate()
    {
        transform.position = new Vector3(_player.position.x, 10, _player.position.z);

        Vector3 forward = Camera.main.transform.forward; 
        forward.y = 0;
        forward.Normalize();

        Quaternion rotation = Quaternion.LookRotation(forward);

        transform.rotation = Quaternion.Euler(90, rotation.eulerAngles.y, 0);
    }
}
