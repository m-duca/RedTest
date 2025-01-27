using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    #region Vari�veis
    // Unity Inspector
    [Header("Configura��es:")]

    [Header("Refer�ncias:")]
    [SerializeField] private ComboScriptable[] combos;
    [SerializeField] private Animator playerMeshAnimator;

    [Header("Ajustes Inputs:")]
    [SerializeField] private float inputInterval;
    [SerializeField] private float lastAttackCoolDown;
    [SerializeField] private float comboCooldown;

    // Armazena os �ltimos golpes feitos pelo jogador, sendo utilizado para checar com os combos
    public AttackConfig.AttackType[] _curAttackOrder = new AttackConfig.AttackType[AttackConfig.ComboSize];

    // �ndice para manipular o array acima
    private int _curAttackIndex = 0;

    // �ndices para manipular a sequ�ncia de anima��es de ataques normais
    private int _punchIndex = 0;
    private int _kickIndex = 0;

    // Par�metro Animator
    [HideInInspector] public bool Attacking { get; private set; }

    // Usado para pausa entre sequ�ncia de ataques / combos
    private bool _canAttack = true;
    #endregion

    #region M�todos Unity
    private void Start() => Attacking = false;
    #endregion

    #region M�todos Pr�prios
    #region Inputs
    // Detec��o Input de Soco
    public void HandlePunchInput(InputAction.CallbackContext context)
    {
        // Caso ainda n�o puder atacar, retorne e n�o execute nada
        if (!_canAttack) return;

        if (context.performed) 
            RegisterNewAttack(AttackConfig.AttackType.Punch);  // Adicione mais um soco para o array de golpes feitos
    }

    // Detec��o Input de Chute
    public void HandleKickInput(InputAction.CallbackContext context) 
    {
        // Caso ainda n�o puder atacar, retorne e n�o execute nada
        if (!_canAttack) return;

        if (context.performed) 
            RegisterNewAttack(AttackConfig.AttackType.Kick); // Adicione mais um chute para o array de golpes feitos
    }
    #endregion

    #region Combos
    // Registra os ataques executados, com base em seu tipo enumerado
    private void RegisterNewAttack(AttackConfig.AttackType attack)
    {
        StopAllCoroutines();

        Attacking = true;
        playerMeshAnimator.SetBool("attacking", Attacking);

        // Adicione o novo ataque
        _curAttackOrder[_curAttackIndex] = attack;

        // Caso ainda n�o for o 3� ataque feito
        if (_curAttackIndex < _curAttackOrder.Length - 1) 
        {
            // Tocando o SFX, com base no seu tipo
            PlayAttackSFX(attack);

            // Anime o player, com base no seu tipo
            AnimateAttack(attack);

            // Incremente o �ndice
            _curAttackIndex++;

            StartCoroutine(SetInputInterval());
        }
        // Caso for o 3� ataque em sequ�ncia
        else
        {
            // Desabilitando o pr�ximo ataque (ir� ser aplicado um Cooldown)
            _canAttack = false;

            // Verifique se foi feito um combo (caso for feito, o m�todo ir� retornar um nome de AnimationClip)
            var comboCreated = VerifyCombo();

            // Se n�o retornou nenhum nome de anima��o, n�o foi um combo
            if (comboCreated == null) 
            {
                PlayAttackSFX(attack); // Tocando o SFX
                AnimateAttack(attack); // Ent�o, anime um ataque normal
                StartCoroutine(SetLastAttackCooldown()); // Aplicando cooldown para o pr�ximo ataque
            }
            // Se retornou um nome, anime-o
            else 
            {
                PlayComboSFX(comboCreated[1]); // Tocando SFX do Combo
                AnimateCombo(comboCreated[0]); // Animando Clip de Anima��o do Combo
                StartCoroutine(SetComboCooldown()); // Aplicando cooldown para o pr�ximo ataque
            }

            // Resetando �ndices de anima��o dos ataques normais
            _punchIndex = 0;
            _kickIndex = 0;
            _curAttackIndex = 0;
        }
    }

    // Checa combo por combo, usando a sequ�ncia de golpes que foi feita
    private string[] VerifyCombo()
    {
        foreach (var combo in combos)
        {
            var order = combo.AttackOrder; // Acesse a combina��o de golpes do combo atual

            for (int attackIndex = 0; attackIndex < order.Length; attackIndex++) //  Verifique golpe por golpe
            {
                if (_curAttackOrder[attackIndex] != order[attackIndex]) // Caso o golpe verificado atualmente for diferente de um feito pelo jogador
                {
                    break; // Quebre o loop de verifica��es
                }
                else if (attackIndex == order.Length - 1) // Caso for o �ltimo golpe e o la�o n�o foi quebrado
                {
                    return new string[] {combo.comboAnimClipName, combo.comboSfxClipName}; // Retorne a anima��o e sfxs correspondentes do combo
                }
            }
        }
        return null; // Caso n�o tiver sido detectado nenhum combo, retorne o valor nulo
    }
    #endregion

    #region Anima��es
    private void AnimateAttack(AttackConfig.AttackType attack) 
    {
        // Soco
        if (attack == AttackConfig.AttackType.Punch)
        {
            playerMeshAnimator.Play("Anim_Player_Punch" + _punchIndex.ToString());
            _punchIndex++;
            _kickIndex = 0; // Resetando �ndice do chute
        }
        // Chute
        else
        {
            playerMeshAnimator.Play("Anim_Player_Kick" + _kickIndex.ToString());
            _kickIndex++;
            _punchIndex = 0; // Resetando �ndice do soco
        }
    }

    // Animando combo
    private void AnimateCombo(string animClipName) => playerMeshAnimator.Play(animClipName);
    #endregion

    #region Timers
    // Intervalo de espera para receber o pr�ximo ataque, caso n�o houver inputs de ataques durante esse tempo, resete a sequ�ncia
    private IEnumerator SetInputInterval() 
    {
        yield return new WaitForSeconds(inputInterval);
        _punchIndex = 0;
        _kickIndex = 0;
        _curAttackIndex = 0;
        Attacking = false;
        playerMeshAnimator.SetBool("attacking", Attacking);
    }

    // Cooldown ap�s realizar o �ltimo ataque de uma sequ�ncia de golpes normais
    private IEnumerator SetLastAttackCooldown()
    {
        yield return new WaitForSeconds(lastAttackCoolDown);
        _canAttack = true;
        Attacking = false;
        playerMeshAnimator.SetBool("attacking", Attacking);
    }

    // Cooldown ap�s realizar um combo
    private IEnumerator SetComboCooldown() 
    {
        yield return new WaitForSeconds(comboCooldown);
        _canAttack = true;
        Attacking = false;
        playerMeshAnimator.SetBool("attacking", Attacking);
    }
    #endregion

    #region SFXs
    // Toca efeitos sonoros dos ataques b�sicos
    private void PlayAttackSFX(AttackConfig.AttackType attack) 
    {
        // Caso for um soco
        if (attack == AttackConfig.AttackType.Punch)
            AudioManager.Instance.PlaySFX("sfx_player_punch" + _punchIndex); // Toque o SFX atrav�s do AudioManager, com base no �ndice atual do soco
        // Caso for um chute
        else
            AudioManager.Instance.PlaySFX("sfx_player_punch" + _kickIndex); // Toque o SFX atrav�s do AudioManager, com base no �ndice atual do chute
    }

    // Toca efeitos sonoros dos combos
    private void PlayComboSFX(string sfxName) => AudioManager.Instance.PlaySFX(sfxName);
    #endregion
    #endregion
}
