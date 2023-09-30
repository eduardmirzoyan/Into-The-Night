using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    private void Start()
    {
        // Open the scene
        TransitionManager.instance.OpenScene();
    }
}
