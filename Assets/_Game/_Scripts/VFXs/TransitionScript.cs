using EasyTransition;
using UnityEngine;

public class TransitionScript : MonoBehaviour
{
    [Header("Transi��o:")]
    [SerializeField] private TransitionSettings transitionSettings;
    [SerializeField] private float loadTime;
    [SerializeField] private bool onStart;

    private void Start()
    {
        if (onStart)
            Apply();
    }

    public void Apply() 
    {
        TransitionManager.Instance().Transition(transitionSettings, loadTime);
    }
}
