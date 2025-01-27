using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ComboScriptable", menuName = "Scriptable Objects/ComboScriptable")]
public class ComboScriptable : ScriptableObject
{
    public AttackConfig.AttackType[] AttackOrder = new AttackConfig.AttackType[AttackConfig.ComboSize];
    public string comboAnimClipName;
    public string comboSfxClipName;
}
