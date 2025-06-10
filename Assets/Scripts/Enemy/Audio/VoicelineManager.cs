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

        _enemyBrain.OnSpeak += PlaySound;
    }

    void PlaySound(Voiceline voiceline, VoiceType voiceType)
    {
        _audioSource.Stop();
        switch (voiceline)
        {
            case Voiceline.BeforeHit:
                switch (voiceType)
                {
                    case VoiceType.Jules:
                        _audioSource.PlayOneShot(_beforeHitData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Leo:
                        _audioSource.PlayOneShot(_beforeHitData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Todd:
                        _audioSource.PlayOneShot(_beforeHitData.GetRandomLine(voiceType));
                        break;
                }
                break;
            case Voiceline.BeforeShoot:
                switch (voiceType)
                {
                    case VoiceType.Jules:
                        _audioSource.PlayOneShot(_beforeShootData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Leo:
                        _audioSource.PlayOneShot(_beforeShootData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Todd:
                        _audioSource.PlayOneShot(_beforeShootData.GetRandomLine(voiceType));
                        break;
                }
                break;
            case Voiceline.Check:
                switch (voiceType)
                {
                    case VoiceType.Jules:
                        _audioSource.PlayOneShot(_checkData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Leo:
                        _audioSource.PlayOneShot(_checkData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Todd:
                        _audioSource.PlayOneShot(_checkData.GetRandomLine(voiceType));
                        break;
                }
                break;
            case Voiceline.Coms:
                switch (voiceType)
                {
                    case VoiceType.Jules:
                        _audioSource.PlayOneShot(_comsData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Leo:
                        _audioSource.PlayOneShot(_comsData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Todd:
                        _audioSource.PlayOneShot(_comsData.GetRandomLine(voiceType));
                        break;
                }
                break;
            case Voiceline.GunDraw:
                switch (voiceType)
                {
                    case VoiceType.Jules:
                        _audioSource.PlayOneShot(_gunDrawData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Leo:
                        _audioSource.PlayOneShot(_gunDrawData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Todd:
                        _audioSource.PlayOneShot(_gunDrawData.GetRandomLine(voiceType));
                        break;
                }
                break;
            case Voiceline.Search:
                switch (voiceType)
                {
                    case VoiceType.Jules:
                        _audioSource.PlayOneShot(_searchData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Leo:
                        _audioSource.PlayOneShot(_searchData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Todd:
                        _audioSource.PlayOneShot(_searchData.GetRandomLine(voiceType));
                        break;
                }
                break;
            case Voiceline.SearchEnd:
                switch (voiceType)
                {
                    case VoiceType.Jules:
                        _audioSource.PlayOneShot(_searchEndData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Leo:
                        _audioSource.PlayOneShot(_searchEndData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Todd:
                        _audioSource.PlayOneShot(_searchEndData.GetRandomLine(voiceType));
                        break;
                }
                break;
            case Voiceline.SearchLow:
                switch (voiceType)
                {
                    case VoiceType.Jules:
                        _audioSource.PlayOneShot(_searchLowData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Leo:
                        _audioSource.PlayOneShot(_searchLowData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Todd:
                        _audioSource.PlayOneShot(_searchLowData.GetRandomLine(voiceType));
                        break;
                }
                break;
            case Voiceline.Track:
                switch (voiceType)
                {
                    case VoiceType.Jules:
                        _audioSource.PlayOneShot(_trackData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Leo:
                        _audioSource.PlayOneShot(_trackData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Todd:
                        _audioSource.PlayOneShot(_trackData.GetRandomLine(voiceType));
                        break;
                }
                break;
            case Voiceline.TrackEnd:
                switch (voiceType)
                {
                    case VoiceType.Jules:
                        _audioSource.PlayOneShot(_trackEndData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Leo:
                        _audioSource.PlayOneShot(_trackEndData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Todd:
                        _audioSource.PlayOneShot(_trackEndData.GetRandomLine(voiceType));
                        break;
                }
                break;
            case Voiceline.TrackLow:
                switch (voiceType)
                {
                    case VoiceType.Jules:
                        _audioSource.PlayOneShot(_trackLowData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Leo:
                        _audioSource.PlayOneShot(_trackLowData.GetRandomLine(voiceType));
                        break;
                    case VoiceType.Todd:
                        _audioSource.PlayOneShot(_trackLowData.GetRandomLine(voiceType));
                        break;
                }
                break;
        }
    }
}