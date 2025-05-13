using UnityEngine;

public class CamEnterTrigger : MonoBehaviour
{
    [SerializeField] DynamicCam _dynamicCam;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _dynamicCam.SetDollyCam();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _dynamicCam.SetFreeCam();
        }
    }
}
