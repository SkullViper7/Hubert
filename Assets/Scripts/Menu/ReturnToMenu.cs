using UnityEngine;

public class ReturnToMenu : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        GameManager.Instance.BackToMenu();
    }
}
