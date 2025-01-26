using UnityEngine;

public class HideMouseCursor : MonoBehaviour
{
    private void Start() => Hide();

    private void Hide() 
    {
        // Escondendo cursor
        Cursor.visible = false;
        
        // Travando sua posição
        Cursor.lockState = CursorLockMode.Locked;
    }
}
