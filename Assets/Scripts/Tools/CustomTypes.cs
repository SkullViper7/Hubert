using UnityEngine;

public enum PatrolType
{
    LoopPatrol,
    PingPongPatrol,
    Fixed
}

public enum EnemyStateEnterType
{
    Null,
    HasAGoal,
    HasNoGoal,
    IsHit,
    IsShot
}

public enum VisionType
{
    Enemy,
    Camera
}

[System.Serializable]
public struct MinMaxInt
{
    public int Min;
    public int Max;
}

[System.Serializable]
public struct MinMaxFloat
{
    public float Min;
    public float Max;
}

[System.Serializable]
public struct WaypointInfos
{
    public Waypoint PreviousWaypoint;
    public int StepsForAccess;
    public float WaypointScore;
    public bool HasBeenClosed;
}

public readonly struct SoundSource
{
    public readonly int Id;
    public readonly Vector3 Position;

    public SoundSource(int id, Vector3 position)
    {
        Id = id;
        Position = position;
    }

    public override int GetHashCode() => Id;

    public override bool Equals(object obj) => obj is SoundSource other && Id == other.Id;
}