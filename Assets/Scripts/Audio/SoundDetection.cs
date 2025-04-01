using UnityEngine;

public class SoundDetection : MonoBehaviour
{
    Vector3 _soundPosition;

    public void SendTrigger(float sphereSize, Vector3 position)
    {
        _soundPosition = position;

        Collider[] colliders = Physics.OverlapSphere(transform.position, sphereSize, LayerMask.GetMask("Enemy"));

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Enemy>().GoToSoundPosition(_soundPosition);
        }
    }
}
