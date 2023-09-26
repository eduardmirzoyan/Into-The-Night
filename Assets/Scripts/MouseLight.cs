using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MouseLight : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private Light2D light2D;
    [SerializeField] private CircleCollider2D circleCollider;

    [Header("Settings")]
    [SerializeField] private float radius = 3f;
    [SerializeField] private Vector2 pulseRange;

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

        // Get hitbox
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.radius = radius / 2;

        // Set up light
        light2D = GetComponent<Light2D>();
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
        Pulsate();
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
