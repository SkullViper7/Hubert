using UnityEngine;
using UnityEditor;

public class MissingPrefabCleaner : MonoBehaviour
{
    [MenuItem("Tools/Clean Missing Prefabs")]
    private static void CleanMissingPrefabs()
    {
        int removedCount = 0;
        GameObject[] allObjects = FindObjectsOfType<GameObject>(true);

        foreach (GameObject obj in allObjects)
        {
            if (PrefabUtility.GetPrefabInstanceStatus(obj) == PrefabInstanceStatus.MissingAsset)
            {
                Debug.Log($"Removed missing prefab: {obj.name}", obj);
                DestroyImmediate(obj);
                removedCount++;
            }
        }

        Debug.Log($"Cleanup complete. Removed {removedCount} missing prefab(s).");
    }
}
