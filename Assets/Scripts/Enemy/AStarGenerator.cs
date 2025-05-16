using System.Collections.Generic;
using UnityEngine;

public static class AStarGenerator
{
    /// <summary>
    /// List which contains current open waypoints.
    /// </summary>
    private static List<Waypoint> s_openWaypoints = new();

    /// <summary>
    /// Dictionary which contains infos about waypoints.
    /// </summary>
    private static readonly Dictionary<Waypoint, WaypointInfos> s_waypointInfos = new();

    /// <summary>
    /// List which contains potential waypoints at the ping-pong distance given.
    /// </summary>
    private static readonly List<Waypoint> s_potentialPingPongWaypoints = new();

    /// <summary>
    /// The minimum distance to reach before to come back to the originx (in steps).
    /// </summary>
    private static int s_minDistance;

    /// <summary>
    /// The maximum distance reachable to do a loop (in steps).
    /// </summary>
    private static int s_maxDistance;

    /// <summary>
    /// The distance for a ping-pong if no loop is found.
    /// </summary>
    private static int s_pingPongDistance;

    /// <summary>
    /// A value indicating if the targeted waypoint is available or not (if the minimum distance is reached).
    /// </summary>
    private static bool s_isTargetOpen;

    /// <summary>
    /// The origin waypoint given.
    /// </summary>
    private static Waypoint s_originWaypoint;

    /// <summary>
    /// The new origin waypoint, could be the same as the origin if it's possible.
    /// </summary>
    private static Waypoint s_newOriginWaypoint;

    /// <summary>
    /// Called to get a path around a point given.
    /// </summary>
    /// <param name="originWaypoint"> The waypoint which is the origin of the path. </param>
    /// <param name="minDistance"> The minimum distance to deviate from the start. </param>
    /// <param name="maxDistance"> The maximum distance reachable to do a loop. </param>
    /// <param name="pingPongDistance"> The distance for a ping-pong if no loop is found. </param>
    /// <returns></returns>
    public static (List<Waypoint>, PatrolType) GetPatrolAround(Waypoint originWaypoint, int minDistance, int maxDistance, int pingPongDistance)
    {
        // Reset all
        s_openWaypoints.Clear();
        s_waypointInfos.Clear();
        s_potentialPingPongWaypoints.Clear();

        // Set new values
        s_originWaypoint = originWaypoint;
        s_minDistance = minDistance;
        s_maxDistance = maxDistance;
        s_pingPongDistance = pingPongDistance;

        // Close original waypoint
        SetWaypointCloseState(s_originWaypoint, true);

        // Determine the waypoint at which the enemy will try to come back
        // Return true if there is no possibility for a loop
        (Waypoint, bool) gettingTargetedWaypointResult = GetTargetedWaypoint(s_originWaypoint);

        // If true
        if (gettingTargetedWaypointResult.Item2)
        {
            // Return the ping-pong
            return (GetPath(gettingTargetedWaypointResult.Item1, s_originWaypoint, new()), PatrolType.PingPongPatrol);
        }
        else
        {
            // Start calculate a potential loop
            return CalculatePath(s_newOriginWaypoint, gettingTargetedWaypointResult.Item1);
        }
    }

