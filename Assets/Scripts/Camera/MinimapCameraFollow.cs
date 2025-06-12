using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    private void LateUpdate()
    {
        Vector3 forward = Camera.main.transform.forward; 
        forward.y = 0;
        forward.Normalize();

        Quaternion rotation = Quaternion.LookRotation(forward);

        transform.rotation = Quaternion.Euler(90, rotation.eulerAngles.y, 0);
    }
}
