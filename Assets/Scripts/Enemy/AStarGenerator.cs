using System.Collections.Generic;
using UnityEngine;

public static class AStarGenerator
{
    private static List<Waypoint> s_openWaypoints = new();
    private static Dictionary<Waypoint, WaypointInfos> s_waypointInfos = new();
    private static int s_minDistance;
    private static int s_maxDistance;
    private static int s_pingPongDistance;
    private static int _currentStep;

    private static bool s_isTargetOpen;

    private static Waypoint s_originWaypoint;
    private static Waypoint s_targetedWaypoint;

    private static PatrolType s_patrolType;

    /// <summary>
    /// Called to get a path around a point given.
    /// </summary>
    /// <param name="targetedWaypoint"> The waypoint which is the departure and the arrival of the path. </param>
    /// <param name="minDistance"> The minimum distance to deviate from the start. </param>
    /// <returns></returns>
    public static List<Waypoint> GetPatrolAround(Waypoint targetedWaypoint, int minDistance, int maxDistance, int pingPongDistance)
    {
        // Reset all
        s_openWaypoints.Clear();
        s_waypointInfos.Clear();
        _currentStep = 0;

        // Set values
        s_originWaypoint = targetedWaypoint;
        s_minDistance = minDistance;
        s_maxDistance = maxDistance;
        s_pingPongDistance = pingPongDistance;

        // Determine the waypoint at which the enemy will start
        s_targetedWaypoint = GetStartingWaypoint(s_originWaypoint);

        // Start calculate a new path
        return CalculatePath(targetedWaypoint, targetedWaypoint, _currentStep);
    }

    private static List<Waypoint> CalculatePath(Waypoint currentWaypoint, Waypoint targetedWaypoint, int currentDistance)
    {
        s_isTargetOpen = currentDistance >= s_minDistance;

        // As long as the current waypoint is not the targeted waypoint and the minimum distance is not reached we continue
        if (currentWaypoint != targetedWaypoint || currentDistance < s_minDistance)
        {
            // Remove the current waypoint from open waypoints because it is closed
            if (s_openWaypoints.Contains(currentWaypoint))
            {
                s_openWaypoints.Remove(currentWaypoint);
            }
            if (s_waypointInfos.ContainsKey(currentWaypoint))
            {
                WaypointInfos infos = s_waypointInfos[currentWaypoint];
                infos.HasBeenClosed = true;
                s_waypointInfos[currentWaypoint] = infos;
            }

            // If the current waypoint has neighbors
            if (currentWaypoint.Neighbors.Count > 0)
            {
                // For each neighbor
                for (int i = 0; i < currentWaypoint.Neighbors.Count; i++)
                {
                    Waypoint currentNeighbor = currentWaypoint.Neighbors[i];

                    // Ignore target as much as the min distance is not reached
                    if (currentNeighbor == targetedWaypoint && !s_isTargetOpen) continue;

                    // If the neighbor is not already open we open it and calculate its score
                    if (s_openWaypoints.Contains(currentNeighbor)) continue;
                    if (s_waypointInfos.ContainsKey(currentNeighbor))
                    {
                        if (s_waypointInfos[currentNeighbor].HasBeenClosed) continue;
                    }
                    else
                    {
                        // Open waypoint
                        s_openWaypoints.Add(currentNeighbor);
                        s_waypointInfos.Add(currentNeighbor, new WaypointInfos());
                        WaypointInfos infos = s_waypointInfos[currentNeighbor];
                        infos.PreviousWaypoint = currentWaypoint;
                        s_waypointInfos[currentNeighbor] = infos;

                        CalculateWaypointScore(currentNeighbor, targetedWaypoint);
                    }
                }

                // Finally, perform again this action with the closest open waypoint
                Waypoint nextWaypoint = GetBestOpenedWaypoint();
                if (nextWaypoint == null) return null;
                return CalculatePath(nextWaypoint, targetedWaypoint, s_waypointInfos[nextWaypoint].StepsForAccess);
            }
            else
            {
                // If the current waypoint doesn't have neighbors, perform again this action with the other closest open waypoint
                Waypoint nextWaypoint = GetBestOpenedWaypoint();
                if (nextWaypoint == null) return null;
                return CalculatePath(GetBestOpenedWaypoint(), targetedWaypoint, s_waypointInfos[nextWaypoint].StepsForAccess);
            }
        }
        else
        {
            return GetPath(currentWaypoint, targetedWaypoint, new());
        }
    }