    #region First step
    /// <summary>
    /// Called to get the targeted waypoint, the last of the loop or the ping-pong and the information about if it is arleady a ping-pong patrol or not.
    /// </summary>
    /// <param name="currentWaypoint"> Current waypoint checked. </param>
    /// <param name="currentWaypoint"> Previous waypoint checked. </param>
    /// <returns></returns>
    private static (Waypoint, bool) GetTargetedWaypoint(Waypoint currentWaypoint)
    {
        // If the steps access value is equal to the ping-pong value, add it to the list
        if (s_waypointInfos[currentWaypoint].StepsForAccess == s_pingPongDistance)
        {
            s_potentialPingPongWaypoints.Add(currentWaypoint);
        }

        // If the maximum distance is exceeded return a ping-pong waypoint
        if (s_waypointInfos[currentWaypoint].StepsForAccess > s_maxDistance)
        {
            // Normally there is necessarily one ping-pong waypoint
            (Waypoint, bool) randomPingPongResult = GetRandomPingPongWaypoint();
            if (randomPingPongResult.Item2)
            {
                return (randomPingPongResult.Item1, true);
            }
            // Not supposed to happen
            else
            {
                Debug.LogError("Works but not supposed to.");
                return (currentWaypoint, true);
            }
        }

        // Get the neighbor list and remove the previous waypoint from the list
        List<Waypoint> neighbors = currentWaypoint.Neighbors;

        for (int i = 0; i < neighbors.Count; i++)
        {
            if (s_waypointInfos[currentWaypoint].PreviousWaypoint == neighbors[i])
            {
                neighbors.RemoveAt(i);
            }
        }

        // If there is more than one neighbors chose a random one or continue with the only one
        // If there is no neighbors left, return the current waypoint and it will be a ping-pong patrol
        if (neighbors.Count > 1)
        {
            s_newOriginWaypoint = currentWaypoint;
            return (neighbors[Random.Range(0, neighbors.Count)], false);
        }
        else if (neighbors.Count == 1)
        {
            SetPreviousWaypoint(neighbors[0], currentWaypoint);
            SetWaypointStepsForAccess(neighbors[0], s_waypointInfos[currentWaypoint].StepsForAccess + 1);
            SetWaypointCloseState(neighbors[0], true);
            return GetTargetedWaypoint(neighbors[0]);
        }
        else
        {
            return (currentWaypoint, true);
        }
    }
    #endregion

    #region Second step
    /// <summary>
    /// Called to open neighbors of the current waypoint, calculate their scores and continue with the best until we reach the target.
    /// </summary>
    /// <param name="currentWaypoint"> Current waypoint checked. </param>
    /// <param name="targetedWaypoint"> Targeted waypoint that we want to reach. </param>
    /// <returns></returns>
    private static (List<Waypoint>, PatrolType) CalculatePath(Waypoint currentWaypoint, Waypoint targetedWaypoint)
    {
        // If the steps access value of the current waypoint is equal or greater than the minimum distance, the target is open
        s_isTargetOpen = s_waypointInfos[currentWaypoint].StepsForAccess >= s_minDistance;

        // As long as the current waypoint is not the targeted waypoint and the minimum distance is not reached we continue
        if (currentWaypoint != targetedWaypoint || !s_isTargetOpen)
        {
            // Close the current waypoint
            if (s_openWaypoints.Contains(currentWaypoint))
            {
                s_openWaypoints.Remove(currentWaypoint);
            }
            SetWaypointCloseState(currentWaypoint, true);

            // If the current waypoint has neighbors
            if (currentWaypoint.Neighbors.Count > 0)
            {
                // For each neighbor
                for (int i = 0; i < currentWaypoint.Neighbors.Count; i++)
                {
                    Waypoint currentNeighbor = currentWaypoint.Neighbors[i];

                    // Ignore target as much as the minimum distance is not reached
                    if (currentNeighbor == targetedWaypoint && !s_isTargetOpen) continue;

                    // If the neighbor is not already open we open it and calculate its score
                    if (s_openWaypoints.Contains(currentNeighbor)) continue;
                    if (s_waypointInfos.ContainsKey(currentNeighbor))
                    {
                        if (s_waypointInfos[currentNeighbor].HasBeenClosed) continue;
                    }
                    else
                    {
                        // Open waypoint and calculate its score
                        s_openWaypoints.Add(currentNeighbor);
                        SetPreviousWaypoint(currentNeighbor, currentWaypoint);
                        CalculateWaypointScore(currentNeighbor, targetedWaypoint);
                    }
                }

                // Finally, try to get the best open waypoint and perform again with it
                // If there is not, return a ping-pong path
                (Waypoint, bool) nextWaypointResult = GetBestOpenWaypoint(targetedWaypoint);
                if (nextWaypointResult.Item2)
                {
                    return CalculatePath(nextWaypointResult.Item1, targetedWaypoint);
                }
                else
                {
                    // If there is ping-pong waypoints get during the calculation return one, else return the last waypoint reached
                    (Waypoint, bool) randomPingPongResult = GetRandomPingPongWaypoint();
                    if (randomPingPongResult.Item2)
                    {
                        return (GetPath(randomPingPongResult.Item1, s_originWaypoint, new()), PatrolType.PingPongPatrol);
                    }
                    else
                    {
                        return (GetPath(currentWaypoint, s_originWaypoint, new()), PatrolType.PingPongPatrol);
                    }
                }
            }
            // If the current waypoint doesn't have neighbors
            else
            {
                // Try to get the best open waypoint and perform again with it
                // If there is not, return a ping-pong path
                (Waypoint, bool) nextWaypointResult = GetBestOpenWaypoint(targetedWaypoint);
                if (nextWaypointResult.Item2)
                {
                    return CalculatePath(nextWaypointResult.Item1, targetedWaypoint);
                }
                else
                {
                    // If there is ping-pong waypoints get during the calculation return one, else return the last waypoint reached
                    (Waypoint, bool) randomPingPongResult = GetRandomPingPongWaypoint();
                    if (randomPingPongResult.Item2)
                    {
                        return (GetPath(randomPingPongResult.Item1, s_originWaypoint, new()), PatrolType.PingPongPatrol);
                    }
                    else
                    {
                        return (GetPath(currentWaypoint, s_originWaypoint, new()), PatrolType.PingPongPatrol);
                    }
                }
            }
        }
        // If its the target and if it's open, return the loop
        else
        {
            return (GetPath(currentWaypoint, s_newOriginWaypoint, new()), PatrolType.LoopPatrol);
        }
    }
    #endregion

