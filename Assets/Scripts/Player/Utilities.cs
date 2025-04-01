using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class Utilities
{
    /// <summary>
    /// Called to get a corrected normal if the player is close to an edge.
    /// </summary>
    /// <param name="normal"> Normal of the surface. </param>
    /// <param name="wallTransform"> Transform of the wall. </param>
    /// <returns></returns>
    public static Vector3 GetCorrectedNormal(Vector3 normal, Transform wallTransform)
    {
        // List of possible normals
        Vector3[] possibleNormals =
        {
        wallTransform.forward,
        -wallTransform.forward,
        wallTransform.right,
        -wallTransform.right
    };

        // Find the nearest normal
        Vector3 bestMatch = possibleNormals[0];
        float maxDot = Vector3.Dot(normal, bestMatch);

        for (int i = 1; i < possibleNormals.Length; i++)
        {
            float dot = Vector3.Dot(normal, possibleNormals[i]);
            if (dot > maxDot)
            {
                maxDot = dot;
                bestMatch = possibleNormals[i];
            }
        }

        return SimplifyVector(bestMatch);
    }

    /// <summary>
    /// Called to simplify a vector.
    /// </summary>
    /// <param name="vector"> The vector to simplify. </param>
    /// <returns></returns>
    private static Vector3 SimplifyVector(Vector3 vector)
    {
        return new Vector3(
            Mathf.Round(vector.x),
            Mathf.Round(vector.y),
            Mathf.Round(vector.z)
        );
    }

    /// <summary>
    /// Called to get the correct sticked position on a wall.
    /// </summary>
    /// <param name="position"> Position we want to correct. </param>
    /// <param name="normal"> Normal of the surface. </param>
    /// <param name="wallCollider"> Wall on witch we want to stick. </param>
    /// <param name="characterController"> Character controller of the player. </param>
    /// <returns></returns>
    public static Vector3 GetCorrectPosition(Vector3 position, Vector3 normal, BoxCollider wallCollider, CharacterController characterController)
    {
        Vector3 bestPosition = position;

        // Get the wall dimensions
        Vector3 wallSize = wallCollider.bounds.size;
        Vector3 wallPosition = wallCollider.transform.position;

        if (normal.x != 0f)
        {
            bestPosition.z = Mathf.Clamp(bestPosition.z, wallPosition.z - wallSize.z / 2 + characterController.radius, wallPosition.z + wallSize.z / 2 - characterController.radius);
        }
        else if (normal.z != 0f)
        {
            bestPosition.x = Mathf.Clamp(bestPosition.x, wallPosition.x - wallSize.x / 2 + characterController.radius, wallPosition.x + wallSize.x / 2 - characterController.radius);
        }

        return bestPosition;
    }

    /// <summary>
    /// Called to sort walls near to the player. We remove the surfaces that are too short and we remove the surfaces that overlap.
    /// </summary>
    /// <param name="walls"> Walls we want to sort. </param>
    /// <param name="playerTransform"> Transform of the player. </param>
    /// <param name="characterController"> Character controller of the player. </param>
    /// <returns></returns>
    public static List<BoxCollider> SortWalls(List<BoxCollider> walls, Transform playerTransform, CharacterController characterController)
    {
        // Stock walls and nearest position of the player on the wall
        List<BoxCollider> sortedWalls = new();
        Dictionary<BoxCollider, Vector3> wallPoints = new();

        for (int i = 0; i < walls.Count; i++)
        {
            sortedWalls.Add(walls[i]);
            Vector3 pointOnWall = walls[i].ClosestPoint(playerTransform.position);
            wallPoints[walls[i]] = pointOnWall;
        }

        // Sort too short surfaces
        for (int i = 0; i < walls.Count; i++)
        {
            Vector3 normal = GetCorrectedNormal(playerTransform.position - wallPoints[walls[i]], walls[i].transform);

            if (normal.x != 0f)
            {
                if (walls[i].bounds.size.z < characterController.radius * 2)
                {
                    sortedWalls.Remove(walls[i]);
                    wallPoints.Remove(walls[i]);
                }
            }
            else if (normal.z != 0f)
            {
                if (walls[i].bounds.size.x < characterController.radius * 2)
                {
                    sortedWalls.Remove(walls[i]);
                    wallPoints.Remove(walls[i]);
                }
            }
        }

        // Sort surfaces that overlap
        foreach (KeyValuePair<BoxCollider, Vector3> wall1 in wallPoints)
        {
            foreach (KeyValuePair<BoxCollider, Vector3> wall2 in wallPoints)
            {
                if (wall1.Key == wall2.Key) continue;

                Vector3 normal1 = GetCorrectedNormal(playerTransform.position - wall1.Value, wall1.Key.transform);
                Vector3 normal2 = GetCorrectedNormal(playerTransform.position - wall2.Value, wall2.Key.transform);

                // Check if the points are close and in the same direction
                if (Vector3.Distance(wall1.Value, wall2.Value) < 0.01f && Vector3.Dot(normal1, normal2) > 0.95f)
                {
                    float width1 = 0f;
                    float width2 = 0f;

                    if (normal1.x != 0f)
                    {
                        width1 = wall1.Key.bounds.size.z;
                        width2 = wall2.Key.bounds.size.z;
                    }
                    else if (normal1.z != 0f)
                    {
                        width1 = wall1.Key.bounds.size.x;
                        width2 = wall2.Key.bounds.size.x;
                    }

                    // Keep the widest wall
                    if (width1 > width2)
                    {
                        if(walls.Contains(wall2.Key))
                        {
                            sortedWalls.Remove(wall2.Key);
                        }
                    }
                    else
                    {
                        if (walls.Contains(wall1.Key))
                        {
                            sortedWalls.Remove(wall1.Key);
                        }
                    }
                }
            }
        }

        return sortedWalls;
    }

    /// <summary>
    /// Called to know if a way is clear or not.
    /// </summary>
    /// <param name="wallCollider"> Collider of the which on we want to stick. </param>
    /// <param name="stickedPosition"> Position that we want to rich. </param>
    /// <param name="playerTransform"> Transform of the player. </param>
    /// <param name="characterController"> Character controller of the player. </param>
    /// <returns></returns>
    public static bool IsWayClear(BoxCollider wallCollider, Vector3 stickedPosition, Transform playerTransform, CharacterController characterController)
    {
        Vector3 playerPositionOnGround = playerTransform.position - new Vector3(0, characterController.height / 2 - 0.1f, 0);
        Vector3 stickedPositionOnGround = stickedPosition - new Vector3(0, characterController.height / 2 - 0.1f, 0);

        Vector3 waydirection = (stickedPositionOnGround - playerPositionOnGround).normalized;

        RaycastHit hit;

        // Check center
        if (Physics.Raycast(playerPositionOnGround, waydirection, out hit, 5f))
        {
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Wall") && hit.collider.gameObject.layer != LayerMask.NameToLayer("Breakable"))
            {
                return false;
            }
        }

        // Check on right
        Vector3 playerPositionOnGroundOnRight = playerPositionOnGround + Vector3.Cross(Vector3.up, waydirection).normalized * characterController.radius;

        if (Physics.Raycast(playerPositionOnGroundOnRight, waydirection, out hit, 5f))
        {
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Wall") && hit.collider.gameObject.layer != LayerMask.NameToLayer("Breakable"))
            {
                return false;
            }
        }

        // Check on left
        Vector3 playerPositionOnGroundOnLeft = playerPositionOnGround - Vector3.Cross(Vector3.up, waydirection).normalized * characterController.radius;

        if (Physics.Raycast(playerPositionOnGroundOnLeft, waydirection, out hit, 5f))
        {
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Wall") && hit.collider.gameObject.layer != LayerMask.NameToLayer("Breakable"))
            {
                return false;
            }
        }

        return true;
    }
}
