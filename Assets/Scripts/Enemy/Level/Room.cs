using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField]
    private List<RoomDoor> _doors;

    [SerializeField]
    private List<EnemyBrain> _enemiesInRoom;

    private void Start()
    {
        for (int i = 0; i < _doors.Count; i++)
        {
            _doors[i].OnEnemyEnter += TryAddEnemy;
            _doors[i].OnEnemyExit += TryRemoveEnemy;
        }
    }

    private void TryAddEnemy(EnemyBrain enemy)
    {
        if (!_enemiesInRoom.Contains(enemy))
        {
            _enemiesInRoom.Add(enemy);
        }
    }

    private void TryRemoveEnemy(EnemyBrain enemy)
    {
        if (_enemiesInRoom.Contains(enemy))
        {
            _enemiesInRoom.Remove(enemy);
        }
    }
}
