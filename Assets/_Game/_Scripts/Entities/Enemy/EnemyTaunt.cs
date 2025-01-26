using System.Collections;
using UnityEngine;

public class EnemyTaunt : MonoBehaviour
{
    #region Variáveis
    // Unity Inspector
    [Header("Configurações:")]

    [Header("Intervalo:")]
    [SerializeField] private float minTauntTime;
    [SerializeField] private float maxTauntTime;

    [Header("Referências:")]
    [SerializeField] private Animator enemyMeshAnimator;

    // Índice utilizado para as animações de provocação
    private int _tauntIndex = 0;
    #endregion

    #region Métodos Unity
    // Começando a coroutine que chama a animação de provocação
    private void Start() => StartCoroutine(SetTauntTimer());

    // Aguarda um tempo para então chamar a animação
    private IEnumerator SetTauntTimer() 
    {
        // Espere um valor aleatório em segundos, entre o tempo mínimo e máximo
        yield return new WaitForSeconds(Random.Range(minTauntTime, maxTauntTime));
        // Chame o método que executa a animação de provocação
        AnimateTaunt(); 
    }
    #endregion

    #region Métodos Próprios
    // Executa a animação de provocação, com base no índice atual
    public void AnimateTaunt() 
    {
        // Parâmetro referente a variação da animação de provocar
        enemyMeshAnimator.SetInteger("tauntIndex", _tauntIndex);

        // Parâmetro trigger que irá acionar uma das duas variações
        enemyMeshAnimator.SetTrigger("hasTaunt");

        // Caso a última animação foi a primeira
        if (_tauntIndex == 0) _tauntIndex = 1; // a próxima será a segunda
        // Caso a última animação foi a segunda
        else _tauntIndex = 0; // a próxima será a primeira

        // Chame mais uma vez a coroutine
        StartCoroutine(SetTauntTimer());
    }
    #endregion
}
