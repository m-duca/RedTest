using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyFacePlayer : MonoBehaviour
{
    #region Vari�veis
    // Unity Inspector
    [Header("Configura��es:")]
    [SerializeField] private float faceSpeed;

    // Refer�ncias
    private Transform _playerTransform;
    #endregion

    #region M�todos Unity
    // Pegando refer�ncia do componente Transform do Player
    private void Awake() => _playerTransform = FindObjectOfType<PlayerMovement>().gameObject.transform;

    // Atualizando Rota��o do Inimigo
    private void Update() => Face();
    #endregion

    #region M�todos Pr�prios
    // Rotaciona o Inimigo atrav�s da sua propriedade transform.rotation, de modo que encare o Player
    private void Face() 
    {
        // Obtendo a rota��o desejada, com base na posi��o do jogador
        var targetRotation = Quaternion.LookRotation(_playerTransform.position, Vector3.up);

        // Aplicando o Quaternion obtido, na rota��o do componente Transform do Inimigo
        transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, targetRotation, faceSpeed * Time.deltaTime);
    }
    #endregion
}
