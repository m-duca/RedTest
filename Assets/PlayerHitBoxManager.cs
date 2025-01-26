using UnityEngine;

public class PlayerHitBoxManager : MonoBehaviour
{
    #region Variáveis
    [Header("Configurações:")]

    [Header("Hitbox Pernas:")]
    [SerializeField] private SphereCollider[] legsBoxes;

    [Header("Hitbox Mãos:")]
    [SerializeField] private SphereCollider[] handsBoxes;

    [Header("Cotovelo:")]
    [SerializeField] private SphereCollider[] elbowBox;

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
    public void SetHitBoxes(EnableBoxes enable) 
    {
        DisableBoxes();

        switch (enable)
        {
            case EnableBoxes.LeftLeg:
                handsBoxes[(int)BoxesIndex.LeftLegIndex].enabled = true;
                break;

            case EnableBoxes.RightLeg:
                handsBoxes[(int)BoxesIndex.RightLegIndex].enabled = true;
                break;

            case EnableBoxes.BothLegs:
                handsBoxes[(int)BoxesIndex.LeftLegIndex].enabled = true;
                handsBoxes[(int)BoxesIndex.RightLegIndex].enabled = true;
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

    public void DisableBoxes() 
    {
        for (int i = 0; i < legsBoxes.Length; i++) legsBoxes[i].enabled = false;
        for (int i = 0; i < handsBoxes.Length; i++) handsBoxes[i].enabled = false;
        elbowBox[(int)BoxesIndex.ElbowIndex].enabled = false;
    }
    #endregion
}
