using EasyTransition;
using UnityEngine;

public class TransitionScript : MonoBehaviour
{
    // Unity Inspector:
    [Header("Transição:")]
    [SerializeField] private TransitionSettings transitionSettings;
    [SerializeField] private float loadTime;
    [SerializeField] private bool onStart;

    // Aplicando Transição Inicial
    private void Start()
    {
        if (onStart)
            Apply();
    }

    // Aplica a transição, através do Transition Manager
    public void Apply() 
    {
        TransitionManager.Instance().Transition(transitionSettings, loadTime);
    }
}
