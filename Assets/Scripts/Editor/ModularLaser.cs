using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class LaserBetweenBoxes : MonoBehaviour
{
    public Transform laserBody;
    public Transform boxStart;
    public Transform boxEnd;
    public float laserThickness = 0.1f;

#if UNITY_EDITOR
    void OnEnable()
    {
        EditorApplication.update += EditorUpdate;
    }

    void OnDisable()
    {
        EditorApplication.update -= EditorUpdate;
    }

    private void EditorUpdate()
    {
        if (!Application.isPlaying)
        {
            UpdateLaser();
        }
    }
#endif

    void Update()
    {
        if (Application.isPlaying)
            UpdateLaser();
    }

    void UpdateLaser()
    {
        if (laserBody == null || boxStart == null || boxEnd == null)
            return;

        Vector3 startPos = boxStart.position;
        Vector3 endPos = boxEnd.position;
        Vector3 direction = endPos - startPos;
        float length = direction.magnitude;

        if (length < 0.0001f)
            return;

        // Position laser between boxes
        laserBody.position = (startPos + endPos) * 0.5f;

        // Rotate it to face the end box
        laserBody.rotation = Quaternion.LookRotation(direction);

        // Scale along Z (assuming laser faces forward)
        laserBody.localScale = new Vector3(laserThickness, laserThickness, length);
    }
}
