using UnityEngine;

public class SetUICombo : MonoBehaviour
{
    // Unity Inspector
    [SerializeField] private Combos targetCombo;

    // Enumerador para selecionar qual será o combo animado
    private enum Combos 
    {
        Combo1 = 0,
        Combo2 = 1,
        Combo3 = 2,
        Combo4 = 3,
    }

    private void Awake()
    {
        // Acessando animator do mesh
        var anim = transform.Find("Grp_Mesh").gameObject.GetComponent<Animator>();
        
        // Tocando animação específica
        anim.Play("Anim_UIPlayer_Combo" + (int)targetCombo);
    }
}
