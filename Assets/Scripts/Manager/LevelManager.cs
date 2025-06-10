using UnityEngine;

public class LevelManager : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.GetComponent<PlayerStateManager>().HasVase)
            {
                
            }
        }
    }
}
