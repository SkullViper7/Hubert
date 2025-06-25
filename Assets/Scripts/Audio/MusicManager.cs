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

    [Header("LevelSFX")]
    [SerializeField] AudioClip _electrocuted;
    [SerializeField] AudioClip _win;
    [SerializeField] AudioClip _button;

    [Header("Animations")]
    [SerializeField] Animator _musicAnimator;
    [SerializeField] AnimationClip _searchAnim;
    [SerializeField] AnimationClip _searchEndAnim;

    bool _isPlayerDetected;
    bool _isPlayerSearched;
    bool _isLost;
    bool _isElectrocuted;

    PlayerStateManager _player;

    /// <summary>
    /// Called to init all listeners when the player is instanciated.
    /// </summary>
    void Awake()
    {
        // When the player is instanciated, store its reference and init all listeners.
        GameManager.Instance.OnPlayerInstanciated += player =>
        {
            _player = player;
            InitListeners(_player);
        };

        // When the player press a button, play the sound.
        Button.OnPressedButton += PressButton;
    }

    /// <summary>
    /// Called to init all listeners.
    /// </summary>
    /// <param name="player"> The reference to the player. </param>
    /// <remarks>
    /// This method is called when the player is instanciated and init all listeners.
    /// </remarks>
    private void InitListeners(PlayerStateManager player)
    {
        // When the player changes room, remove old listeners and set new.
        player.OnRoomChanged += PlayerHasChangedRoom;

        // When the alert level of the room changes, change the music.
        player.CurrentRoom.OnRoomAlerteLevelChanged += ChangeMusic;
        // When there is a general alerte in the room, change the music.
        player.CurrentRoom.OnGeneralAlerte += ChangeMusic;

        // When the player is electrified, play the sound.
        player.OnElectrified += Electrocuted;

        // When the player is out of breath, play the sound.
        player.OnHoldAlmostFinished += BeforeOutOfBreath;
        player.OnOutOfBreath += OutOfBreath;
        // When the player cancel the out of breath, cancel the sound.
        player.OnHoldCanceled += CancelBeforeOutOfBreath;
    }

    /// <summary>
    /// Called when the player changes room to remove old listeners and set new.
    /// </summary>
    /// <param name="oldRoom">The room the player is leaving.</param>
    /// <param name="newRoom">The room the player is entering.</param>
    private void PlayerHasChangedRoom(Room oldRoom, Room newRoom)
    {
        // Unsubscribe from events in the old room
        oldRoom.OnRoomAlerteLevelChanged -= ChangeMusic;
        oldRoom.OnGeneralAlerte -= ChangeMusic;

        // Subscribe to events in the new room
        newRoom.OnRoomAlerteLevelChanged += ChangeMusic;
        newRoom.OnGeneralAlerte += ChangeMusic;

        // Update music to reflect the alert level of the new room
        ChangeMusic(newRoom.RoomAlerteLevel);
    }

    /// <summary>
    /// Called to change the music depending of the alert level.
    /// </summary>
    /// <param name="newAlerteLevel"> The new alert level of the room. </param>
    private void ChangeMusic(AlerteLevel newAlerteLevel)
    {
        switch (newAlerteLevel)
        {
            case AlerteLevel.Patrol:
                // If the player was detected and is not electrified, then the player is lost.
                if (_isPlayerDetected && !_isElectrocuted)
                {
                    _isPlayerDetected = false;
                    _isLost = true;
                }
                // If the player was searched, then play the sound to stop the research.
                else if (_isPlayerSearched)
                {
                    _sfxSource.PlayOneShot(_searchEnd);
                    _searchingSource.Play();
                    _musicAnimator.Play(_searchEndAnim.name);
                    _isPlayerSearched = false;
                }
                break;
            case AlerteLevel.Research:
                // Start the research music.
                _sfxSource.PlayOneShot(_search);
                _isPlayerSearched = true;
                _musicAnimator.Play(_searchAnim.name);
                break;
            case AlerteLevel.Alerte:
                // Play the sound when the player is detected.
                _sfxSource.PlayOneShot(_alert);
                _isPlayerDetected = true;
                // Start the music for the alert.
                StartCoroutine(CallAlert());
                break;
            case AlerteLevel.GeneralAlerte:
                // Play the sound when the player is detected.
                _sfxSource.PlayOneShot(_bigAlert);
                _isPlayerDetected = true;
                // Start the music for the alert.
                StartCoroutine(CallAlert());
                break;
        }
    }

    /// <summary>
    /// Ends the alert state and resumes calm music if the player is considered lost.
    /// </summary>
    public void EndAlert()
    {
        if (_isLost)
        {
            // Play the alert end sound effect
            _sfxSource.PlayOneShot(_alertEnd);

            // Stop the tracked music source
            _trackedSource.Stop();

            // Start the calm music source
            _calmSource.Play();

            // Reset the lost player state
            _isLost = false;
        }
    }

    /// <summary>
    /// Called when the alert level of the room changes to an alert or general alert state.
    /// </summary>
    /// <returns> A coroutine to play the alert music. </returns>
    /// <remarks>
    /// This method is called when the alert level of the room changes to an alert or general alert state.
    /// It stops the calm music, waits for 0.5 second, plays the alert fill sound effect, waits for 0.9 second and then starts the tracked music.
    /// </remarks>
    IEnumerator CallAlert()
    {
        // Stop the calm music
        _calmSource.Stop();
        // Stop the searching music
        _searchingSource.Stop();

        // Wait for 0.5 second
        yield return new WaitForSeconds(0.5f);

        // Play the alert fill sound effect
        _sfxSource.PlayOneShot(_alertFill);

        // Wait for 0.9 second
        yield return new WaitForSeconds(0.9f);

        // Start the tracked music
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

    void PressButton()
    {
        _sfxSource.PlayOneShot(_button);
    }
}
