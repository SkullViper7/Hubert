using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public static class Utilities
{
    /// <summary>
    /// Called to get the closest side of a wall from a normal.
    /// </summary>
    /// <param name="normal"> Normal of the surface. </param>
    /// <param name="wallTransform"> Transform of the wall. </param>
    /// <returns></returns>
    public static Vector3 GetWallSide(Vector3 normal, Transform wallTransform)
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

        return bestMatch;
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
        position.y = MathF.Round(position.y);

        Vector3 bestPosition = position;

        // Get some infos about the player
        float playerWidth = characterController.radius + 0.2f;
        float playerLenght = characterController.radius;
        // Get some infos about the wall
        Vector3 wallSize = Vector3.Scale(wallCollider.size, wallCollider.transform.lossyScale);
        Vector3 wallPosition = wallCollider.bounds.center;

        // Get some infos about the surface
        Vector3 orthogonalVector = (Quaternion.Euler(0, 90, 0) * normal).normalized;
        Vector3 localNormal = SimplifyVector(wallCollider.transform.InverseTransformDirection(normal));

        Vector3 surfaceCenter = Vector3.zero;
        float halfLength = 0f;

        if (Mathf.Abs(localNormal.x) != 0f)
        {
            halfLength = wallSize.z / 2;
            surfaceCenter = wallPosition + normal * (playerLenght + wallSize.x / 2);
        }
        else if (Mathf.Abs(localNormal.z) != 0f)
        {
            halfLength = wallSize.x / 2;
            surfaceCenter = wallPosition + normal * (playerLenght + wallSize.z / 2);
        }

        surfaceCenter.y = bestPosition.y;

        // Clamp along the orthogonal axis
        Vector3 toBest = bestPosition - surfaceCenter;
        float projected = Vector3.Dot(toBest, orthogonalVector);
        float clamped = Mathf.Clamp(projected, -halfLength + playerWidth, halfLength - playerWidth);

        bestPosition = surfaceCenter + orthogonalVector * clamped;

        return bestPosition;
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
            Vector3 wallSize = Vector3.Scale(walls[i].size, walls[i].transform.lossyScale);
            Vector3 normal = GetWallSide(playerTransform.position - wallPoints[walls[i]], walls[i].transform);
            Vector3 localNormal = SimplifyVector(walls[i].transform.InverseTransformDirection(normal));

            if (localNormal.x != 0f)
            {
                if (wallSize.z < characterController.radius * 2)
                {
                    sortedWalls.Remove(walls[i]);
                    wallPoints.Remove(walls[i]);
                }
            }
            else if (localNormal.z != 0f)
            {
                if (wallSize.x < characterController.radius * 2)
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

                Vector3 normal1 = GetWallSide(playerTransform.position - wall1.Value, wall1.Key.transform);
                Vector3 normal2 = GetWallSide(playerTransform.position - wall2.Value, wall2.Key.transform);

                // Check if the points are close and in the same direction
                if (Vector3.Distance(wall1.Value, wall2.Value) < 0.01f && Vector3.Dot(normal1, normal2) > 0.95f)
                {
                    Vector3 wall1Size = Vector3.Scale(wall1.Key.size, wall1.Key.transform.lossyScale);
                    Vector3 wall2Size = Vector3.Scale(wall2.Key.size, wall2.Key.transform.lossyScale);

                    float width1 = 0f;
                    float width2 = 0f;

                    Vector3 localNormal1 = SimplifyVector(wall1.Key.transform.InverseTransformDirection(normal1));
                    Vector3 localNormal2 = SimplifyVector(wall2.Key.transform.InverseTransformDirection(normal2));

                    if (localNormal1.x != 0f)
                    {
                        width1 = wall1Size.z;
                    }
                    else if (localNormal1.z != 0f)
                    {
                        width1 = wall1Size.x;
                    }

                    if (localNormal2.x != 0f)
                    {
                        width2 = wall2Size.z;
                    }
                    else if (localNormal2.z != 0f)
                    {
                        width2 = wall2Size.x;
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
        Vector3 playerPositionOnGround = playerTransform.position + new Vector3(0, 0.1f, 0);
        Vector3 stickedPositionOnGround = stickedPosition + new Vector3(0, 0.1f, 0);

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

    /// <summary>
    /// Return the enemy to hit if there is one.
    /// </summary>
    /// <param name="enemiesToSort"> Enemies around player. </param>
    /// <param name="angleInFrontOfPlayer"> Angle in front of the player where enemy must be to be hit by the player. </param>
    /// <param name="angleBackToTheEnemy"> Angle in the back of the enemy where player must be to hit the enemy. </param>
    /// <param name="playerTransform"> Transform of the player. </param>
    /// <returns></returns>
    public static Collider SortEnemiesForHit(List<Collider> enemiesToSort, float angleInFrontOfPlayer, float angleBackToTheEnemy, Transform playerTransform)
    {
        List<Collider> sortedEnemies = new();

        for (int i = 0; i < enemiesToSort.Count; i++)
        {
            float angleToEnemy = Vector3.Angle(playerTransform.forward, enemiesToSort[i].transform.position - playerTransform.position);
            float angleBehindEnemy = Vector3.Angle(-enemiesToSort[i].transform.forward, playerTransform.position - enemiesToSort[i].transform.position);

            if (angleToEnemy <= angleInFrontOfPlayer / 2 && angleBehindEnemy <= angleBackToTheEnemy / 2)
            {
                sortedEnemies.Add(enemiesToSort[i]);
            }
        }

        if (sortedEnemies.Count > 1)
        {
            Collider bestEnemy = sortedEnemies[0];
            float bestDistance = float.MaxValue;

            for (int i = 0;i < sortedEnemies.Count; i++)
            {
                float distance = (sortedEnemies[i].transform.position - playerTransform.position).magnitude;
                if (distance < bestDistance)
                {
                    bestEnemy = sortedEnemies[i];
                    bestDistance = distance;
                }
            }

            return bestEnemy;
        }
        else if (sortedEnemies.Count == 1)
        {
             return sortedEnemies[0];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Return the place to hide if there is one.
    /// </summary>
    /// <param name="placesToSort"> Places around player. </param>
    /// <param name="angleInFrontOfPlayer"> Angle in front of the player where hidden place must be. </param>
    /// <param name="playerTransform"> Transform of the player. </param>
    /// <returns></returns>
    public static Collider SortPlacesToHide(List<Collider> placesToSort, float angleInFrontOfPlayer, Transform playerTransform)
    {
        List<Collider> sortedPlaces = new();

        for (int i = 0; i < placesToSort.Count; i++)
        {
            float angleToPlace = Vector3.Angle(playerTransform.forward, placesToSort[i].transform.position - playerTransform.position);

            if (angleToPlace <= angleInFrontOfPlayer / 2)
            {
                sortedPlaces.Add(placesToSort[i]);
            }
        }

        if (sortedPlaces.Count > 1)
        {
            Collider bestPlace = sortedPlaces[0];
            float bestDistance = float.MaxValue;

            for (int i = 0; i < sortedPlaces.Count; i++)
            {
                float distance = (sortedPlaces[i].transform.position - playerTransform.position).magnitude;
                if (distance < bestDistance)
                {
                    bestPlace = sortedPlaces[i];
                    bestDistance = distance;
                }
            }

            return bestPlace;
        }
        else if (sortedPlaces.Count == 1)
        {
            return sortedPlaces[0];
        }
        else
        {
            return null;
        }
    }
}
