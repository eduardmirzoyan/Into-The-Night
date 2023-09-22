using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonoBehaviour
{
    private enum BatState { Asleep, AggroPlayer, AggroLight, Searching, Returning };
    private enum Target { None, Player, Light, Home }

    [Header("Components")]
    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private Collider2D collider2d;
    [SerializeField] private Rigidbody2D rigidbody2d;
    [SerializeField] private SpriteRenderer indicatorRenderer;
    [SerializeField] private SpriteRenderer rangeRenderer;

    [Header("Data")]
    [SerializeField] private BatState batState;
    [SerializeField] private Target target;
    [SerializeField] private Vector3 homePosition;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform lightTransform;

    [Header("Settings")]
    [SerializeField] private float aggroRange = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float returnRange = 0.1f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float bufferDuration = 1f;
    [SerializeField] private float bufferTimer;
    [SerializeField] private float searchDuration = 5f;
    [SerializeField] private float searchTimer;

    private void Awake()
    {
        animationHandler = GetComponent<AnimationHandler>();
        collider2d = GetComponentInChildren<Collider2D>();
        rigidbody2d = GetComponentInChildren<Rigidbody2D>();

        // Set size based on range
        rangeRenderer.transform.localScale = Vector3.one * aggroRange;
    }

    private void Start()
    {
        // Get targets
        playerTransform = PlayerController.instance.transform;
        lightTransform = MouseLight.instance.transform;

        // Set start state
        batState = BatState.Asleep;
        animationHandler.ChangeAnimation("Asleep");

        // No target
        target = Target.None;

        // Find a perching spot from where it was spawned in
        FindPerchingSpot();
    }

    private void Update()
    {
        switch (batState)
        {
            case BatState.Asleep:

                // Search for any targets
                SearchForPlayer();

                // If a target was found
                if (target == Target.Player)
                {
                    // Hide ring
                    // aggroRangeDisplay.enabled = false;

                    // Red indicator
                    indicatorRenderer.color = Color.red;

                    // Set buffer
                    bufferTimer = bufferDuration;

                    // Play sound
                    AudioManager.instance.Play("Bat Aggro");

                    // Change animation
                    animationHandler.ChangeAnimation("Awake");

                    // Change state
                    batState = BatState.AggroPlayer;
                }

                break;
            case BatState.AggroPlayer:

                // Wait
                if (bufferTimer > 0)
                {
                    bufferTimer -= Time.deltaTime;
                }
                else
                {
                    // Chase the target
                    ChaseTarget(target);
                }

                // Look for light
                SearchForLight();

                // If target changed
                if (target == Target.Light)
                {
                    // Set buffer
                    bufferTimer = bufferDuration;

                    // Red indicator
                    indicatorRenderer.color = Color.yellow;

                    // Change state
                    batState = BatState.AggroLight;
                }

                // If something obstructs view, stop aggro
                if (Physics2D.Linecast(transform.position, playerTransform.position, groundLayer))
                {
                    // Stop moving
                    rigidbody2d.velocity = Vector2.zero;

                    // Change indicator
                    indicatorRenderer.color = Color.white;

                    // Set timer
                    searchTimer = searchDuration;

                    // Change state
                    batState = BatState.Searching;
                }

                break;
            case BatState.AggroLight:

                // Wait
                if (bufferTimer > 0)
                {
                    bufferTimer -= Time.deltaTime;
                }
                else
                {
                    // Chase the target
                    ChaseTarget(target);
                }

                // If something obstructs view, stop aggro
                if (Physics2D.Linecast(transform.position, lightTransform.position, groundLayer))
                {
                    // Stop moving
                    rigidbody2d.velocity = Vector2.zero;

                    // Change indicator
                    indicatorRenderer.color = Color.white;

                    // Set timer
                    searchTimer = searchDuration;

                    // Change state
                    batState = BatState.Searching;
                }

                break;
            case BatState.Searching:

                // Be aware of targets
                SearchForPlayer();
                SearchForLight();

                if (searchTimer > 0)
                {
                    searchTimer -= Time.deltaTime;
                }
                else
                {
                    // Remove collision
                    collider2d.enabled = false;

                    // Change indicator
                    indicatorRenderer.color = Color.clear;

                    // Change aggro
                    target = Target.Home;

                    // Change state
                    batState = BatState.Returning;
                }

                break;
            case BatState.Returning:

                // Travel home
                ChaseTarget(target);

                // Be aware of targets
                SearchForPlayer();
                SearchForLight();

                // If target changed
                if (target == Target.Player)
                {
                    // Red indicator
                    indicatorRenderer.color = Color.red;

                    // Change state
                    batState = BatState.AggroPlayer;
                }

                // If target changed
                if (target == Target.Light)
                {
                    // Red indicator
                    indicatorRenderer.color = Color.yellow;

                    // Change state
                    batState = BatState.AggroLight;
                }

                // If reached home
                if (Vector2.Distance(transform.position, homePosition) <= returnRange)
                {
                    // Show ring
                    // aggroRangeDisplay.enabled = true;

                    // Stop moving
                    rigidbody2d.velocity = Vector2.zero;

                    // Enable collision
                    collider2d.enabled = true;

                    // Change indicator
                    indicatorRenderer.color = Color.clear;

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

    private void SearchForPlayer()
    {
        // Check if player is close
        if (Vector3.Distance(transform.position, playerTransform.position) <= aggroRange)
        {
            // Get direction
            var direction = (playerTransform.position - transform.position).normalized;

            // Now check for line of sight
            var visionHit = Physics2D.Raycast(transform.position, direction, aggroRange, groundLayer);

            // If NOT obstructed
            if (!visionHit)
            {
                // Set target
                target = Target.Player;
            }
        }
    }

    private void SearchForLight()
    {
        if (Vector3.Distance(transform.position, lightTransform.position) <= aggroRange)
        {
            // Get direction
            var direction = (lightTransform.position - transform.position).normalized;

            // Now check for line of sight
            var visionHit = Physics2D.Raycast(transform.position, direction, aggroRange, groundLayer);

            // If NOT obstructed
            if (!visionHit)
            {
                // Set target
                target = Target.Light;
            }
        }
    }

    private void ChaseTarget(Target target)
    {
        // Follow the target
        // agent.SetDestination(targetTransform.position);

        Vector3 direction;
        switch (target)
        {
            case Target.Player:

                direction = playerTransform.position - transform.position;
                direction.Normalize();

                // Move in direction of target
                rigidbody2d.velocity = direction * moveSpeed;

                break;
            case Target.Light:

                direction = lightTransform.position - transform.position;
                direction.Normalize();

                // Move in direction of target
                rigidbody2d.velocity = direction * moveSpeed;

                break;
            case Target.Home:

                direction = homePosition - transform.position;
                direction.Normalize();

                // Move in direction of target
                rigidbody2d.velocity = direction * moveSpeed;

                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the target can be damaged
        if (other.transform.parent.TryGetComponent(out DamageHandler damageHandler))
        {
            // Damage it
            damageHandler.Kill();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(homePosition, returnRange);
    }
}
