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
    HasNoGoal
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