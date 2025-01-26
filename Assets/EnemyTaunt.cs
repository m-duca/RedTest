using System.Collections;
using UnityEngine;

public class EnemyTaunt : MonoBehaviour
{
    #region Variáveis
    [Header("Configurações:")]

    [Header("Intervalo:")]
    [SerializeField] private float minTauntTime;
    [SerializeField] private float maxTauntTime;

    [Header("Referências:")]
    [SerializeField] private Animator enemyMeshAnimator;

    private int _tauntIndex = 0;
    #endregion

    #region Métodos Unity
    private void Start() => StartCoroutine(SetTauntTimer());

    private IEnumerator SetTauntTimer() 
    {
        yield return new WaitForSeconds(Random.Range(minTauntTime, maxTauntTime));
        AnimateTaunt();
    }
    #endregion

    #region Métodos Próprios
    private void AnimateTaunt() 
    {
        enemyMeshAnimator.SetInteger("tauntIndex", _tauntIndex);
        enemyMeshAnimator.SetTrigger("hasTaunt");

        if (_tauntIndex == 0) _tauntIndex = 1;
        else _tauntIndex = 0;

        StartCoroutine(SetTauntTimer());
    }
    #endregion
}
