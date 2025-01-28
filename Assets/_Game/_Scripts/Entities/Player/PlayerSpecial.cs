using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpecial : MonoBehaviour
{
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

    // Componentes
    private PlayerCombat _playerCombat;
    private Rigidbody _rb;

    [HideInInspector] public bool IsCharging { get; private set; }

    [HideInInspector] public bool DoingSpecial { get; private set; }

    private float _curEnergy = 0f;

    private void Awake()
    {
        IsCharging = false;
        DoingSpecial = false;
    }

    private void Start()
    {
        _playerCombat = GetComponent<PlayerCombat>();
        _rb = GetComponent<Rigidbody>();
        canvasHud.SetEnergyBar(_curEnergy, maxEnergy);
    }

    private void Update()
    {
        if (IsCharging) AddEnergy();
    }

    private void FixedUpdate()
    {
        if (DoingSpecial) ApplyForce();
    }

    public void HandleSpecialInput(InputAction.CallbackContext context)
    {
        if (DoingSpecial || _playerCombat.Attacking) return;

        if (context.performed) 
            StartCharge();
        else if (context.canceled) 
            StopCharge();
    }

    private void StartCharge() 
    {
        IsCharging = true;
        playerMeshAnimator.SetBool("charging", IsCharging);

        chargeEffect.Play();

        _playerCombat.StopAllCoroutines();
        _playerCombat.CanAttack = false;
    }

    private void StopCharge() 
    {
        if (HasReachMaxEnergy()) 
        {
            IsCharging = false;
            playerMeshAnimator.SetBool("charging", IsCharging);

            ApplySpecial();   
        }
        else 
        {
            IsCharging = false;
            _playerCombat.CanAttack = true;
            playerMeshAnimator.SetBool("charging", IsCharging);
        }

        chargeEffect.Stop();

        canvasHud.SetEnergyBar(_curEnergy, maxEnergy);
    }

    public void AddEnergy(bool overTime=true) 
    {
        if (overTime) _curEnergy += energyTimeIncrement * Time.deltaTime;
        else _curEnergy += energyIncrement;

        if (_curEnergy > maxEnergy)
            _curEnergy = maxEnergy;

        canvasHud.SetEnergyBar(_curEnergy, maxEnergy);
    }

    private bool HasReachMaxEnergy() 
    {
        if (_curEnergy >= maxEnergy) return true;
        return false;
    }

    private void ApplySpecial()
    {
        _curEnergy = 0;

        DoingSpecial = true;

        playerMeshAnimator.SetBool("doingSpecial", DoingSpecial);

        specialEffect.Play();

        AudioManager.Instance.PlaySFX("sfx_player_special");

        StartCoroutine(StopSpecial());
    }

    private IEnumerator StopSpecial() 
    {
        yield return new WaitForSeconds(specialInterval);

        _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);

        DoingSpecial = false;

        playerMeshAnimator.SetBool("doingSpecial", DoingSpecial);

        specialEffect.Stop();

        _playerCombat.CanAttack = true;
    }

    private void ApplyForce() => _rb.AddForce(gameObject.transform.forward * specialDashForce * Time.fixedDeltaTime, ForceMode.Impulse);
}