using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rigidbody2d;

    [Header("Move Settings")]
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float decelerateDistance = 2f;
    [SerializeField] private LayerMask lightLayer;

    [Header("Debugging")]
    [SerializeField, ReadOnly] private Transform target;
    [SerializeField, ReadOnly] private Vector2 currentVelocity;

    private void Awake()
    {
        rigidbody2d = GetComponentInChildren<Rigidbody2D>();
        currentVelocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.IsTouchingLayers(lightLayer) && !other.transform.parent.TryGetComponent(out WispController _))
        {
            // When light enters, start following
            // print($"Started following: {other.name}");

            target = other.transform.parent;

            // Play sound
            AudioManager.instance.PlaySFX("Wisp Aggro");
        }
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            // If too close to target
            if (Vector3.Distance(transform.position, target.position) <= decelerateDistance)
            {
                // Decelerate
                currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, deceleration * Time.deltaTime);
            }
            else
            {
                // Accelerate
                Vector3 direction = target.position - transform.position;
                direction.Normalize();
                currentVelocity = Vector2.MoveTowards(currentVelocity, direction * maxSpeed, acceleration * Time.deltaTime);
            }
        }
        else
        {
            // Decelerate
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, deceleration * Time.deltaTime);
        }

        rigidbody2d.velocity = currentVelocity;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.parent == target)
        {
            // When light exits, stop following
            // print($"Stopped following: {other.name}");

            target = null;
        }

    }
}
