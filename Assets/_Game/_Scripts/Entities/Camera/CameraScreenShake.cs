using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraScreenShake : MonoBehaviour
{
    #region Variáveis
    // Unity Inspector
    [Header("Configurações:")]
    [SerializeField] private float shakeIntensity;
    [SerializeField] private float shakeInterval;

    // Componentes
    private CinemachineBasicMultiChannelPerlin _multiChannel;
    #endregion

    #region Métodos Unity
    private void Awake()
    {
        // Acessando a Instância da Camera
        var camera = GetComponent<CinemachineCamera>();
        
        // Pegando os canais de noise para criar o efeito
        _multiChannel = camera.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }
    #endregion

    #region Métodos Próprios
    // Aplica o efeito de tremor
    public void Shake() 
    {
        // Aumenta a amplitude para gerar o efeito
        _multiChannel.AmplitudeGain = shakeIntensity;

        // Comece uma contagem para parar o efeito
        StartCoroutine(StopShake());
    }

    private IEnumerator StopShake() 
    {
        yield return new WaitForSeconds(shakeInterval);
        // Zere a amplitude, terminando assim o efeito
        _multiChannel.AmplitudeGain = 0;
    }
    #endregion
}
