using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MouseLight : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private Light2D light2D;
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Settings")]
    [SerializeField] private float radius = 3f;

    public static MouseLight instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        // Setup renderer
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.transform.localScale = Vector3.one * radius;

        // Setup hitbox
        circleCollider = GetComponentInChildren<CircleCollider2D>();
        circleCollider.radius = radius;

        // Setup light
        light2D = GetComponentInChildren<Light2D>();
        light2D.pointLightOuterRadius = radius;
    }

    private void Start()
    {
        transform.position = PlayerController.instance.transform.position;
    }

    void Update()
    {
        if (PauseManager.instance.isPaused)
            return;

        FollowMouse();
        // Pulsate();
    }

    private void FollowMouse()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        transform.position = mousePosition;
    }

    private void Pulsate()
    {
        light2D.intensity = 0.85f + Mathf.PingPong(Time.time / 4, 0.25f);
    }
}