    /// <summary>
    /// Called to calculate the score of a waypoint.
    /// </summary>
    /// <param name="currentWaypoint"> The waypoint for which the score must be calculated. </param>
    /// <param name="targetedWaypoint"> The targeted waypoint of the patrol. </param>
    private static void CalculateWaypointScore(Waypoint currentWaypoint, Waypoint targetedWaypoint)
    {
        WaypointInfos infos;

        // H is the distance as the crow flies between the current waypoint and the targeted waypoint
        float h = Vector3.Distance(currentWaypoint.transform.position, targetedWaypoint.transform.position);

        // G is equal to the number of waypoints between the very start and the current waypoint
        if (s_waypointInfos.ContainsKey(s_waypointInfos[currentWaypoint].PreviousWaypoint))
        {
            infos = s_waypointInfos[currentWaypoint];
            infos.StepsForAccess = s_waypointInfos[infos.PreviousWaypoint].StepsForAccess + 1;
            s_waypointInfos[currentWaypoint] = infos;
        }
        else
        {
            infos = s_waypointInfos[currentWaypoint];
            infos.StepsForAccess = 1;
            s_waypointInfos[currentWaypoint] = infos;
        }

        int g = s_waypointInfos[currentWaypoint].StepsForAccess;

        // Waypoint score is equal to h + g and represent a score for distance between the very start and this waypoint
        infos = s_waypointInfos[currentWaypoint];
        infos.WaypointScore = h + g;
        s_waypointInfos[currentWaypoint] = infos;
    }

    /// <summary>
    /// Called to get the best opened waypoint.
    /// </summary>
    /// <returns></returns>
    private static Waypoint GetBestOpenedWaypoint()
    {
        if (s_openWaypoints.Count > 1)
        {
            int closestScoreIndice = 0;
            float minScore = float.MaxValue;

            for (int i = 0; i < s_openWaypoints.Count; i++)
            {
                if (s_waypointInfos[s_openWaypoints[i]].WaypointScore < minScore)
                {
                    minScore = s_waypointInfos[s_openWaypoints[i]].WaypointScore;
                    closestScoreIndice = i;
                }
            }

            return s_openWaypoints[closestScoreIndice];
        }
        else
        {
            Debug.LogError("No open waypoint in the list.");
            return null;
        }
    }

    /// <summary>
    /// Called to get a path from an arrival to a
    /// </summary>
    /// <param name="currentWaypoint"> Last waypoint found. </param>
    /// <param name="departureWaypoint"> Original waypoint used for the path. </param>
    /// <param name="path"></param>
    /// <returns></returns>
    private static List<Waypoint> GetPath(Waypoint currentWaypoint, Waypoint departureWaypoint, List<Waypoint> path)
    {
        // Go back up the path from the last waypoint found
        Waypoint previousWaypoint = s_waypointInfos[currentWaypoint].PreviousWaypoint;

        if (previousWaypoint != departureWaypoint)
        {
            path.Add(previousWaypoint);
            return GetPath(previousWaypoint, departureWaypoint, path);
        }
        else
        {
            return path;
        }
    }

    private static Waypoint GetStartingWaypoint(Waypoint currentWaypoint)
    {
        // If there is neighbors chose a random one or check with the next one
        if (currentWaypoint.Neighbors.Count > 1)
        {
            _currentStep++;
            return currentWaypoint.Neighbors[Random.Range(0, currentWaypoint.Neighbors.Count-1)];
        }
        else
        {
            _currentStep++;
            return GetStartingWaypoint(currentWaypoint.Neighbors[0]);
        }
    }
}
