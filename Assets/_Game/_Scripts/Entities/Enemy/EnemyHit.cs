using System.Collections;
using UnityEngine;

public class EnemyHit : MonoBehaviour
{
    #region Vari�veis
    // Unity Inspector
    [Header("Configura��es:")]

    [Header("Refer�ncias:")]
    [SerializeField] private SingleUnityLayer playerhitLayer;
    [SerializeField] private Animator enemyMeshAnimator;
    [SerializeField] private Transform playerTransform;

    [Header("Intervalo:")]
    [SerializeField] private float hitInterval;

    // Componentes
    private EnemyTaunt _enemyTaunt;

    // �ndice usado para a sequ�ncia de anima��o de levar golpes
    private int _hitIndex = 0;
    #endregion

    #region M�todos Unity
    private void Start() => _enemyTaunt = GetComponent<EnemyTaunt>();

    // Quando qualquer collider com trigger, entrar em colis�o com o inimigo
    private void OnTriggerEnter(Collider collision)
    {
        // Se for uma hitbox do player
        if (collision.gameObject.layer == playerhitLayer.Index) 
        {
            // Aproxime o Jogador do Inimigo
            playerTransform.position = Vector3.MoveTowards(playerTransform.position, gameObject.transform.position, 8f * Time.deltaTime);

            // Pare todas as coroutines desse script (voltam para o comportamento de provoca��o)
            StopAllCoroutines();

            // Pare todas as coroutines do script de provoca��o (chamam a anima��o de provoca��o)
            _enemyTaunt.StopAllCoroutines();

            // Executar anima��o de Hit
            AnimateHit();
        }
    }
    #endregion

    #region M�todos Pr�prios
    // Cuida da anima��o de golpe levado
    private void AnimateHit() 
    {
        // Par�metro para varia��o de anima��o
        enemyMeshAnimator.SetInteger("hitIndex", _hitIndex);
        // Par�metro trigger que aciona o estado de anima��o
        enemyMeshAnimator.SetTrigger("receivedHit");

        // Caso ainda n�o for a �ltima varia��o
        if (_hitIndex < 2) _hitIndex++; // Toque a seguinte na pr�xima vez
        // Se for a �ltima varia��o
        else _hitIndex = 0; // Toque a primeira na pr�xima vez

        StartCoroutine(SetHitInterval());
    }

    // Intervalo para voltar a provocar
    private IEnumerator SetHitInterval() 
    {
        // Espere o tempo espec�fico
        yield return new WaitForSeconds(hitInterval);
        // Chame em seguida a anima��o de Provoca��o
        _enemyTaunt.AnimateTaunt();
    }
    #endregion
}
