using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] AudioSource _calmSource;
    [SerializeField] AudioSource _searchingSource;
    [SerializeField] AudioSource _trackedSource;

    [Header("AlertSFX")]
    [SerializeField] AudioSource _sfxSource;
    [SerializeField] AudioClip _search;
    [SerializeField] AudioClip _alert;
    [SerializeField] AudioClip _bigAlert;
    [SerializeField] AudioClip _searchEnd;
    [SerializeField] AudioClip _alertEnd;
    [SerializeField] AudioClip _alertFill;

    [Header("StickSFX")]
    [SerializeField] AudioClip _beforeOutOfBreath;
    [SerializeField] AudioClip _outOfBreath;

    [Header("GameSFX")]
    [SerializeField] AudioClip _electrocuted;
    [SerializeField] AudioClip _win;

    [Header("Animations")]
    [SerializeField] Animator _musicAnimator;
    [SerializeField] AnimationClip _searchAnim;
    [SerializeField] AnimationClip _searchEndAnim;

    bool _isPlayerDetected;
    bool _isPlayerSearched;
    bool _isLost;
    bool _isElectrocuted;

    PlayerStateManager _player;

    void Awake()
    {
        GameManager.Instance.OnPlayerInstanciated += player =>
        {
            _player = player;
            InitListeners(_player);
        };
    }

    /// <summary>
    /// Called to init all listeners.
    /// </summary>
    /// <param name="player"> The reference to the player. </param>
    private void InitListeners(PlayerStateManager player)
    {
        player.OnRoomChanged += PlayerHasChangedRoom;

        player.CurrentRoom.OnRoomAlerteLevelChanged += ChangeMusic;
        player.CurrentRoom.OnGeneralAlerte += ChangeMusic;

        player.OnElectrified += Electrocuted;

        player.OnHoldAlmostFinished += BeforeOutOfBreath;
        player.OnOutOfBreath += OutOfBreath;
        player.OnHoldCanceled += CancelBeforeOutOfBreath;
    }

    /// <summary>
    /// Called when the player changes room to remove old listeners and set new.
    /// </summary>
    private void PlayerHasChangedRoom(Room oldRoom, Room newRoom)
    {
        oldRoom.OnRoomAlerteLevelChanged -= ChangeMusic;
        newRoom.OnRoomAlerteLevelChanged += ChangeMusic;

        oldRoom.OnGeneralAlerte -= ChangeMusic;
        newRoom.OnGeneralAlerte += ChangeMusic;

        ChangeMusic(newRoom.RoomAlerteLevel);
    }

    /// <summary>
    /// Called to change the music depending of the alert level.
    /// </summary>
    private void ChangeMusic(AlerteLevel newAlerteLevel)
    {
        switch (newAlerteLevel)
        {
            case AlerteLevel.Patrol:
                if (_isPlayerDetected && !_isElectrocuted)
                {
                    _isPlayerDetected = false;
                    _isLost = true;
                }
                else if (_isPlayerSearched)
                {
                    _sfxSource.PlayOneShot(_searchEnd);
                    _searchingSource.Play();
                    _musicAnimator.Play(_searchEndAnim.name);
                    _isPlayerSearched = false;
                }
                break;
            case AlerteLevel.Research:
                _sfxSource.PlayOneShot(_search);
                _isPlayerSearched = true;
                _musicAnimator.Play(_searchAnim.name);
                break;
            case AlerteLevel.Alerte:
                _sfxSource.PlayOneShot(_alert);
                _isPlayerDetected = true;
                StartCoroutine(CallAlert());
                break;
            case AlerteLevel.GeneralAlerte:
                _sfxSource.PlayOneShot(_bigAlert);
                _isPlayerDetected = true;
                StartCoroutine(CallAlert());
                break;
        }
    }

    public void EndAlert()
    {
        if (_isLost)
        {
            _sfxSource.PlayOneShot(_alertEnd);
            _trackedSource.Stop();
            _calmSource.Play();

            _isLost = false;
        }
    }

    IEnumerator CallAlert()
    {
        _calmSource.Stop();
        _searchingSource.Stop();

        yield return new WaitForSeconds(0.5f);

        _sfxSource.PlayOneShot(_alertFill);

        yield return new WaitForSeconds(0.9f);

        _trackedSource.Play();
    }

    void Electrocuted()
    {
        _sfxSource.PlayOneShot(_electrocuted);
        _trackedSource.Stop();
        _isElectrocuted = true;
    }

    void BeforeOutOfBreath()
    {
        _sfxSource.PlayOneShot(_beforeOutOfBreath);
    }

    void OutOfBreath()
    {
        _sfxSource.PlayOneShot(_outOfBreath);
    }
    
    void CancelBeforeOutOfBreath()
    {
        _sfxSource.Stop();
    }
}
