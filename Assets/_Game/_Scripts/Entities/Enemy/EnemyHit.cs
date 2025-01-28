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
    [SerializeField] private CameraScreenShake screenShake;
    [SerializeField] private ParticleSystem bloodParticle;
    [SerializeField] private PlayerSpecial playerSpecial;
    [SerializeField] private GamePadVibration gamePadVibration;
    [SerializeField] private AttackCounter attackCounter;

    [Header("Intervalo:")]
    [SerializeField] private float hitInterval;

    [Header("Vibração:")]
    [SerializeField] private float vibrationX;
    [SerializeField] private float vibrationY;
    [SerializeField] private float vibrationInterval;

    // Componentes
    private EnemyTaunt _enemyTaunt;

    // Índice usado para a sequência de animação de levar golpes
    private int _hitIndex = 0;

    // Índice usado para o SFX de hit
    private int _sfxIndex = 0;

    private bool _playerIsNear = false;
    #endregion

    #region Métodos Unity
    private void Start() => _enemyTaunt = GetComponent<EnemyTaunt>();

    // Quando qualquer collider com trigger, entrar em colisão com o inimigo
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

            // Pare todas as coroutines desse script (voltam para o comportamento de provocação / acabam com a vibração do controle)
            StopAllCoroutines();

            // Pare todas as coroutines do script de provocação (chamam a animação de provocação)
            _enemyTaunt.StopAllCoroutines();

            // Executar animação de Hit
            AnimateHit();

            // Tremendo a tela
            screenShake.Shake();

            // Ativar efeito de Sangue
            bloodParticle.Play();

            // Tocar Efeito Sonoro
            PlayHitSFX();

            // Adicione Energia para o Especial do Player, overTime == false (seja de imediato)
            playerSpecial.AddEnergy(false);

            // Ative a vibração do controle
            gamePadVibration.SetGamePadVibration(vibrationX, vibrationY);

            // Chame a coroutine que desativa a vibração
            StartCoroutine(SetVibrationInterval());

            // Atualizando contador de golpes
            attackCounter.AddAttackCounter(1);
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

        _playerIsNear = false;
    }

    private void PlayHitSFX() 
    {
        // Tocando o efeito sonoro, através do Audio Manager
        AudioManager.Instance.PlaySFX("sfx_enemy_hit" + _sfxIndex);

        // Caso tenha sido tocado a primeira variação
        if (_sfxIndex == 0) _sfxIndex = 1; // a próxima será a segunda
        // Caso tenha sido tocado a segunga variação
        else _sfxIndex = 0; // a próxima será a primeira
    }

    // Desativa a vibração do controle depois de um intervalo
    private IEnumerator SetVibrationInterval() 
    {
        yield return new WaitForSeconds(vibrationInterval);
        gamePadVibration.SetGamePadVibration(0f, 0f);
    }
    #endregion
}
