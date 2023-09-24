using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform target;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Camera cam;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Material material;

    [Header("Settings")]
    [SerializeField] private float parallaxEffect;

    private void Start()
    {
        cam = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
        startPosition = cam.transform.position;

        target = PlayerController.instance.transform;
        transform.position = target.position;
    }

    private void LateUpdate()
    {
        var displacement = cam.transform.position - startPosition;
        material.SetTextureOffset("_MainTex", displacement * parallaxEffect);
        transform.position = target.position;
    }
}
