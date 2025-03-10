using UnityEngine;

public class VisionCone : MonoBehaviour
{
    [SerializeField] MeshFilter _meshFilter;
    [SerializeField] LayerMask _layerMask;

    void FixedUpdate()
    {
        Color[] colors = new Color[_meshFilter.mesh.vertexCount];
        Vector3 origin = transform.position;

        for (int i = 0; i < _meshFilter.mesh.vertexCount; i++)
        {
            Vector3 worldDirection = transform.TransformDirection(_meshFilter.mesh.vertices[i]);

            if (Physics.Raycast(origin, worldDirection, out RaycastHit hit))
            {
                if (((1 << hit.transform.gameObject.layer) & _layerMask) != 0)
                {
                    colors[i] = new Color(0, 0, 0, 0); // Rendre invisible
                }
                else if (hit.transform == null)
                {
                    Debug.Log("Hit transform is null");
                }
                else
                {
                    colors[i] = Color.white; // Visible
                }
            }
            else
            {
                colors[i] = Color.white; // Pas de collision, donc visible
            }
        }

        _meshFilter.mesh.colors = colors; // Appliquer les couleurs mises à jour
    }
}
