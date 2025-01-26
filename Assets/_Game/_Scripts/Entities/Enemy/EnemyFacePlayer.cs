using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyFacePlayer : MonoBehaviour
{
    #region Variáveis
    // Unity Inspector
    [Header("Configurações:")]
    [SerializeField] private float faceSpeed;

    // Referências
    private Transform _playerTransform;
    #endregion

    #region Métodos Unity
    // Pegando referência do componente Transform do Player
    private void Awake() => _playerTransform = FindObjectOfType<PlayerMovement>().gameObject.transform;

    // Atualizando Rotação do Inimigo
    private void Update() => Face();
    #endregion

    #region Métodos Próprios
    // Rotaciona o Inimigo através da sua propriedade transform.rotation, de modo que encare o Player
    private void Face() 
    {
        // Obtendo a rotação desejada, com base na posição do jogador
        var targetRotation = Quaternion.LookRotation(_playerTransform.position, Vector3.up);

        // Aplicando o Quaternion obtido, na rotação do componente Transform do Inimigo
        transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, targetRotation, faceSpeed * Time.deltaTime);
    }
    #endregion
}
