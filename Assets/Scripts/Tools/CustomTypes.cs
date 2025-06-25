using System;
using UnityEngine;

/// <summary>
/// The type of a patrol.
/// </summary>
public enum PatrolType
{
    LoopPatrol,
    PingPongPatrol,
    Fixed
}

/// <summary>
/// Context of the state enter.
/// </summary>
public enum EnemyStateEnterType
{
    Null,
    HasAGoal,
    HasAGoalButNoAstonishment,
    HasNoGoal,
    IsHit,
    IsShot
}

/// <summary>
/// Type of a vision.
/// </summary>
public enum VisionType
{
    Enemy,
    Camera
}

/// <summary>
/// Type of a sound.
/// </summary>
public enum SoundType
{
    OneShot,
    Continue
}

/// <summary>
/// Context when player is seen.
/// </summary>
public enum PlayerSeenContext
{
    FirstTime,
    Continue,
    LastTime
}

/// <summary>
/// Alerte level of an enemy or a room.
/// </summary>
public enum AlerteLevel
{
    Patrol,
    Research,
    Alerte,
    GeneralAlerte
}

/// <summary>
/// A minimal and a maximal integer value.
/// </summary>
[System.Serializable]
public struct MinMaxInt
{
    public int Min;
    public int Max;
}

/// <summary>
/// A minimal and a maximal float value.
/// </summary>
[System.Serializable]
public struct MinMaxFloat
{
    public float Min;
    public float Max;
}

/// <summary>
/// Informations about a waypoint.
/// </summary>
[System.Serializable]
public struct WaypointInfos
{
    public Waypoint PreviousWaypoint;
    public int StepsForAccess;
    public float WaypointScore;
    public bool HasBeenClosed;
}

/// <summary>
/// A face associated to a room for room entries.
/// </summary>
[Serializable]
public struct DoorFace
{
    public Color Color;
    public Vector3 LocalDirection;
    public Room AssociatedRoom;
}

/// <summary>
/// A source of a sound.
/// </summary>
public class SoundSource
{
    private static int _nextId = 0;

    public int Id { get; }
    public Vector3 Position { get; set; }
    public SoundType SoundType { get; set; }
    public int Listeners { get; set; }
    public bool IsPushedByAnEnemy { get; set; }
    public event Action OnReached;

    public SoundSource(Vector3 position, SoundType soundType, bool isPushedByAnEnemy, int listeners = 0)
    {
        Id = _nextId++;
        Position = position;
        SoundType = soundType;
        Listeners = listeners;
        IsPushedByAnEnemy = isPushedByAnEnemy;
    }

    public override int GetHashCode() => Id;

    public override bool Equals(object obj) => obj is SoundSource other && Id == other.Id;

    /// <summary>
    /// Triggers all listeners subscribed to this position.
    /// </summary>
    public void Invoke()
    {
        OnReached?.Invoke();
    }
}

/// <summary>
/// A known position of the player.
/// </summary>
public class PlayerPosition
{
    private static int _nextId = 0;

    public int Id { get; }
    public Vector3 Position { get; set; }
    public PlayerSeenContext PlayerSeenContext { get; set; }
    public int Listeners { get; set; }
    public event Action OnReached;

    public PlayerPosition(Vector3 position, PlayerSeenContext playerSeenContext, int listeners = 0)
    {
        Id = _nextId++;
        Position = position;
        PlayerSeenContext = playerSeenContext;
        Listeners = listeners;
    }

    public override int GetHashCode() => Id;

    public override bool Equals(object obj) => obj is PlayerPosition other && Id == other.Id;

    /// <summary>
    /// Triggers all listeners subscribed to this position.
    /// </summary>
    public void Invoke()
    {
        OnReached?.Invoke();
    }
}