    #region Third step
    /// <summary>
    /// Called to calculate the score of a waypoint.
    /// </summary>
    /// <param name="currentWaypoint"> The waypoint for which the score must be calculated. </param>
    /// <param name="targetedWaypoint"> The targeted waypoint of the patrol. </param>
    private static void CalculateWaypointScore(Waypoint currentWaypoint, Waypoint targetedWaypoint)
    {
        // If it is the target set the score to 0
        if (currentWaypoint == targetedWaypoint)
        {
            SetWaypointStepsForAccess(currentWaypoint, s_waypointInfos[s_waypointInfos[currentWaypoint].PreviousWaypoint].StepsForAccess + 1);
            SetWaypointScore(currentWaypoint, 0);
        }
        else
        {
            // H is the distance as the crow flies between the current waypoint and the targeted waypoint
            float h = Vector3.Distance(currentWaypoint.transform.position, targetedWaypoint.transform.position);

            // G is equal to the number of waypoints between the very start and the current waypoint
            SetWaypointStepsForAccess(currentWaypoint, s_waypointInfos[s_waypointInfos[currentWaypoint].PreviousWaypoint].StepsForAccess + 1);

            // Add if it is a ping-pong distance waypoint
            if (s_waypointInfos[currentWaypoint].StepsForAccess == s_pingPongDistance)
            {
                s_potentialPingPongWaypoints.Add(currentWaypoint);
            }

            int g = s_waypointInfos[currentWaypoint].StepsForAccess;

            // Waypoint score is equal to h + g and represent a score for distance between the very start and this waypoint
            SetWaypointScore(currentWaypoint, h + g);
        }
    }
    #endregion

    #region Fourth step
    /// <summary>
    /// Called to remove to far open waypoints from the list before returning the best.
    /// </summary>
    /// <param name="targetedWaypoint"> The targeted waypoint of the path. </param>
    private static void RemoveToFarOpenWaypoints(Waypoint targetedWaypoint)
    {
        List<Waypoint> valideWaypoints = new();

        // For each open waypoint
        for (int i = 0; i < s_openWaypoints.Count; i++)
        {
            // If its steps access value exceeds the maximum distance or if it is the targeted waypoint, we keep it
            if (s_waypointInfos[s_openWaypoints[i]].StepsForAccess < s_maxDistance || s_openWaypoints[i] == targetedWaypoint)
            {
                valideWaypoints.Add(s_openWaypoints[i]);
            }
        }

        s_openWaypoints = valideWaypoints;
    }

