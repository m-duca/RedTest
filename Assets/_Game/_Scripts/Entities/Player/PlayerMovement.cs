using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Variáveis
    // Unity Inspector
    [Header("Configurações:")]

    [Header("Atributos:")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveRotateSpeed;

    [Header("Referências:")]
    [SerializeField] private Animator playerMeshAnimator;

    // Componentes
    private Rigidbody _rb;

    // Direção para controlar o movimento e rotação, sendo obtida através da manipulação dos valores de inputs
    private Vector3 _moveDirection;
    #endregion

    #region Métodos Unity
    private void Start()
    {
        // Pegando referências dos outros do componentes do gameObject
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Caso estiver se movendo para alguma direção, alterar a rotação do Player
        if (_moveDirection != Vector3.zero) 
        {
            SetNewRotation();
            // Sinalize que o jogador está se movendo para o parâmetro do Animator
            playerMeshAnimator.SetBool("moving", true);
        }
        // Caso não estiver se movendo, sinalize para o parâmetro do Animator
        else  
        {
            playerMeshAnimator.SetBool("moving", false);
        }
    }

    private void FixedUpdate()
    {
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
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, moveRotateSpeed * Time.deltaTime);
    }
    #endregion
}
