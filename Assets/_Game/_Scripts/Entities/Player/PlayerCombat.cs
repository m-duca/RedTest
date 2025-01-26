using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    #region Variáveis
    [Header("Configurações:")]

    [Header("Referências:")]
    [SerializeField] private ComboScriptable[] combos;
    [SerializeField] private Animator playerMeshAnimator;

    [Header("Ajustes Inputs:")]
    [SerializeField] private float inputDelay;
    [SerializeField] private float attackCooldown;

    public AttackConfig.AttackType[] _curAttackOrder = new AttackConfig.AttackType[AttackConfig.ComboSize];

    private int _curAttackIndex = 0;
    private int _punchIndex = 0;
    private int _kickIndex = 0;
    #endregion

    #region Métodos Próprios
    #region Inputs
    public void HandlePunchInput(InputAction.CallbackContext context)
    {
        if (context.performed) 
            RegisterNewAttack(AttackConfig.AttackType.Punch);
    }

    public void HandleKickInput(InputAction.CallbackContext context) 
    {
        if (context.performed) 
            RegisterNewAttack(AttackConfig.AttackType.Kick);
    }
    #endregion

    #region Combos
    private void RegisterNewAttack(AttackConfig.AttackType attack)
    {
        if (_curAttackIndex < _curAttackOrder.Length - 1) 
        {
            Debug.LogWarning("Passou aqui");
            _curAttackOrder[_curAttackIndex] = attack;
            
            AnimateAttack(attack);
            _curAttackIndex++;
        }
        else
        {
            var comboCreated = VerifyCombo();

            if (comboCreated == null)
                AnimateAttack(attack);
            else
                AnimateCombo(comboCreated);

            _punchIndex = 0;
            _kickIndex = 0;
        }
    }

    private string VerifyCombo()
    {
        foreach (var combo in combos)
        {
            var order = combo.AttackOrder;

            for (int attackIndex = 0; attackIndex < order.Length; attackIndex++)
            {
                if (_curAttackOrder[attackIndex] != order[attackIndex])
                {
                    break;
                }
                else if (attackIndex == order.Length - 1)
                {
                    return combo.comboAnimClipName;
                }
            }
        }
        return null;
    }
    #endregion

    #region Animações
    private void AnimateAttack(AttackConfig.AttackType attack) 
    {
        if (attack == AttackConfig.AttackType.Punch)
        {
            playerMeshAnimator.Play("Anim_Player_Punch" + _punchIndex.ToString());
            _punchIndex++;
            _kickIndex = 0;
        }
        // Chute
        else
        {
            playerMeshAnimator.Play("Anim_Player_Kick" + _kickIndex.ToString());
            _kickIndex++;
            _punchIndex = 0;
        }
    }
    private void AnimateCombo(string animClipName) => playerMeshAnimator.Play(animClipName);
    #endregion
    #endregion
}
