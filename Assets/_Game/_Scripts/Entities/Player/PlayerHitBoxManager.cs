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
    [SerializeField] private SphereCollider[] elbowsBoxes;

    [Header("Joelho:")]
    [SerializeField] private SphereCollider[] kneesBoxes;

    // Enumerador que será usado para determinar quais colisores queremos ativar na animação
    public enum EnableBoxes
    {
        BothHands,
        RightHand,
        LeftHand,
        BothLegs,
        LeftLeg,
        RightLeg,
        BothElbows,
        LeftElbow,
        RightElbow,
        BothKnees,
        LeftKnee,
        RightKnee,
        All
    }

    // Índices que serão usados para ativar os colisores
    public enum BoxesIndex
    {
        LeftLegIndex = 0,
        RightLegIndex = 1,
        LeftHandIndex = 0,
        RightHandIndex = 1,
        LeftElbowIndex = 0,
        RightElbowIndex = 1,
        LeftKneeIndex = 0,
        RightKneeIndex = 1,
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
                legsBoxes[(int)BoxesIndex.LeftLegIndex].gameObject.SetActive(true);
                break;

            case EnableBoxes.RightLeg:
                legsBoxes[(int)BoxesIndex.RightLegIndex].gameObject.SetActive(true);
                break;

            case EnableBoxes.BothLegs:
                legsBoxes[(int)BoxesIndex.LeftLegIndex].gameObject.SetActive(true);
                legsBoxes[(int)BoxesIndex.RightLegIndex].gameObject.SetActive(true);
                break;

            case EnableBoxes.LeftHand:
                handsBoxes[(int)BoxesIndex.LeftHandIndex].gameObject.SetActive(true);
                break;

            case EnableBoxes.RightHand:
                handsBoxes[(int)BoxesIndex.RightHandIndex].gameObject.SetActive(true);
                break;

            case EnableBoxes.BothHands:
                handsBoxes[(int)BoxesIndex.LeftHandIndex].gameObject.SetActive(true);
                handsBoxes[(int)BoxesIndex.RightHandIndex].gameObject.SetActive(true);
                break;

            case EnableBoxes.LeftElbow:
                elbowsBoxes[(int)BoxesIndex.LeftElbowIndex].gameObject.SetActive(true);
                break;

            case EnableBoxes.RightElbow:
                elbowsBoxes[(int)BoxesIndex.RightElbowIndex].gameObject.SetActive(true);
                break;

            case EnableBoxes.BothElbows:
                elbowsBoxes[(int)BoxesIndex.LeftElbowIndex].gameObject.SetActive(true);
                elbowsBoxes[(int)BoxesIndex.RightElbowIndex].gameObject.SetActive(true);
                break;

            case EnableBoxes.LeftKnee:
                kneesBoxes[(int)BoxesIndex.LeftKneeIndex].gameObject.SetActive(true);
                break;

            case EnableBoxes.RightKnee:
                kneesBoxes[(int)BoxesIndex.RightKneeIndex].gameObject.SetActive(true);
                break;

            case EnableBoxes.BothKnees:
                kneesBoxes[(int)BoxesIndex.LeftKneeIndex].gameObject.SetActive(true);
                kneesBoxes[(int)BoxesIndex.RightKneeIndex].gameObject.SetActive(true);
                break;

            case EnableBoxes.All:
                for (int i = 0; i < legsBoxes.Length; i++) legsBoxes[i].gameObject.SetActive(true);
                for (int i = 0; i < handsBoxes.Length; i++) handsBoxes[i].gameObject.SetActive(true);
                for (int i = 0; i < elbowsBoxes.Length; i++) elbowsBoxes[i].gameObject.SetActive(true);
                for (int i = 0; i < kneesBoxes.Length; i++) kneesBoxes[i].gameObject.SetActive(true);
                break;
        }
    }

    // Sendo chamado através de AnimationEvents, após o término da animação de ataque
    private void DisableBoxes() 
    {
        // Desativando componentes de colisão
        for (int i = 0; i < legsBoxes.Length; i++) legsBoxes[i].gameObject.SetActive(false);
        for (int i = 0; i < handsBoxes.Length; i++) handsBoxes[i].gameObject.SetActive(false);
        for (int i = 0; i < elbowsBoxes.Length; i++) elbowsBoxes[i].gameObject.SetActive(false);
        for (int i = 0; i < kneesBoxes.Length; i++) kneesBoxes[i].gameObject.SetActive(false);
    }
    #endregion
}
