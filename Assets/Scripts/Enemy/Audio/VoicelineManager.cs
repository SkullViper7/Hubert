using UnityEngine;

public class VoicelineManager : MonoBehaviour
{
    [Header("Voicelines")]
    [SerializeField] VoiceData _beforeHitData;
    [SerializeField] VoiceData _beforeShootData;
    [SerializeField] VoiceData _checkData;
    [SerializeField] VoiceData _comsData;
    [SerializeField] VoiceData _gunDrawData;
    [SerializeField] VoiceData _searchData;
    [SerializeField] VoiceData _searchEndData;
    [SerializeField] VoiceData _searchLowData;
    [SerializeField] VoiceData _trackData;
    [SerializeField] VoiceData _trackEndData;
    [SerializeField] VoiceData _trackLowData;

    AudioSource _audioSource;

    EnemyBrain _enemyBrain;

    void Start()
    {
        _enemyBrain = GetComponent<EnemyBrain>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayBeforeHit()
    {
        switch (_enemyBrain.VoiceType)
        {
            case VoiceType.Jules:
                _audioSource.PlayOneShot(_beforeHitData.GetRandomLine(VoiceType.Jules));
                break;
            case VoiceType.Leo:
                _audioSource.PlayOneShot(_beforeHitData.GetRandomLine(VoiceType.Leo));
                break;
            case VoiceType.Todd:
                _audioSource.PlayOneShot(_beforeHitData.GetRandomLine(VoiceType.Todd));
                break;
        }
    }

    public void PlayBeforeShoot()
    {
        switch (_enemyBrain.VoiceType)
        {
            case VoiceType.Jules:
                _audioSource.PlayOneShot(_beforeShootData.GetRandomLine(VoiceType.Jules));
                break;
            case VoiceType.Leo:
                _audioSource.PlayOneShot(_beforeShootData.GetRandomLine(VoiceType.Leo));
                break;
            case VoiceType.Todd:
                _audioSource.PlayOneShot(_beforeShootData.GetRandomLine(VoiceType.Todd));
                break;
        }
    }

    public void PlayCheck()
    {
        switch (_enemyBrain.VoiceType)
        {
            case VoiceType.Jules:
                _audioSource.PlayOneShot(_checkData.GetRandomLine(VoiceType.Jules));
                break;
            case VoiceType.Leo:
                _audioSource.PlayOneShot(_checkData.GetRandomLine(VoiceType.Leo));
                break;
            case VoiceType.Todd:
                _audioSource.PlayOneShot(_checkData.GetRandomLine(VoiceType.Todd));
                break;
        }
    }

    public void PlayComs()
    {
        switch (_enemyBrain.VoiceType)
        {
            case VoiceType.Jules:
                _audioSource.PlayOneShot(_comsData.GetRandomLine(VoiceType.Jules));
                break;
            case VoiceType.Leo:
                _audioSource.PlayOneShot(_comsData.GetRandomLine(VoiceType.Leo));
                break;
            case VoiceType.Todd:
                _audioSource.PlayOneShot(_comsData.GetRandomLine(VoiceType.Todd));
                break;
        }
    }

    public void PlayGunDraw()
    {
        switch (_enemyBrain.VoiceType)
        {
            case VoiceType.Jules:
                _audioSource.PlayOneShot(_gunDrawData.GetRandomLine(VoiceType.Jules));
                break;
            case VoiceType.Leo:
                _audioSource.PlayOneShot(_gunDrawData.GetRandomLine(VoiceType.Leo));
                break;
            case VoiceType.Todd:
                _audioSource.PlayOneShot(_gunDrawData.GetRandomLine(VoiceType.Todd));
                break;
        }
    }

    public void PlaySearch()
    {
        switch (_enemyBrain.VoiceType)
        {
            case VoiceType.Jules:
                _audioSource.PlayOneShot(_searchData.GetRandomLine(VoiceType.Jules));
                break;
            case VoiceType.Leo:
                _audioSource.PlayOneShot(_searchData.GetRandomLine(VoiceType.Leo));
                break;
            case VoiceType.Todd:
                _audioSource.PlayOneShot(_searchData.GetRandomLine(VoiceType.Todd));
                break;
        }
    }

    public void PlaySearchEnd()
    {
        switch (_enemyBrain.VoiceType)
        {
            case VoiceType.Jules:
                _audioSource.PlayOneShot(_searchEndData.GetRandomLine(VoiceType.Jules));
                break;
            case VoiceType.Leo:
                _audioSource.PlayOneShot(_searchEndData.GetRandomLine(VoiceType.Leo));
                break;
            case VoiceType.Todd:
                _audioSource.PlayOneShot(_searchEndData.GetRandomLine(VoiceType.Todd));
                break;
        }
    }

    public void PlaySearchLow()
    {
        switch (_enemyBrain.VoiceType)
        {
            case VoiceType.Jules:
                _audioSource.PlayOneShot(_searchLowData.GetRandomLine(VoiceType.Jules));
                break;
            case VoiceType.Leo:
                _audioSource.PlayOneShot(_searchLowData.GetRandomLine(VoiceType.Leo));
                break;
            case VoiceType.Todd:
                _audioSource.PlayOneShot(_searchLowData.GetRandomLine(VoiceType.Todd));
                break;
        }
    }

    public void PlayTrack()
    {
        switch (_enemyBrain.VoiceType)
        {
            case VoiceType.Jules:
                _audioSource.PlayOneShot(_trackData.GetRandomLine(VoiceType.Jules));
                break;
            case VoiceType.Leo:
                _audioSource.PlayOneShot(_trackData.GetRandomLine(VoiceType.Leo));
                break;
            case VoiceType.Todd:
                _audioSource.PlayOneShot(_trackData.GetRandomLine(VoiceType.Todd));
                break;
        }
    }

    public void PlayTrackEnd()
    {
        switch (_enemyBrain.VoiceType)
        {
            case VoiceType.Jules:
                _audioSource.PlayOneShot(_trackEndData.GetRandomLine(VoiceType.Jules));
                break;
            case VoiceType.Leo:
                _audioSource.PlayOneShot(_trackEndData.GetRandomLine(VoiceType.Leo));
                break;
            case VoiceType.Todd:
                _audioSource.PlayOneShot(_trackEndData.GetRandomLine(VoiceType.Todd));
                break;
        }
    }

    public void PlayTrackLow()
    {
        switch (_enemyBrain.VoiceType)
        {
            case VoiceType.Jules:
                _audioSource.PlayOneShot(_trackLowData.GetRandomLine(VoiceType.Jules));
                break;
            case VoiceType.Leo:
                _audioSource.PlayOneShot(_trackLowData.GetRandomLine(VoiceType.Leo));
                break;
            case VoiceType.Todd:
                _audioSource.PlayOneShot(_trackLowData.GetRandomLine(VoiceType.Todd));
                break;
        }
    }
}
