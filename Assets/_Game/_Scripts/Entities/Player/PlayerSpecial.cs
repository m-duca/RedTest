using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpecial : MonoBehaviour
{
    #region Vari�veis
    // Unity Inspector
    [Header("Configura��es:")]

    [Header("Refer�ncias:")]
    [SerializeField] private Animator playerMeshAnimator;
    [SerializeField] private CanvasHUD canvasHud;
    [SerializeField] private ParticleSystem chargeEffect;
    [SerializeField] private ParticleSystem specialEffect;

    [Header("Energia:")]
    [SerializeField] private float energyTimeIncrement;
    [SerializeField] private float energyIncrement;
    [SerializeField] private float maxEnergy;

    [Header("Especial:")]
    [SerializeField] private float specialDashForce;
    [SerializeField] private float specialInterval;

    [Header("Vibra��o:")]
    [SerializeField] private float vibrationX;
    [SerializeField] private float vibrationY;

    // Componentes
    private PlayerCombat _playerCombat;
    private GamePadVibration _gamePadVibration;
    private Rigidbody _rb;

    // Par�metros Animator
    [HideInInspector] public bool IsCharging { get; private set; }

    [HideInInspector] public bool DoingSpecial { get; private set; }

    // Energia Atual
    private float _curEnergy = 0f;
    #endregion

    #region M�todos Unity
    private void Awake()
    {
        IsCharging = false;
        DoingSpecial = false;
    }

    // Pegando refer�ncias e zerando o valor inicial da barra de energia
    private void Start()
    {
        _playerCombat = GetComponent<PlayerCombat>();
        _gamePadVibration = GetComponent<GamePadVibration>();
        _rb = GetComponent<Rigidbody>();
        canvasHud.SetEnergyBar(_curEnergy, maxEnergy);
    }

    private void Update()
    {
        // Caso estiver na anima��o de carregar, comece a acumular energia
        if (IsCharging) AddEnergy();
    }

    private void FixedUpdate()
    {
        // Caso estiver na anima��o de especial, aplique a for�a do dash
        if (DoingSpecial) ApplyForce();
    }
    #endregion

    #region M�todos Pr�prios
    #region Input
    // M�todo sendo chamado atrav�s do Evento de Input, ao segurar
    public void HandleSpecialInput(InputAction.CallbackContext context)
    {
        // Caso j� estiver fazendo o especial ou atacando, retorne e n�o fa�a nada
        if (DoingSpecial || _playerCombat.Attacking) return;

        // Caso estiver segurando
        if (context.performed) 
        {
            // Carregue
            StartCharge();

            // Ative a vibra��o do gamepad
            _gamePadVibration.SetGamePadVibration(vibrationX, vibrationY);
        }
        // Caso soltar  
        else if (context.canceled) 
        {
            // Pare de carregar
            StopCharge();

            // Desative a vibra��o do gamepad
            _gamePadVibration.SetGamePadVibration(0f, 0f);
        }
    }
    #endregion

    #region Carregando
    // Inicia o comportamento de carregar energia
    private void StartCharge() 
    {
        // Configurando Anima��o
        IsCharging = true;
        playerMeshAnimator.SetBool("charging", IsCharging);

        // Ativando efeito de part�culas
        chargeEffect.Play();

        // Toque o SFX
        AudioManager.Instance.PlaySFX("sfx_player_charge");

        // Desabilitando o comportamento de ataque
        _playerCombat.StopAllCoroutines();
        _playerCombat.CanAttack = false;
    }

    // Desativa o comportamento de carregar energia
    private void StopCharge() 
    {
        // Caso tiver acumulado energia m�xima
        if (HasReachMaxEnergy()) 
        {
            // Desative a anima��o de carregar
            IsCharging = false;
            playerMeshAnimator.SetBool("charging", IsCharging);

            // Comece o golpe especial
            ApplySpecial();   
        }
        // Caso n�o tiver acumulado a energia m�xima
        else 
        {
            // Apenas desative a anima��o de carregar
            IsCharging = false;
            _playerCombat.CanAttack = true;
            playerMeshAnimator.SetBool("charging", IsCharging);
        }

        // Desative o efeito de part�culas
        chargeEffect.Stop();

        // Atualize a barra de energia
        canvasHud.SetEnergyBar(_curEnergy, maxEnergy);
    }

    // Atualiza a energia atual, o par�metro serve para especificar se ser� cont�nuo ou de maneira imediata
    public void AddEnergy(bool overTime=true) 
    {
        // Escalando com base na varia��o de tempo
        if (overTime) _curEnergy += energyTimeIncrement * Time.deltaTime;
        // Acrescentando direto
        else _curEnergy += energyIncrement;

        // Verificando se atingiu a energia m�xima
        if (_curEnergy > maxEnergy)
            _curEnergy = maxEnergy; // Maximizando o valor

        // Atualizando a barra de energia na HUD
        canvasHud.SetEnergyBar(_curEnergy, maxEnergy);
    }

    // Retorne true / false, com base na compara��o da energia atual com a m�xima
    private bool HasReachMaxEnergy() 
    {
        if (_curEnergy >= maxEnergy) return true;
        return false;
    }
    #endregion

    #region Especial

    // Inicializa o comportamento do golpe especial
    private void ApplySpecial()
    {
        // Esgote a energia atual
        _curEnergy = 0;

        // Configure a anima��o 
        DoingSpecial = true;
        playerMeshAnimator.SetBool("doingSpecial", DoingSpecial);

        // Inicie o efeito de part�culas
        specialEffect.Play();

        // Toque o SFX
        AudioManager.Instance.PlaySFX("sfx_player_special");

        // Ative a coroutine que ir� desativar o comportamento em um certo tempo
        StartCoroutine(StopSpecial());
    }

    // Desativa o comportamento do golpe especial depois de um intervalo
    private IEnumerator StopSpecial() 
    {
        yield return new WaitForSeconds(specialInterval);

        // Zere a velocidade do Rigidbody do Player
        _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);

        // Pare a anima��o
        DoingSpecial = false;
        playerMeshAnimator.SetBool("doingSpecial", DoingSpecial);

        // Desative o efeito de part�culas
        specialEffect.Stop();

        // Habilite novamente o comportamento de golpes b�sicos
        _playerCombat.CanAttack = true;
    }

    // Aplica o dash do golpe especial, no rigidbody do Player
    private void ApplyForce() => _rb.AddForce(gameObject.transform.forward * specialDashForce * Time.fixedDeltaTime, ForceMode.Impulse);
    #endregion
    #endregion
}