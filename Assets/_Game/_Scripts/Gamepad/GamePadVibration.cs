using UnityEngine;
using UnityEngine.InputSystem;

public class GamePadVibration : MonoBehaviour
{
    // Seguran�a, para n�o deixar o controle vibrando
    private void Awake() => SetGamePadVibration(0f, 0f);        

    // Manipula a vibra��o do controle
    public void SetGamePadVibration(float x, float y)
    {
        // Verificando se o controle est� conectado
        if (Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(x, y);
    }

    // Seguran�a, para n�o deixar o controle vibrando
    private void OnApplicationQuit() => SetGamePadVibration(0f, 0f);
}
