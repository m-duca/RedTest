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
    [SerializeField] private CameraScreenShake screenShake;
    [SerializeField] private ParticleSystem bloodParticle;
    [SerializeField] private PlayerSpecial playerSpecial;
    [SerializeField] private GamePadVibration gamePadVibration;
    [SerializeField] private AttackCounter attackCounter;

    [Header("Intervalo:")]
    [SerializeField] private float hitInterval;

    [Header("Vibra��o:")]
    [SerializeField] private float vibrationX;
    [SerializeField] private float vibrationY;
    [SerializeField] private float vibrationInterval;

    // Componentes
    private EnemyTaunt _enemyTaunt;

    // �ndice usado para a sequ�ncia de anima��o de levar golpes
    private int _hitIndex = 0;

    // �ndice usado para o SFX de hit
    private int _sfxIndex = 0;

    private bool _playerIsNear = false;
    #endregion

    #region M�todos Unity
    private void Start() => _enemyTaunt = GetComponent<EnemyTaunt>();

    // Quando qualquer collider com trigger, entrar em colis�o com o inimigo
    private void OnTriggerEnter(Collider collision)
    {
        // Se for uma hitbox do player
        if (collision.gameObject.layer == playerhitLayer.Index) 
        {
           if (!_playerIsNear) 
           {
                // Aproxime o Jogador do Inimigo
                playerTransform.position = Vector3.MoveTowards(playerTransform.position, gameObject.transform.position, 8f * Time.deltaTime);
                
                _playerIsNear = true;
           }

            // Pare todas as coroutines desse script (voltam para o comportamento de provoca��o / acabam com a vibra��o do controle)
            StopAllCoroutines();

            // Pare todas as coroutines do script de provoca��o (chamam a anima��o de provoca��o)
            _enemyTaunt.StopAllCoroutines();

            // Executar anima��o de Hit
            AnimateHit();

            // Tremendo a tela
            screenShake.Shake();

            // Ativar efeito de Sangue
            bloodParticle.Play();

            // Tocar Efeito Sonoro
            PlayHitSFX();

            // Adicione Energia para o Especial do Player, overTime == false (seja de imediato)
            playerSpecial.AddEnergy(false);

            // Ative a vibra��o do controle
            gamePadVibration.SetGamePadVibration(vibrationX, vibrationY);

            // Chame a coroutine que desativa a vibra��o
            StartCoroutine(SetVibrationInterval());

            // Atualizando contador de golpes
            attackCounter.AddAttackCounter(1);
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

        _playerIsNear = false;
    }

    private void PlayHitSFX() 
    {
        // Tocando o efeito sonoro, atrav�s do Audio Manager
        AudioManager.Instance.PlaySFX("sfx_enemy_hit" + _sfxIndex);

        // Caso tenha sido tocado a primeira varia��o
        if (_sfxIndex == 0) _sfxIndex = 1; // a pr�xima ser� a segunda
        // Caso tenha sido tocado a segunga varia��o
        else _sfxIndex = 0; // a pr�xima ser� a primeira
    }

    // Desativa a vibra��o do controle depois de um intervalo
    private IEnumerator SetVibrationInterval() 
    {
        yield return new WaitForSeconds(vibrationInterval);
        gamePadVibration.SetGamePadVibration(0f, 0f);
    }
    #endregion
}
