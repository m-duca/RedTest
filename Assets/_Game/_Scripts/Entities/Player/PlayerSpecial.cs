using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpecial : MonoBehaviour
{
    #region Variáveis
    // Unity Inspector
    [Header("Configurações:")]

    [Header("Referências:")]
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

    [Header("Vibração:")]
    [SerializeField] private float vibrationX;
    [SerializeField] private float vibrationY;

    // Componentes
    private PlayerCombat _playerCombat;
    private GamePadVibration _gamePadVibration;
    private Rigidbody _rb;

    // Parâmetros Animator
    [HideInInspector] public bool IsCharging { get; private set; }

    [HideInInspector] public bool DoingSpecial { get; private set; }

    // Energia Atual
    private float _curEnergy = 0f;
    #endregion

    #region Métodos Unity
    private void Awake()
    {
        IsCharging = false;
        DoingSpecial = false;
    }

    // Pegando referências e zerando o valor inicial da barra de energia
    private void Start()
    {
        _playerCombat = GetComponent<PlayerCombat>();
        _gamePadVibration = GetComponent<GamePadVibration>();
        _rb = GetComponent<Rigidbody>();
        canvasHud.SetEnergyBar(_curEnergy, maxEnergy);
    }

    private void Update()
    {
        // Caso estiver na animação de carregar, comece a acumular energia
        if (IsCharging) AddEnergy();
    }

    private void FixedUpdate()
    {
        // Caso estiver na animação de especial, aplique a força do dash
        if (DoingSpecial) ApplyForce();
    }
    #endregion

    #region Métodos Próprios
    #region Input
    // Método sendo chamado através do Evento de Input, ao segurar
    public void HandleSpecialInput(InputAction.CallbackContext context)
    {
        // Caso já estiver fazendo o especial ou atacando, retorne e não faça nada
        if (DoingSpecial || _playerCombat.Attacking) return;

        // Caso estiver segurando
        if (context.performed) 
        {
            // Carregue
            StartCharge();

            // Ative a vibração do gamepad
            _gamePadVibration.SetGamePadVibration(vibrationX, vibrationY);
        }
        // Caso soltar  
        else if (context.canceled) 
        {
            // Pare de carregar
            StopCharge();

            // Desative a vibração do gamepad
            _gamePadVibration.SetGamePadVibration(0f, 0f);
        }
    }
    #endregion

    #region Carregando
    // Inicia o comportamento de carregar energia
    private void StartCharge() 
    {
        // Configurando Animação
        IsCharging = true;
        playerMeshAnimator.SetBool("charging", IsCharging);

        // Ativando efeito de partículas
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
        // Caso tiver acumulado energia máxima
        if (HasReachMaxEnergy()) 
        {
            // Desative a animação de carregar
            IsCharging = false;
            playerMeshAnimator.SetBool("charging", IsCharging);

            // Comece o golpe especial
            ApplySpecial();   
        }
        // Caso não tiver acumulado a energia máxima
        else 
        {
            // Apenas desative a animação de carregar
            IsCharging = false;
            _playerCombat.CanAttack = true;
            playerMeshAnimator.SetBool("charging", IsCharging);
        }

        // Desative o efeito de partículas
        chargeEffect.Stop();

        // Atualize a barra de energia
        canvasHud.SetEnergyBar(_curEnergy, maxEnergy);
    }

    // Atualiza a energia atual, o parâmetro serve para especificar se será contínuo ou de maneira imediata
    public void AddEnergy(bool overTime=true) 
    {
        // Escalando com base na variação de tempo
        if (overTime) _curEnergy += energyTimeIncrement * Time.deltaTime;
        // Acrescentando direto
        else _curEnergy += energyIncrement;

        // Verificando se atingiu a energia máxima
        if (_curEnergy > maxEnergy)
            _curEnergy = maxEnergy; // Maximizando o valor

        // Atualizando a barra de energia na HUD
        canvasHud.SetEnergyBar(_curEnergy, maxEnergy);
    }

    // Retorne true / false, com base na comparação da energia atual com a máxima
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

        // Configure a animação 
        DoingSpecial = true;
        playerMeshAnimator.SetBool("doingSpecial", DoingSpecial);

        // Inicie o efeito de partículas
        specialEffect.Play();

        // Toque o SFX
        AudioManager.Instance.PlaySFX("sfx_player_special");

        // Ative a coroutine que irá desativar o comportamento em um certo tempo
        StartCoroutine(StopSpecial());
    }

    // Desativa o comportamento do golpe especial depois de um intervalo
    private IEnumerator StopSpecial() 
    {
        yield return new WaitForSeconds(specialInterval);

        // Zere a velocidade do Rigidbody do Player
        _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);

        // Pare a animação
        DoingSpecial = false;
        playerMeshAnimator.SetBool("doingSpecial", DoingSpecial);

        // Desative o efeito de partículas
        specialEffect.Stop();

        // Habilite novamente o comportamento de golpes básicos
        _playerCombat.CanAttack = true;
    }

    // Aplica o dash do golpe especial, no rigidbody do Player
    private void ApplyForce() => _rb.AddForce(gameObject.transform.forward * specialDashForce * Time.fixedDeltaTime, ForceMode.Impulse);
    #endregion
    #endregion
}