using UnityEngine;

public class AttackConfig : MonoBehaviour
{
    public static int ComboSize = 3;

    public enum AttackType 
    {
        None,
        Punch,
        Kick
    }
}
