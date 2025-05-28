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
public struct SoundSource
{
    public readonly int Id;
    public readonly Vector3 Position;
    public readonly SoundType SoundType;
    public int Listeners;

    public SoundSource(int id, Vector3 position, SoundType soundType, int listeners)
    {
        Id = id;
        Position = position;
        SoundType = soundType;
        Listeners = listeners;
    }

    public override int GetHashCode() => Id;

    public override bool Equals(object obj) => obj is SoundSource other && Id == other.Id;
}

/// <summary>
/// A known position of the player.
/// </summary>
public readonly struct PlayerPosition
{
    public readonly int Id;
    public readonly Vector3 Position;
    public readonly PlayerSeenContext PlayerSeenContext;

    public PlayerPosition(int id, Vector3 position, PlayerSeenContext playerSeenContext)
    {
        Id = id;
        Position = position;
        PlayerSeenContext = playerSeenContext;
    }

    public override int GetHashCode() => Id;

    public override bool Equals(object obj) => obj is PlayerPosition other && Id == other.Id;
}