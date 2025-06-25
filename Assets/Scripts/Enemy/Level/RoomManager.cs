using System;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField]
    private Room[] _rooms;

    private void Start()
    {
        if (_rooms.Length > 0)
        {
            for (int i = 0; i < _rooms.Length; i++)
            {
                _rooms[i].OnPlayerIsInTheRoom += UpdateRooms;
            }
        }
    }

    /// <summary>
    /// Called to desactivate all rooms excepted those in a range of 3 around the current.
    /// </summary>
    /// <param name="currentRoom"> The current room where player is. </param>
    private void UpdateRooms(Room currentRoom)
    {
        int index = Array.IndexOf(_rooms, currentRoom);

        for (int i = 0; i < _rooms.Length; i++)
        {
            if (i >= index-3 && i <= index + 3)
            {
                _rooms[i].gameObject.SetActive(true);

                for (int j = 0; j < _rooms[i].EnemiesInRoom.Count; j++)
                {
                    _rooms[i].EnemiesInRoom[j].gameObject.SetActive(true);
                    _rooms[i].EnemiesInRoom[j].EnableEnemy();
                }
            }
            else
            {
                _rooms[i].gameObject.SetActive(false);

                for (int j = 0; j < _rooms[i].EnemiesInRoom.Count; j++)
                {
                    if (_rooms[i].EnemiesInRoom[j].gameObject.activeSelf)
                    {
                        StartCoroutine(_rooms[i].EnemiesInRoom[j].DisableEnemy());
                    }
                }
            }
        }
    }
}
