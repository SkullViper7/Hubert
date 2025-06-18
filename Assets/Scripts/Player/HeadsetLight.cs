using UnityEngine;

public class HeadsetLight : MonoBehaviour
{
    /// <summary>
    /// Material when enemy is in patrol state.
    /// </summary>
    [SerializeField]
    private Material _patrolMaterial;

    /// <summary>
    /// Material when enemy is in research state.
    /// </summary>
    [SerializeField]
    private Material _researchMaterial;

    /// <summary>
    /// Material when enemy is in alerte state.
    /// </summary>
    [SerializeField]
    private Material _alerteMaterial;

    /// <summary>
    /// Reference to the player.
    /// </summary>
    [SerializeField]
    private PlayerStateManager _player;

    /// <summary>
    /// Mesh renderer of the light.
    /// </summary>
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        _meshRenderer.material = _patrolMaterial;

        _player.OnRoomChanged += PlayerHasChangedRoom;
        _player.CurrentRoom.OnRoomAlerteLevelChanged += ChangeLightColor;
    }

    /// <summary>
    /// Called when the player changes room to remove old listeners and set new.
    /// </summary>
    private void PlayerHasChangedRoom(Room oldRoom, Room newRoom)
    {
        oldRoom.OnRoomAlerteLevelChanged -= ChangeLightColor;
        newRoom.OnRoomAlerteLevelChanged += ChangeLightColor;

        ChangeLightColor(newRoom.RoomAlerteLevel);
    }
    /// <summary>
    /// Called to change the color of the light depending of the alerte level of the room where player is.
    /// </summary>
    /// <param name="newAlerteLevel"> New alerte level of the room. </param>
    private void ChangeLightColor(AlerteLevel newAlerteLevel)
    {
        switch (newAlerteLevel)
        {
            case AlerteLevel.Patrol:
                _meshRenderer.material = _patrolMaterial;
                break;
            case AlerteLevel.Research:
                _meshRenderer.material = _researchMaterial;
                break;
            case AlerteLevel.Alerte:
                _meshRenderer.material = _alerteMaterial;
                break;
        }
    }
}
