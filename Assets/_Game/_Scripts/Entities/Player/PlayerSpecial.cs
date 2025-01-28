using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpecial : MonoBehaviour
{
    [Header("Configurações:")]

    [Header("Referências:")]
    [SerializeField] private Animator playerMeshAnimator;
    [SerializeField] private CanvasHUD canvasHud;

    [Header("Energia:")]
    [SerializeField] private float energyIncrement;
    [SerializeField] private float maxEnergy;

    [Header("Especial:")]
    [SerializeField] private float specialDashForce;

    // Componentes
    private PlayerCombat _playerCombat;
    private Rigidbody _rb;

    [HideInInspector] public bool IsCharging = false;

    private float _curEnergy = 0f;

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

    public void HandleSpecialInput(InputAction.CallbackContext context)
    {
        if (_playerCombat.Attacking) return;

        if (context.performed) 
            StartCharge();
        else if (context.canceled) 
            StopCharge();
    }

    private void StartCharge() 
    {
        IsCharging = true;
        _playerCombat.StopAllCoroutines();
        _playerCombat.CanAttack = false;
        playerMeshAnimator.Play("Anim_Player_Charging");
    }

    private void StopCharge() 
    {
        if (HasReachMaxEnergy()) 
        {
            ApplySpecial();   
        }
        else 
        {
            IsCharging = false;
            _playerCombat.CanAttack = true;
            playerMeshAnimator.Play("Anim_Player_Idle");
            canvasHud.SetEnergyBar(_curEnergy, maxEnergy);
        }
    }

    private void AddEnergy() 
    {
        _curEnergy += energyIncrement * Time.deltaTime;
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
        _rb.AddForce(gameObject.transform.forward * specialDashForce * Time.deltaTime, ForceMode.Impulse);
        _curEnergy = 0;
    }
}