using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectUI : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Show()
    {
        animator.Play("Fade In");
    }

    public void Hide()
    {
        animator.Play("Fade Out");
    }

    public void SelectLevel(int index)
    {
        // Load scene
        GameManager.instance.EnterLevel(index);

        // Play animation
        animator.Play("Fade To Level");
    }
}
