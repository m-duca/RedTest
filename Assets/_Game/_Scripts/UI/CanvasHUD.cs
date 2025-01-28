using System.Collections;
using EasyTransition;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class CanvasHUD : MonoBehaviour
{
    #region Vari�veis
    // Unity Inspector
    [Header("Configura��es:")]

    [Header("Refer�ncias:")]
    [SerializeField] private GameObject CombosInputsParent;
    [SerializeField] private TextMeshProUGUI tmpInputEnergy;
    [SerializeField] private TextMeshProUGUI tmpOpenCombos;
    [SerializeField] private GameObject UICombosMeshsParent;
    [SerializeField] private RectTransform rectTransEnergyBar;
    [SerializeField] private TransitionScript transition;

    [Header("Valores:")]
    [SerializeField] private string txtGamepadEnergy;
    [SerializeField] private string txtKeyboardMouseEnergy;
    [SerializeField] private string txtGamepadOpenCombos;
    [SerializeField] private string txtKeyboardMouseOpenCombos;
    [SerializeField] private string txtGamepadCloseCombos;
    [SerializeField] private string txtKeyboardMouseCloseCombos;

    // Armazenando o dispositivo atual (para conseguir adaptar as informa��es do HUD)
    private string _currentDevice;

    // Escala inicial no Eixo X da barra de energia (ser� usada para calcular o valor atual)
    private float _energyBarInitialScaleX;
    #endregion

    #region M�todos Unity
    private void Awake() => _energyBarInitialScaleX = rectTransEnergyBar.localScale.x; // Pegando valor inicial da Escala no Eixo X
    #endregion

    #region M�todos Pr�prios
    #region Eventos InputSystem
    // M�todo acionado atrav�s do Evento de Input, abrindo / fechando o HUD de combos
    public void OpenCombos(InputAction.CallbackContext context)
    {
        // Caso o TAB / START seja pressionado
        if (context.performed)
        {
            // Configure os elementos
            SetCombosInputs();
            SetDeviceText();
            SetDeviceButtons();

            // Aplique a transi��o (caso n�o esteja acontecendo alguma)
            if (GameObject.FindAnyObjectByType<Transition>() == null)
                transition.Apply();

            // Toque o efeito sonoro
            AudioManager.Instance.PlaySFX("sfx_hud_open");
        }
    }

    // M�todo chamado atrav�s dos Eventos de Input, quando o dispositivo atual � trocado
    public void ChangeHUDInfo(PlayerInput input)
    {
        // Alterando a refer�ncia do dispositivo atual
        _currentDevice = input.currentControlScheme;

        // Adaptando o texto
        SetDeviceText();

        // Adaptando as imagens, caso o HUD de combos esteja aberto
        if (CombosInputsParent.activeSelf) SetDeviceButtons();
    }
    #endregion

    #region Elementos HUD
    // Ativa os principais elementos do HUD de combos
    private void SetCombosInputs() 
    {
        // Invertendo o valor booleano atual
        var newState = !CombosInputsParent.activeSelf;

        // Caso foi aberto
        if (newState) Time.timeScale = 0f; // Pause o jogo
        // Caso foi fechado
        else Time.timeScale = 1f; // Despause o jogo

        // Ativando / Desligando
        UICombosMeshsParent.SetActive(newState); // Meshs que mostram as anima��es de cada combo
        CombosInputsParent.SetActive(newState); // Inputs de cada combo
    }

    // Altera as informa��es de textos da HUD, com base no dispositivo atual e se est� aberta ou fechada
    private void SetDeviceText()
    {
        // Gamepad
        if (_currentDevice == "Gamepad")
        {
            // Texto para recarregar energia do golpe especial
            tmpInputEnergy.text = txtGamepadEnergy;

            // Caso o HUD n�o estiver aberto
            if (!CombosInputsParent.activeSelf)
                tmpOpenCombos.text = txtGamepadOpenCombos; // texto que indica como abrir
            // Caso o HUD estiver aberto
            else
                tmpOpenCombos.text = txtGamepadCloseCombos; // texto que indica como fechar
        }
        // Teclado e Mouse
        else if (_currentDevice == "Keyboard&Mouse")
        {
            // Texto para recarregar energia do golpe especial
            tmpInputEnergy.text = txtKeyboardMouseEnergy;

            // Caso o HUD n�o estiver aberto
            if (!CombosInputsParent.activeSelf)
                tmpOpenCombos.text = txtKeyboardMouseOpenCombos; // texto que indica como abrir
            // Caso o HUD estiver aberto
            else
                tmpOpenCombos.text = txtKeyboardMouseCloseCombos; // texto que indica como fechar
        }
    }

    // Altera os bot�es da HUD, com base no dispositivo atual
    private void SetDeviceButtons() 
    {
        // Acessando refer�ncias
        var gamepadInputsParent = CombosInputsParent.transform.Find("Gamepad Combos").gameObject;
        var mouseInputsParent = CombosInputsParent.transform.Find("Mouse Combos").gameObject;

        // Gamepad
        if (_currentDevice == "Gamepad") 
        {
            // Mostre apenas os bot�es do Gamepad
            gamepadInputsParent.SetActive(true);
            mouseInputsParent.SetActive(false);
        }
        // Teclado e Mouse
        else if (_currentDevice == "Keyboard&Mouse") 
        {
            // Mostre apenas os bot�es do Mouse
            mouseInputsParent.SetActive(true);
            gamepadInputsParent.SetActive(false);
        }
    }

    // Atualiza a largura atual da barra de energia (est� sendo chamado no PlayerSpecial)
    public void SetEnergyBar(float curValue, float maxValue) 
    {
        // Calculando valor atual, para ent�o modificar a escala no eixo X com base na inicial
        float energyPercentage = curValue / maxValue;

        // Alterando escala
        rectTransEnergyBar.localScale = new Vector3(energyPercentage * _energyBarInitialScaleX, rectTransEnergyBar.localScale.y, rectTransEnergyBar.localScale.z);
    }
    #endregion
    #endregion
}
