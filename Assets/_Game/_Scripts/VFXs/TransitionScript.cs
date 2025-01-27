using EasyTransition;
using UnityEngine;

public class TransitionScript : MonoBehaviour
{
    // Unity Inspector:
    [Header("Transi��o:")]
    [SerializeField] private TransitionSettings transitionSettings;
    [SerializeField] private float loadTime;
    [SerializeField] private bool onStart;

    // Aplicando Transi��o Inicial
    private void Start()
    {
        if (onStart)
            Apply();
    }

    // Aplica a transi��o, atrav�s do Transition Manager
    public void Apply() 
    {
        TransitionManager.Instance().Transition(transitionSettings, loadTime);
    }
}
