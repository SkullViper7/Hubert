using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource _calmSource;
    [SerializeField] AudioSource _searchingSource;
    [SerializeField] AudioSource _trackedSource;

    public bool CanSwitch;

    public void Switch()
    {
        if (CanSwitch)
        {
            CanSwitch = false;
            _calmSource.volume = 0;
            _searchingSource.volume = 0;
            _trackedSource.volume = 1;
        }
    }
}
