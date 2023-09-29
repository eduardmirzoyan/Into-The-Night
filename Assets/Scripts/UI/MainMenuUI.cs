using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Open the scene
        TransitionManager.instance.OpenScene();
    }

    public void Show()
    {
        animator.Play("Fade In");
    }

    public void Hide()
    {
        animator.Play("Fade Out");
    }
}
