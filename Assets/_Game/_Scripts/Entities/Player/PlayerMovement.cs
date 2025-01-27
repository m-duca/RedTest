using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Variáveis
    // Unity Inspector
    [Header("Configurações:")]

    [Header("Referências:")]
    [SerializeField] private Animator playerMeshAnimator;
    [SerializeField] private ParticleSystem movementTrail;

    [Header("Atributos:")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveRotateSpeed;

    // Componentes
    private Rigidbody _rb;
    private PlayerCombat _playerCombat;

    // Direção para controlar o movimento e rotação, sendo obtida através da manipulação dos valores de inputs
    private Vector3 _moveDirection;
    #endregion

    #region Métodos Unity
    private void Start()
    {
        // Pegando referências dos outros do componentes do gameObject
        _rb = GetComponent<Rigidbody>();
        _playerCombat = GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        // Caso estiver atacando, retorne para não executar as demais lógicas abaixo
        if (_playerCombat.Attacking) return;

        // Caso estiver se movendo para alguma direção, alterar a rotação do Player
        if (_moveDirection != Vector3.zero) 
        {
            SetNewRotation();
            // Sinalize que o jogador está se movendo para o parâmetro do Animator
            playerMeshAnimator.SetBool("moving", true);
            // Ative o Efeito de Partículas
            movementTrail.Play();
        }
        // Caso não estiver se movendo, sinalize para o parâmetro do Animator
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
            // Zere a velocidade do Rigidbody nos eixos da movimentação padrão (pode ter sido alterada no frame anterior)
            _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);

            // Retorne para não executar a lógica de movimentação
            return;
        }

        // Caso estiver se movendo para alguma direção, aplique a movimentação
        if (_moveDirection != Vector3.zero)
            ApplyMove();
    }
    #endregion

    #region Métodos Próprios
    // Acessado no componente Player Input, sendo chamado caso houver algum evento de ação do tipo "Move"
    public void HandleMoveInput(InputAction.CallbackContext context) 
    {
        var moveInput = context.ReadValue<Vector2>();
        _moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
    }

    private void ApplyMove() 
    {
        // Cálculo do movimento
        var movement = _moveDirection * moveSpeed;

        // Aplicando a força no componente Rigidbody, referenciado anteriormente
        _rb.AddForce(movement, ForceMode.Force);
    }

    private void SetNewRotation() 
    {
        // Obtendo a rotação desejada, com base na direção em que está se movendo
        var targetRotation = Quaternion.LookRotation(_moveDirection, Vector3.up);

        // Aplicando o Quaternion obtido, na rotação do componente Transform do Player
        transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, targetRotation, moveRotateSpeed * Time.deltaTime);
    }
    #endregion
}
