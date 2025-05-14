public enum PatrolType
{
    LoopPatrol,
    PingPongPatrol,
    Fixed
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
