using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Camera cam;

    [Header("Data")]
    [SerializeField] private Vector2 spriteSize;
    [SerializeField] private Vector3 startPosition;

    [Header("Settings")]
    [SerializeField] private float parallaxMultiplier;

    private void Awake()
    {
        cam = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteSize = spriteRenderer.bounds.size;

        // Create left children with offset
        var leftChild = new GameObject($"Left {name}");
        leftChild.transform.position = new Vector3(transform.position.x - spriteSize.x, transform.position.y, 0);
        leftChild.transform.parent = transform;
        var renderer = leftChild.AddComponent<SpriteRenderer>();
        renderer.material = spriteRenderer.material;
        renderer.sprite = spriteRenderer.sprite;
        renderer.sortingLayerName = spriteRenderer.sortingLayerName;
        renderer.sortingOrder = spriteRenderer.sortingOrder;

        // Create right children with offset
        var rightChild = new GameObject($"Right {name}");
        rightChild.transform.position = new Vector3(transform.position.x + spriteSize.x, transform.position.y, 0);
        rightChild.transform.parent = transform;
        renderer = rightChild.AddComponent<SpriteRenderer>();
        renderer.material = spriteRenderer.material;
        renderer.sprite = spriteRenderer.sprite;
        renderer.sortingLayerName = spriteRenderer.sortingLayerName;
        renderer.sortingOrder = spriteRenderer.sortingOrder;
    }

    private void Start()
    {
        // Start position of background should be in bottom center of map
        var bounds = MapBoundaryManager.instance.GetBoundingCollider().bounds;
        var bottomCenter = new Vector3(bounds.center.x, bounds.min.y, 0);
        startPosition = bottomCenter;
    }

    private void Update()
    {
        // Offset based on camera position
        Vector3 relativeDistance = cam.transform.position * parallaxMultiplier;
        transform.position = new Vector3(startPosition.x + relativeDistance.x, startPosition.y + relativeDistance.y, 0);

        // Relocate if offscreen
        float relativeCameraDistance = cam.transform.position.x * (1 - parallaxMultiplier);
        if (relativeCameraDistance > startPosition.x + spriteSize.x)
        {
            startPosition.x += spriteSize.x;
        }
        else if (relativeCameraDistance < startPosition.x - spriteSize.x)
        {
            startPosition.x -= spriteSize.x;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(startPosition, 0.5f);
    }

    private Vector2 GetScreenWorldSize()
    {
        float aspect = (float)Screen.width / Screen.height;

        float worldHeight = cam.orthographicSize * 2;

        float worldWidth = worldHeight * aspect;

        return new Vector2(worldWidth, worldHeight);
    }
}
