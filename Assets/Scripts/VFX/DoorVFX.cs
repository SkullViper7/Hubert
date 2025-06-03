using UnityEngine;

public class DoorVFX : MonoBehaviour
{
    [SerializeField] GameObject _vfx;
    [SerializeField] GameObject _door;

    public void PlayVFX()
    {
        _vfx.SetActive(true);
    }

    public void DestroyDoor()
    {
        Destroy(_door);
    }
}
