using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Vari�veis
    // Unity Inspector
    [Header("Configura��es:")]

    [Header("Atributos:")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveRotateSpeed;

    [Header("Refer�ncias:")]
    [SerializeField] private Animator playerMeshAnimator;

    // Componentes
    private Rigidbody _rb;

    // Dire��o para controlar o movimento e rota��o, sendo obtida atrav�s da manipula��o dos valores de inputs
    private Vector3 _moveDirection;
    #endregion

    #region M�todos Unity
    private void Start()
    {
        // Pegando refer�ncias dos outros do componentes do gameObject
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Caso estiver se movendo para alguma dire��o, alterar a rota��o do Player
        if (_moveDirection != Vector3.zero) 
        {
            SetNewRotation();
            // Sinalize que o jogador est� se movendo para o par�metro do Animator
            playerMeshAnimator.SetBool("moving", true);
        }
        // Caso n�o estiver se movendo, sinalize para o par�metro do Animator
        else  
        {
            playerMeshAnimator.SetBool("moving", false);
        }
    }

    private void FixedUpdate()
    {
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
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, moveRotateSpeed * Time.deltaTime);
    }
    #endregion
}
