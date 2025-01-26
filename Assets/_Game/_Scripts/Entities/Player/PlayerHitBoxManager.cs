using UnityEngine;

public class PlayerHitBoxManager : MonoBehaviour
{
    #region Vari�veis
    // Unity Inspector
    [Header("Configura��es:")]

    // Refer�ncias dos Colliders
    [Header("Hitbox Pernas:")]
    [SerializeField] private SphereCollider[] legsBoxes;

    [Header("Hitbox M�os:")]
    [SerializeField] private SphereCollider[] handsBoxes;

    [Header("Cotovelo:")]
    [SerializeField] private SphereCollider[] elbowBox;

    // Enumerador que ser� usado para determinar quais colisores queremos ativar na anima��o
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

    // �ndices que ser�o usados para ativar os colisores
    public enum BoxesIndex
    {
        LeftLegIndex = 0,
        RightLegIndex = 1,
        LeftHandIndex = 0,
        RightHandIndex = 1,
        ElbowIndex = 0
    }
    #endregion

    #region M�todos Pr�prios
    // Est� sendo chamado atrav�s dos AnimationEvents das anima��es de ataque do Player
    private void SetHitBoxes(EnableBoxes enable) 
    {
        /* Com base no enumerador informado como p�rametro no AnimationEvent
            Ative as hitboxes espec�ficas daquela anima��o
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

    // Sendo chamado atrav�s de AnimationEvents, ap�s o t�rmino da anima��o de ataque
    private void DisableBoxes() 
    {
        // Desativando componentes de colis�o
        for (int i = 0; i < legsBoxes.Length; i++) legsBoxes[i].enabled = false;
        for (int i = 0; i < handsBoxes.Length; i++) handsBoxes[i].enabled = false;
        elbowBox[(int)BoxesIndex.ElbowIndex].enabled = false;
    }
    #endregion
}
