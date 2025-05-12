using UnityEngine;

public class ShaderInteractions : MonoBehaviour
{
    void Update()
    {
        Shader.SetGlobalVector("_Player1", transform.position);
    }
}
