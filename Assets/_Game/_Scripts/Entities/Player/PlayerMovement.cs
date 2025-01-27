using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Vari�veis
    // Unity Inspector
    [Header("Configura��es:")]

    [Header("Refer�ncias:")]
    [SerializeField] private Animator playerMeshAnimator;
    [SerializeField] private ParticleSystem movementTrail;

    [Header("Atributos:")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveRotateSpeed;

    // Componentes
    private Rigidbody _rb;
    private PlayerCombat _playerCombat;

    // Dire��o para controlar o movimento e rota��o, sendo obtida atrav�s da manipula��o dos valores de inputs
    private Vector3 _moveDirection;
    #endregion

    #region M�todos Unity
    private void Start()
    {
        // Pegando refer�ncias dos outros do componentes do gameObject
        _rb = GetComponent<Rigidbody>();
        _playerCombat = GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        // Caso estiver atacando, retorne para n�o executar as demais l�gicas abaixo
        if (_playerCombat.Attacking) return;

        // Caso estiver se movendo para alguma dire��o, alterar a rota��o do Player
        if (_moveDirection != Vector3.zero) 
        {
            SetNewRotation();
            // Sinalize que o jogador est� se movendo para o par�metro do Animator
            playerMeshAnimator.SetBool("moving", true);
            // Ative o Efeito de Part�culas
            movementTrail.Play();
        }
        // Caso n�o estiver se movendo, sinalize para o par�metro do Animator
        else  
        {
            playerMeshAnimator.SetBool("moving", false);
            movementTrail.Stop();
        }
    }

    private void FixedUpdate()
    {
        // Caso o player estiver atacando
        if (_playerCombat.Attacking)
        {
            // Zere a velocidade do Rigidbody nos eixos da movimenta��o padr�o (pode ter sido alterada no frame anterior)
            _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);

            // Retorne para n�o executar a l�gica de movimenta��o
            return;
        }

        // Caso estiver se movendo para alguma dire��o, aplique a movimenta��o
        if (_moveDirection != Vector3.zero)
            ApplyMove();
    }
    #endregion

    #region M�todos Pr�prios
    // Acessado no componente Player Input, sendo chamado caso houver algum evento de a��o do tipo "Move"
    public void HandleMoveInput(InputAction.CallbackContext context) 
    {
        var moveInput = context.ReadValue<Vector2>();
        _moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
    }

    private void ApplyMove() 
    {
        // C�lculo do movimento
        var movement = _moveDirection * moveSpeed;

        // Aplicando a for�a no componente Rigidbody, referenciado anteriormente
        _rb.AddForce(movement, ForceMode.Force);
    }

    private void SetNewRotation() 
    {
        // Obtendo a rota��o desejada, com base na dire��o em que est� se movendo
        var targetRotation = Quaternion.LookRotation(_moveDirection, Vector3.up);

        // Aplicando o Quaternion obtido, na rota��o do componente Transform do Player
        transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, targetRotation, moveRotateSpeed * Time.deltaTime);
    }
    #endregion
}
