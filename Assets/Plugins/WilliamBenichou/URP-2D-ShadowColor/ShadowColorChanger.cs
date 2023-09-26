using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowColorChanger : MonoBehaviour
{

    [SerializeField] private Color m_color;

    private void Awake()
    {
        SetShadowColor(m_color);
    }

    private void OnValidate()
    {
        SetShadowColor(m_color);
    }


    public static void SetShadowColor(Color c)
    {
        Shader.SetGlobalColor("_ShadowColor", c);
    }
}
