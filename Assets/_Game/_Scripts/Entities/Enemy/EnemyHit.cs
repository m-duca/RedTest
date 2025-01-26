using System.Collections;
using UnityEngine;

public class EnemyHit : MonoBehaviour
{
    #region Variáveis
    // Unity Inspector
    [Header("Configurações:")]

    [Header("Referências:")]
    [SerializeField] private SingleUnityLayer playerhitLayer;
    [SerializeField] private Animator enemyMeshAnimator;
    [SerializeField] private Transform playerTransform;

    [Header("Intervalo:")]
    [SerializeField] private float hitInterval;

    // Componentes
    private EnemyTaunt _enemyTaunt;

    // Índice usado para a sequência de animação de levar golpes
    private int _hitIndex = 0;
    #endregion

    #region Métodos Unity
    private void Start() => _enemyTaunt = GetComponent<EnemyTaunt>();

    // Quando qualquer collider com trigger, entrar em colisão com o inimigo
    private void OnTriggerEnter(Collider collision)
    {
        // Se for uma hitbox do player
        if (collision.gameObject.layer == playerhitLayer.Index) 
        {
            // Aproxime o Jogador do Inimigo
            playerTransform.position = Vector3.MoveTowards(playerTransform.position, gameObject.transform.position, 8f * Time.deltaTime);

            // Pare todas as coroutines desse script (voltam para o comportamento de provocação)
            StopAllCoroutines();

            // Pare todas as coroutines do script de provocação (chamam a animação de provocação)
            _enemyTaunt.StopAllCoroutines();

            // Executar animação de Hit
            AnimateHit();
        }
    }
    #endregion

    #region Métodos Próprios
    // Cuida da animação de golpe levado
    private void AnimateHit() 
    {
        // Parâmetro para variação de animação
        enemyMeshAnimator.SetInteger("hitIndex", _hitIndex);
        // Parâmetro trigger que aciona o estado de animação
        enemyMeshAnimator.SetTrigger("receivedHit");

        // Caso ainda não for a última variação
        if (_hitIndex < 2) _hitIndex++; // Toque a seguinte na próxima vez
        // Se for a última variação
        else _hitIndex = 0; // Toque a primeira na próxima vez

        StartCoroutine(SetHitInterval());
    }

    // Intervalo para voltar a provocar
    private IEnumerator SetHitInterval() 
    {
        // Espere o tempo específico
        yield return new WaitForSeconds(hitInterval);
        // Chame em seguida a animação de Provocação
        _enemyTaunt.AnimateTaunt();
    }
    #endregion
}
