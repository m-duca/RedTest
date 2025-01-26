using System.Collections;
using UnityEngine;

public class EnemyTaunt : MonoBehaviour
{
    #region Vari�veis
    // Unity Inspector
    [Header("Configura��es:")]

    [Header("Intervalo:")]
    [SerializeField] private float minTauntTime;
    [SerializeField] private float maxTauntTime;

    [Header("Refer�ncias:")]
    [SerializeField] private Animator enemyMeshAnimator;

    // �ndice utilizado para as anima��es de provoca��o
    private int _tauntIndex = 0;
    #endregion

    #region M�todos Unity
    // Come�ando a coroutine que chama a anima��o de provoca��o
    private void Start() => StartCoroutine(SetTauntTimer());

    // Aguarda um tempo para ent�o chamar a anima��o
    private IEnumerator SetTauntTimer() 
    {
        // Espere um valor aleat�rio em segundos, entre o tempo m�nimo e m�ximo
        yield return new WaitForSeconds(Random.Range(minTauntTime, maxTauntTime));
        // Chame o m�todo que executa a anima��o de provoca��o
        AnimateTaunt(); 
    }
    #endregion

    #region M�todos Pr�prios
    // Executa a anima��o de provoca��o, com base no �ndice atual
    public void AnimateTaunt() 
    {
        // Par�metro referente a varia��o da anima��o de provocar
        enemyMeshAnimator.SetInteger("tauntIndex", _tauntIndex);

        // Par�metro trigger que ir� acionar uma das duas varia��es
        enemyMeshAnimator.SetTrigger("hasTaunt");

        // Caso a �ltima anima��o foi a primeira
        if (_tauntIndex == 0) _tauntIndex = 1; // a pr�xima ser� a segunda
        // Caso a �ltima anima��o foi a segunda
        else _tauntIndex = 0; // a pr�xima ser� a primeira

        // Chame mais uma vez a coroutine
        StartCoroutine(SetTauntTimer());
    }
    #endregion
}
