using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Waypoint Waypoint;

    public List<Waypoint> path = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //path = AStarGenerator.GetPatrolAround(Waypoint, 3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
