using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    #region Variáveis
    // Unity Inspector
    [Header("Configurações:")]

    [Header("Referências:")]
    [SerializeField] private ComboScriptable[] combos;
    [SerializeField] private Animator playerMeshAnimator;

    [Header("Ajustes Inputs:")]
    [SerializeField] private float inputInterval;
    [SerializeField] private float lastAttackCoolDown;
    [SerializeField] private float comboCooldown;

    // Armazena os últimos golpes feitos pelo jogador, sendo utilizado para checar com os combos
    public AttackConfig.AttackType[] _curAttackOrder = new AttackConfig.AttackType[AttackConfig.ComboSize];

    // Índice para manipular o array acima
    private int _curAttackIndex = 0;

    // Índices para manipular a sequência de animações de ataques normais
    private int _punchIndex = 0;
    private int _kickIndex = 0;

    // Parâmetro Animator
    [HideInInspector] public bool Attacking { get; private set; }

    // Usado para pausa entre sequência de ataques / combos
    private bool _canAttack = true;
    #endregion

    #region Métodos Unity
    private void Start() => Attacking = false;
    #endregion

    #region Métodos Próprios
    #region Inputs
    // Detecção Input de Soco
    public void HandlePunchInput(InputAction.CallbackContext context)
    {
        // Caso ainda não puder atacar, retorne e não execute nada
        if (!_canAttack) return;

        if (context.performed) 
            RegisterNewAttack(AttackConfig.AttackType.Punch);  // Adicione mais um soco para o array de golpes feitos
    }

    // Detecção Input de Chute
    public void HandleKickInput(InputAction.CallbackContext context) 
    {
        // Caso ainda não puder atacar, retorne e não execute nada
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

        // Caso ainda não for o 3º ataque feito
        if (_curAttackIndex < _curAttackOrder.Length - 1) 
        {
            // Tocando o SFX, com base no seu tipo
            PlayAttackSFX(attack);

            // Anime o player, com base no seu tipo
            AnimateAttack(attack);

            // Incremente o Índice
            _curAttackIndex++;

            StartCoroutine(SetInputInterval());
        }
        // Caso for o 3º ataque em sequência
        else
        {
            // Desabilitando o próximo ataque (irá ser aplicado um Cooldown)
            _canAttack = false;

            // Verifique se foi feito um combo (caso for feito, o método irá retornar um nome de AnimationClip)
            var comboCreated = VerifyCombo();

            // Se não retornou nenhum nome de animação, não foi um combo
            if (comboCreated == null) 
            {
                PlayAttackSFX(attack); // Tocando o SFX
                AnimateAttack(attack); // Então, anime um ataque normal
                StartCoroutine(SetLastAttackCooldown()); // Aplicando cooldown para o próximo ataque
            }
            // Se retornou um nome, anime-o
            else 
            {
                PlayComboSFX(comboCreated[1]); // Tocando SFX do Combo
                AnimateCombo(comboCreated[0]); // Animando Clip de Animação do Combo
                StartCoroutine(SetComboCooldown()); // Aplicando cooldown para o próximo ataque
            }

            // Resetando Índices de animação dos ataques normais
            _punchIndex = 0;
            _kickIndex = 0;
            _curAttackIndex = 0;
        }
    }

    // Checa combo por combo, usando a sequência de golpes que foi feita
    private string[] VerifyCombo()
    {
        foreach (var combo in combos)
        {
            var order = combo.AttackOrder; // Acesse a combinação de golpes do combo atual

            for (int attackIndex = 0; attackIndex < order.Length; attackIndex++) //  Verifique golpe por golpe
            {
                if (_curAttackOrder[attackIndex] != order[attackIndex]) // Caso o golpe verificado atualmente for diferente de um feito pelo jogador
                {
                    break; // Quebre o loop de verificações
                }
                else if (attackIndex == order.Length - 1) // Caso for o último golpe e o laço não foi quebrado
                {
                    return new string[] {combo.comboAnimClipName, combo.comboSfxClipName}; // Retorne a animação e sfxs correspondentes do combo
                }
            }
        }
        return null; // Caso não tiver sido detectado nenhum combo, retorne o valor nulo
    }
    #endregion

    #region Animações
    private void AnimateAttack(AttackConfig.AttackType attack) 
    {
        // Soco
        if (attack == AttackConfig.AttackType.Punch)
        {
            playerMeshAnimator.Play("Anim_Player_Punch" + _punchIndex.ToString());
            _punchIndex++;
            _kickIndex = 0; // Resetando índice do chute
        }
        // Chute
        else
        {
            playerMeshAnimator.Play("Anim_Player_Kick" + _kickIndex.ToString());
            _kickIndex++;
            _punchIndex = 0; // Resetando índice do soco
        }
    }

    // Animando combo
    private void AnimateCombo(string animClipName) => playerMeshAnimator.Play(animClipName);
    #endregion

    #region Timers
    // Intervalo de espera para receber o próximo ataque, caso não houver inputs de ataques durante esse tempo, resete a sequência
    private IEnumerator SetInputInterval() 
    {
        yield return new WaitForSeconds(inputInterval);
        _punchIndex = 0;
        _kickIndex = 0;
        _curAttackIndex = 0;
        Attacking = false;
        playerMeshAnimator.SetBool("attacking", Attacking);
    }

    // Cooldown após realizar o último ataque de uma sequência de golpes normais
    private IEnumerator SetLastAttackCooldown()
    {
        yield return new WaitForSeconds(lastAttackCoolDown);
        _canAttack = true;
        Attacking = false;
        playerMeshAnimator.SetBool("attacking", Attacking);
    }

    // Cooldown após realizar um combo
    private IEnumerator SetComboCooldown() 
    {
        yield return new WaitForSeconds(comboCooldown);
        _canAttack = true;
        Attacking = false;
        playerMeshAnimator.SetBool("attacking", Attacking);
    }
    #endregion

    #region SFXs
    // Toca efeitos sonoros dos ataques básicos
    private void PlayAttackSFX(AttackConfig.AttackType attack) 
    {
        // Caso for um soco
        if (attack == AttackConfig.AttackType.Punch)
            AudioManager.Instance.PlaySFX("sfx_player_punch" + _punchIndex); // Toque o SFX através do AudioManager, com base no índice atual do soco
        // Caso for um chute
        else
            AudioManager.Instance.PlaySFX("sfx_player_punch" + _kickIndex); // Toque o SFX através do AudioManager, com base no índice atual do chute
    }

    // Toca efeitos sonoros dos combos
    private void PlayComboSFX(string sfxName) => AudioManager.Instance.PlaySFX(sfxName);
    #endregion
    #endregion
}