    /// <summary>
    /// Called to get the best open waypoint if it's possible.
    /// </summary>
    /// <param name="targetedWaypoint"> The targeted waypoint of the path. </param>
    /// <returns></returns>
    private static (Waypoint, bool) GetBestOpenWaypoint(Waypoint targetedWaypoint)
    {
        // Remove to far open waypoint before
        RemoveToFarOpenWaypoints(targetedWaypoint);

        // If there is many open waypoint, return the closest one.
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

            return (s_openWaypoints[closestScoreIndice], true);
        }
        // If there is only one, return it
        else if (s_openWaypoints.Count == 1)
        {
            return (s_openWaypoints[0], true);
        }
        // If there is not, return false and it will be a ping-pong patrol
        else
        {
            return (null, false);
        }
    }

    /// <summary>
    /// Called when maximum distance is exceeded to return a intermediate value for a ping-pong if it's possible.
    /// </summary>
    /// <returns></returns>
    private static (Waypoint, bool) GetRandomPingPongWaypoint()
    {
        if (s_potentialPingPongWaypoints.Count > 0)
        {
            return (s_potentialPingPongWaypoints[Random.Range(0, s_potentialPingPongWaypoints.Count)], true);
        }
        else
        {
            return (null, false);
        }
    }
    #endregion

    #region Fifth step
    /// <summary>
    /// Called to get the complete path.
    /// </summary>
    /// <param name="currentWaypoint"> Last waypoint found. </param>
    /// <param name="targetedWaypoint"> Origin waypoint used for the path. </param>
    /// <param name="path"> The path to return. </param>
    /// <returns></returns>
    private static List<Waypoint> GetPath(Waypoint currentWaypoint, Waypoint targetedWaypoint, List<Waypoint> path)
    {
        // Go back up the path from the last waypoint found
        Waypoint previousWaypoint = s_waypointInfos[currentWaypoint].PreviousWaypoint;

        // Until it's not the target
        if (currentWaypoint != targetedWaypoint)
        {
            path.Add(currentWaypoint);
            return GetPath(previousWaypoint, targetedWaypoint, path);
        }
        else
        {
            path.Add(currentWaypoint);

            // Reverse to get the path in the good order
            path.Reverse();
            return path;
        }
    }
    #endregion

    #region Tools
    /// <summary>
    /// Called to try to add a waypoint to the list of waypoints with infos.
    /// </summary>
    /// <param name="newWaypoint"> The new waypoint to add. </param>
    private static void TryCreateNewWaypointInfos(Waypoint newWaypoint)
    {
        if (s_waypointInfos.ContainsKey(newWaypoint))
            return;
        else
        {
            s_waypointInfos.Add(newWaypoint, new WaypointInfos());
        }
    }

    /// <summary>
    /// Called to set the previous waypoint of a waypoint.
    /// </summary>
    /// <param name="waypoint"> The waypoint to set. </param>
    /// <param name="previousWaypoint"> The previous waypoint. </param>
    private static void SetPreviousWaypoint(Waypoint waypoint, Waypoint previousWaypoint)
    {
        // Ensure that the entry exists
        TryCreateNewWaypointInfos(waypoint);

        WaypointInfos infos = s_waypointInfos[waypoint];
        infos.PreviousWaypoint = previousWaypoint;
        s_waypointInfos[waypoint] = infos;
    }

    /// <summary>
    /// Called to set the steps for access value of a waypoint.
    /// </summary>
    /// <param name="waypoint"> The waypoint to set. </param>
    /// <param name="stepsForAccess"> The value of the setting. </param>
    private static void SetWaypointStepsForAccess(Waypoint waypoint, int stepsForAccess)
    {
        // Ensure that the entry exists
        TryCreateNewWaypointInfos(waypoint);

        WaypointInfos infos = s_waypointInfos[waypoint];
        infos.StepsForAccess = stepsForAccess;
        s_waypointInfos[waypoint] = infos;
    }

    /// <summary>
    /// Called to set the scoree of a waypoint.
    /// </summary>
    /// <param name="waypoint"> The waypoint to set. </param>
    /// <param name="score"> The value of the setting. </param>
    private static void SetWaypointScore(Waypoint waypoint, float score)
    {
        // Ensure that the entry exists
        TryCreateNewWaypointInfos(waypoint);

        WaypointInfos infos = s_waypointInfos[waypoint];
        infos.WaypointScore = score;
        s_waypointInfos[waypoint] = infos;
    }

    /// <summary>
    /// Called to set the close state of a waypoint.
    /// </summary>
    /// <param name="waypoint"> The waypoint to set. </param>
    /// <param name="closeState"> The value of the setting. </param>
    private static void SetWaypointCloseState(Waypoint waypoint, bool closeState)
    {
        // Ensure that the entry exists
        TryCreateNewWaypointInfos(waypoint);

        WaypointInfos infos = s_waypointInfos[waypoint];
        infos.HasBeenClosed = closeState;
        s_waypointInfos[waypoint] = infos;
    }
    #endregion
}