using Cinemachine;
using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    [SerializeField] Transform _player;
    [SerializeField] CinemachineFreeLook _freeLookCam;

    void LateUpdate()
    {
        transform.position = new Vector3(_player.position.x, 10, _player.position.z);

        Vector3 forward = _freeLookCam.State.CorrectedOrientation * Vector3.forward; 
        forward.y = 0;
        forward.Normalize();

        Quaternion rotation = Quaternion.LookRotation(forward);

        transform.rotation = Quaternion.Euler(90, rotation.eulerAngles.y, 0);
    }
}
