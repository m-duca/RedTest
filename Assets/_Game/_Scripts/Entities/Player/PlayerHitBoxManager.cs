using UnityEngine;

public class PlayerHitBoxManager : MonoBehaviour
{
    #region Variáveis
    // Unity Inspector
    [Header("Configurações:")]

    // Referências dos Colliders
    [Header("Hitbox Pernas:")]
    [SerializeField] private SphereCollider[] legsBoxes;

    [Header("Hitbox Mãos:")]
    [SerializeField] private SphereCollider[] handsBoxes;

    [Header("Cotovelo:")]
    [SerializeField] private SphereCollider[] elbowBox;

    // Enumerador que será usado para determinar quais colisores queremos ativar na animação
    public enum EnableBoxes
    {
        BothHands,
        RightHand,
        LeftHand,
        BothLegs,
        LeftLeg,
        RightLeg,
        Elbow,
        All
    }

    // Índices que serão usados para ativar os colisores
    public enum BoxesIndex
    {
        LeftLegIndex = 0,
        RightLegIndex = 1,
        LeftHandIndex = 0,
        RightHandIndex = 1,
        ElbowIndex = 0
    }
    #endregion

    #region Métodos Próprios
    // Está sendo chamado através dos AnimationEvents das animações de ataque do Player
    private void SetHitBoxes(EnableBoxes enable) 
    {
        /* Com base no enumerador informado como pârametro no AnimationEvent
            Ative as hitboxes específicas daquela animação
        */
        switch (enable)
        {
            case EnableBoxes.LeftLeg:
                legsBoxes[(int)BoxesIndex.LeftLegIndex].enabled = true;
                break;

            case EnableBoxes.RightLeg:
                legsBoxes[(int)BoxesIndex.RightLegIndex].enabled = true;
                break;

            case EnableBoxes.BothLegs:
                legsBoxes[(int)BoxesIndex.LeftLegIndex].enabled = true;
                legsBoxes[(int)BoxesIndex.RightLegIndex].enabled = true;
                break;

            case EnableBoxes.LeftHand:
                handsBoxes[(int)BoxesIndex.LeftHandIndex].enabled = true;
                break;

            case EnableBoxes.RightHand:
                handsBoxes[(int)BoxesIndex.RightHandIndex].enabled = true;
                break;

            case EnableBoxes.BothHands:
                handsBoxes[(int)BoxesIndex.LeftHandIndex].enabled = true;
                handsBoxes[(int)BoxesIndex.RightHandIndex].enabled = true;
                break;

            case EnableBoxes.Elbow:
                elbowBox[(int)BoxesIndex.ElbowIndex].enabled = true;
                break;

            case EnableBoxes.All:
                for (int i = 0; i < legsBoxes.Length; i++) legsBoxes[i].enabled = true;
                for (int i = 0; i < handsBoxes.Length; i++) handsBoxes[i].enabled = true;
                elbowBox[(int)BoxesIndex.ElbowIndex].enabled = true;
                break;
        }
    }

    // Sendo chamado através de AnimationEvents, após o término da animação de ataque
    private void DisableBoxes() 
    {
        // Desativando componentes de colisão
        for (int i = 0; i < legsBoxes.Length; i++) legsBoxes[i].enabled = false;
        for (int i = 0; i < handsBoxes.Length; i++) handsBoxes[i].enabled = false;
        elbowBox[(int)BoxesIndex.ElbowIndex].enabled = false;
    }
    #endregion
}
