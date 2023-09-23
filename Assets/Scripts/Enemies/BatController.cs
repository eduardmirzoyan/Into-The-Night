using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonoBehaviour
{
    private enum BatState { Asleep, AggroPlayer, AggroLight, Searching, Returning };

    [Header("Components")]
    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private Collider2D collider2d;
    [SerializeField] private Rigidbody2D rigidbody2d;
    [SerializeField] private SpriteRenderer indicatorRenderer;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform lightTransform;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Sprite eyeSprite;
    [SerializeField] private Sprite homeSprite;
    [SerializeField] private Color chasePlayerColor;
    [SerializeField] private Color chaseLightColor;
    [SerializeField] private Color chaseHomeColor;

    [Header("Asleep Settings")]
    [SerializeField] private float aggroRange = 4f;

    [Header("Search Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float searchDuration = 2f;

    [Header("Move Settings")]
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float decelerateDistance = 2f;

    [Header("Debugging")]
    [SerializeField, ReadOnly] private BatState batState;
    [SerializeField, ReadOnly] private Vector3 homePosition;
    [SerializeField, ReadOnly] private Vector2 currentVelocity;
    [SerializeField, ReadOnly] private float searchTimer;

    private void Awake()
    {
        animationHandler = GetComponent<AnimationHandler>();
        collider2d = GetComponentInChildren<Collider2D>();
        rigidbody2d = GetComponentInChildren<Rigidbody2D>();
    }

    private void Start()
    {
        // Get targets
        playerTransform = PlayerController.instance.transform;
        lightTransform = MouseLight.instance.transform;

        // Set start state
        lineRenderer.enabled = false;
        animationHandler.ChangeAnimation("Asleep");
        batState = BatState.Asleep;

        // Find a perching spot from where it was spawned in
        FindPerchingSpot();
    }

    private void Update()
    {
        switch (batState)
        {
            case BatState.Asleep:

                // If player gets too close
                if (Vector3.Distance(transform.position, playerTransform.position) <= aggroRange && PlayerInLineOfSight())
                {
                    // Set timer
                    searchTimer = searchDuration;

                    // Enable collision
                    collider2d.enabled = true;

                    // Play sound
                    AudioManager.instance.PlaySFX("Bat Aggro");

                    // Change animation
                    animationHandler.ChangeAnimation("Awake");

                    // Change state
                    batState = BatState.Searching;
                }

                break;
            case BatState.AggroPlayer:

                // Chase player
                ChaseTarget(playerTransform.position);

                // Look for light
                if (LightInLineOfSight())
                {
                    // Red indicator
                    indicatorRenderer.color = Color.yellow;

                    // Show path
                    lineRenderer.enabled = true;
                    lineRenderer.endColor = chaseLightColor;

                    // Change state
                    batState = BatState.AggroLight;
                }

                // If something obstructs view or off-screen, stop aggro
                if (Physics2D.Linecast(transform.position, playerTransform.position, groundLayer) || IsOffScreen())
                {
                    // Stop moving
                    rigidbody2d.velocity = Vector2.zero;

                    // Change indicator
                    indicatorRenderer.color = Color.white;

                    // Hide path
                    lineRenderer.enabled = false;

                    // Set timer
                    searchTimer = searchDuration;

                    // Change state
                    batState = BatState.Searching;
                }

                break;
            case BatState.AggroLight:

                // Chase light
                ChaseTarget(lightTransform.position);

                // If something obstructs view or off-screen, stop aggro
                if (Physics2D.Linecast(transform.position, lightTransform.position, groundLayer) || IsOffScreen())
                {
                    // Stop moving
                    rigidbody2d.velocity = Vector2.zero;

                    // Change indicator
                    indicatorRenderer.color = Color.white;

                    // Hide path
                    lineRenderer.enabled = false;

                    // Set timer
                    searchTimer = searchDuration;

                    // Change state
                    batState = BatState.Searching;
                }

                break;
            case BatState.Searching:

                // Be aware of targets

                if (PlayerInLineOfSight())
                {
                    // Red indicator
                    indicatorRenderer.color = Color.red;

                    // Show path
                    lineRenderer.enabled = true;
                    lineRenderer.endColor = chasePlayerColor;

                    // Change state
                    batState = BatState.AggroPlayer;
                }

                if (LightInLineOfSight())
                {
                    // Red indicator
                    indicatorRenderer.color = Color.yellow;

                    // Show path
                    lineRenderer.enabled = true;
                    lineRenderer.endColor = chaseLightColor;

                    // Change state
                    batState = BatState.AggroLight;
                }

                // Else count down timer
                if (searchTimer > 0)
                {
                    searchTimer -= Time.deltaTime;
                }
                else
                {
                    // Remove collision
                    collider2d.enabled = false;

                    // Change indicator
                    indicatorRenderer.sprite = homeSprite;
                    indicatorRenderer.color = Color.white;

                    // Hide path
                    lineRenderer.enabled = true;
                    lineRenderer.endColor = chaseHomeColor;

                    // Change state
                    batState = BatState.Returning;
                }

                break;
            case BatState.Returning:

                // Travel home
                ChaseTarget(homePosition);

                // If reached home
                if (Vector2.Distance(transform.position, homePosition) <= decelerateDistance)
                {
                    // Snap to position
                    transform.position = homePosition;

                    // Stop moving
                    rigidbody2d.velocity = Vector2.zero;

                    // Disable collision
                    collider2d.enabled = false;

                    // Change indicator
                    indicatorRenderer.sprite = eyeSprite;
                    indicatorRenderer.color = Color.clear;

                    // Hide path
                    lineRenderer.enabled = false;

                    // Play animation
                    animationHandler.ChangeAnimation("Asleep");

                    // Change state
                    batState = BatState.Asleep;
                }

                break;
            default:
                throw new System.Exception("STATE NOT IMPLEMENTED.");
        }
    }

    private void FindPerchingSpot()
    {
        var hit = Physics2D.Raycast(transform.position, Vector2.up, float.PositiveInfinity, groundLayer);

        // If a ceiling was hit
        if (hit)
        {
            // Calculate new position
            var newPosition = hit.point - new Vector2(0f, collider2d.bounds.extents.y);

            // Relocate
            transform.position = newPosition;

            // Set home here
            homePosition = newPosition;
        }
    }

    private bool PlayerInLineOfSight()
    {
        if (IsOffScreen())
            return false;

        return !Physics2D.Linecast(transform.position, playerTransform.position, groundLayer);
    }

    private bool LightInLineOfSight()
    {
        if (IsOffScreen())
            return false;

        // Now check for line of sight
        return !Physics2D.Linecast(transform.position, lightTransform.position, groundLayer);
    }

    private bool IsOffScreen()
    {
        // Check if offscreen
        var view = Camera.main.WorldToViewportPoint(transform.position);
        return view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1;
    }

    private void ChaseTarget(Vector3 target)
    {
        // If too close to target
        if (Vector3.Distance(transform.position, target) <= decelerateDistance)
        {
            // Decelerate
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, deceleration * Time.deltaTime);
        }
        else
        {
            // Accelerate
            Vector3 direction = target - transform.position;
            direction.Normalize();
            currentVelocity = Vector2.MoveTowards(currentVelocity, direction * maxSpeed, acceleration * Time.deltaTime);
        }

        // Update path
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, target);

        rigidbody2d.velocity = currentVelocity;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the target can be damaged
        if (other.transform.parent.TryGetComponent(out DamageHandler damageHandler))
        {
            // Play sound
            AudioManager.instance.PlaySFX("Hurt");

            // Damage it
            damageHandler.Kill();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, decelerateDistance);
    }
}
