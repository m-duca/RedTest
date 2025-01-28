using TMPro;
using UnityEngine;

public class AttackCounter : MonoBehaviour
{
    #region Vari�veis
    // Componentes
    private TextMeshProUGUI _tmpAttackCounter;
    private Animator _attackCounterAnimator;

    // Contador de golpes recebidos
    private int _curAttackCount;
    #endregion

    #region M�todos Unity
    private void Start()
    {
        _tmpAttackCounter = GetComponent<TextMeshProUGUI>();
        _attackCounterAnimator = GetComponent<Animator>();
    }
    #endregion

    #region M�todos Pr�prios

    // Incrementa o valor atual de golpes recebidos
    public void AddAttackCounter(int value)
    {
        _curAttackCount += value;
        SetTextAttackCounter(); // Atualizando na HUD
        _attackCounterAnimator.SetTrigger("attack"); // Chamando novamente anima��o do texto quando ocorre um golpe
    }

    // Reseto o valor atual de golpes recebidos (Sendo chamado no AnimationEvent quando o txt desaparece)
    private void ResetAttackCounter() 
    {
        _curAttackCount = 0;
        SetTextAttackCounter(); // Atualizando na HUD
    }

    // Atualizando texto na HUD
    private void SetTextAttackCounter() =>   _tmpAttackCounter.text = "X" + _curAttackCount.ToString();
    #endregion
}
