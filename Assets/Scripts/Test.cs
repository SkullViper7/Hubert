using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Waypoint Waypoint;

    public int minDistance;

    public int maxDistance;

    public int pingPongDistance;

    public List<Waypoint> path = new();

    public PatrolType patrolType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        (List<Waypoint>, PatrolType) returnedPath = AStarGenerator.GetPatrolAround(Waypoint, minDistance, maxDistance, pingPongDistance);
        path = returnedPath.Item1;
        patrolType = returnedPath.Item2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
