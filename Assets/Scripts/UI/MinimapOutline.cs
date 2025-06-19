using UnityEngine;
using UnityEngine.UI;

public class MinimapOutline : MonoBehaviour
{
    /// <summary>
    /// Color of the outline when the room is in patrol state.
    /// </summary>
    [SerializeField]
    private Color _patrolColor;

    /// <summary>
    /// Color of the outline when the room is in research state.
    /// </summary>
    [SerializeField]
    private Color _researchColor;

    /// <summary>
    /// Color of the outline when the room is in alerte state.
    /// </summary>
    [SerializeField]
    private Color _alerteColor;

    /// <summary>
    /// A reference to the player.
    /// </summary>
    private PlayerStateManager _player;

    /// <summary>
    /// Animator of the outline.
    /// </summary>
    private Animator _animator;

    /// <summary>
    /// Image component of the outline.
    /// </summary>
    private Image _image;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _image = GetComponent<Image>();

        GameManager.Instance.OnPlayerInstanciated += (PlayerStateManager player) =>
        {
            _player = player;
            InitListeners(_player);
        };
    }

    private void Start()
    {
        _image.color = _patrolColor;
    }

    /// <summary>
    /// Called to init all listeners.
    /// </summary>
    /// <param name="player"> The reference to the player. </param>
    private void InitListeners(PlayerStateManager player)
    {
        player.OnRoomChanged += PlayerHasChangedRoom;
        player.CurrentRoom.OnAlerteAlmostFinished += PlayFlash;
        player.CurrentRoom.OnRoomAlerteLevelChanged += ChangeOutlineColor;
    }

    /// <summary>
    /// Called when the player changes room to remove old listeners and set new.
    /// </summary>
    private void PlayerHasChangedRoom(Room oldRoom, Room newRoom)
    {
        oldRoom.OnAlerteAlmostFinished -= PlayFlash;
        newRoom.OnAlerteAlmostFinished += PlayFlash;
        oldRoom.OnRoomAlerteLevelChanged -= ChangeOutlineColor;
        newRoom.OnRoomAlerteLevelChanged += ChangeOutlineColor;

        ChangeOutlineColor(newRoom.RoomAlerteLevel);
    }

    /// <summary>
    /// Called to play a flash.
    /// </summary>
    private void PlayFlash()
    {
        _animator.SetTrigger("Flash");
    }

    /// <summary>
    /// Called to change the color of the outline depending of the alerte level of the room where player is.
    /// </summary>
    /// <param name="newAlerteLevel"> New alerte level of the room. </param>
    private void ChangeOutlineColor(AlerteLevel newAlerteLevel)
    {
        switch (newAlerteLevel)
        {
            case AlerteLevel.Patrol:
                _image.color = _patrolColor;
                break;
            case AlerteLevel.Research:
                _image.color = _researchColor;
                break;
            case AlerteLevel.Alerte:
                _image.color = _alerteColor;
                break;
        }
    }
}
