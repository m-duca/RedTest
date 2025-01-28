using UnityEngine;
using UnityEngine.InputSystem;

public class GamePadVibration : MonoBehaviour
{
    // Segurança, para não deixar o controle vibrando
    private void Awake() => SetGamePadVibration(0f, 0f);        

    // Manipula a vibração do controle
    public void SetGamePadVibration(float x, float y)
    {
        // Verificando se o controle está conectado
        if (Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(x, y);
    }

    // Segurança, para não deixar o controle vibrando
    private void OnApplicationQuit() => SetGamePadVibration(0f, 0f);
}
