using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSizeRuler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void OnGUI()
    {
        if (GUILayout.Button("Resize to Screen"))
        {
            if (spriteRenderer != null)
            {
                Resize();
            }
            else
            {
                print("Sprite Render is null.");
            }
        }
    }

    private void Start()
    {
        Resize();
        print(spriteRenderer.size);
    }

    private void Resize()
    {
        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        spriteRenderer.size = new Vector2(worldScreenWidth, worldScreenHeight);
    }
}
