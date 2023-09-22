using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : MonoBehaviour
{
    private enum WolfState { Idle, Running, Preparing, Attacking, Blinded };

    [Header("Components")]
    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private Collider2D groundCollider;
    [SerializeField] private Collider2D hitboxCollider;
    [SerializeField] private Rigidbody2D rigidbody2d;
    [SerializeField] private SpriteRenderer indicatorRenderer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform dropCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform lightTransform;

    [Header("Data")]
    [SerializeField] private WolfState wolfState;
    [SerializeField] private float aggroRange = 7f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float checkRadius = 0.25f;
    [SerializeField] private float walkDuration = 1f;
    [SerializeField] private float breakDuation = 1f;
    [SerializeField] private float walkTimer = 0f;
    [SerializeField] private float breakTimer = 0f;
    [SerializeField] private float blindRange = 2f;
    [SerializeField] private float prepareDuration = 0.5f;
    [SerializeField] private float prepareTimer = 0f;
    [SerializeField] private Vector2 attackForce;

    private void Awake()
    {
        animationHandler = GetComponent<AnimationHandler>();
        rigidbody2d = GetComponentInChildren<Rigidbody2D>();
    }

    private void Start()
    {
        // Get targets
        playerTransform = PlayerController.instance.transform;
        lightTransform = MouseLight.instance.transform;

        // Set start state
        rigidbody2d.velocity = Vector2.zero;
        animationHandler.ChangeAnimation("Idle");
        walkTimer = walkDuration;
        wolfState = WolfState.Idle;
    }

    private void Update()
    {
        switch (wolfState)
        {
            case WolfState.Idle:

                if (breakTimer > 0)
                {
                    breakTimer -= Time.deltaTime;
                }
                else
                {
                    // Randomly change facing direction
                    RandomFlip();

                    // Move in facing direction
                    rigidbody2d.velocity = transform.right * moveSpeed;

                    walkTimer = walkDuration;
                    animationHandler.ChangeAnimation("Run");
                    wolfState = WolfState.Running;
                }

                // Check for player
                if (PlayerInTerritory())
                {
                    // Face player
                    var direction = playerTransform.position - transform.position;
                    if (direction.x > 0)
                        transform.rotation = Quaternion.identity;
                    else if (direction.x < 0)
                        transform.rotation = Quaternion.Euler(0, 180, 0);

                    rigidbody2d.velocity = Vector2.zero;
                    prepareTimer = prepareDuration;
                    animationHandler.ChangeAnimation("Prepare");
                    wolfState = WolfState.Preparing;
                }

                // Check for blind
                if (Vector3.Distance(transform.position, lightTransform.position) <= blindRange)
                {
                    hitboxCollider.enabled = false;
                    indicatorRenderer.color = Color.white;
                    animationHandler.ChangeAnimation("Sit");
                    wolfState = WolfState.Blinded;
                }

                break;
            case WolfState.Running:

                if (walkTimer > 0 && !EndOfPlatform())
                {
                    walkTimer -= Time.deltaTime;
                }
                else
                {
                    // Move in direction of target
                    rigidbody2d.velocity = Vector2.zero;

                    breakTimer = breakDuation;
                    animationHandler.ChangeAnimation("Idle");
                    wolfState = WolfState.Idle;
                }

                // Check for player
                if (PlayerInTerritory())
                {
                    // Face player
                    var direction = playerTransform.position - transform.position;
                    if (direction.x > 0)
                        transform.rotation = Quaternion.identity;
                    else if (direction.x < 0)
                        transform.rotation = Quaternion.Euler(0, 180, 0);

                    rigidbody2d.velocity = Vector2.zero;
                    prepareTimer = prepareDuration;
                    animationHandler.ChangeAnimation("Prepare");
                    wolfState = WolfState.Preparing;
                }

                // Check for blind
                if (Vector3.Distance(transform.position, lightTransform.position) <= blindRange)
                {
                    rigidbody2d.velocity = Vector2.zero;

                    hitboxCollider.enabled = false;
                    indicatorRenderer.color = Color.white;
                    animationHandler.ChangeAnimation("Sit");
                    wolfState = WolfState.Blinded;
                }

                break;
            case WolfState.Preparing:

                prepareTimer -= Time.deltaTime;

                if (prepareTimer <= 0)
                {
                    // rigidbody2d.velocity = new Vector2(transform.right.x * attackForce.x, attackForce.y);
                    var force = new Vector2(transform.right.x * attackForce.x, attackForce.y);
                    rigidbody2d.AddForce(force, ForceMode2D.Impulse);
                    animationHandler.ChangeAnimation("Attack");
                    wolfState = WolfState.Attacking;
                }

                break;
            case WolfState.Attacking:

                // When finally grounded
                if (IsGrounded())
                {
                    rigidbody2d.velocity = Vector2.zero;
                    indicatorRenderer.color = Color.clear;
                    breakTimer = breakDuation;
                    animationHandler.ChangeAnimation("Idle");
                    wolfState = WolfState.Idle;
                }

                break;
            case WolfState.Blinded:

                // Do nothing

                // If light leaves range
                if (Vector3.Distance(transform.position, lightTransform.position) > blindRange)
                {
                    hitboxCollider.enabled = true;
                    indicatorRenderer.color = Color.clear;
                    breakTimer = breakDuation;
                    animationHandler.ChangeAnimation("Idle");
                    wolfState = WolfState.Idle;
                }

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

    private bool PlayerInTerritory()
    {
        return Vector3.Distance(transform.position, playerTransform.position) <= aggroRange
            && (Physics2D.Raycast(transform.position, Vector2.left, aggroRange, playerLayer)
            || Physics2D.Raycast(transform.position, Vector2.right, aggroRange, playerLayer));
    }

    private bool EndOfPlatform()
    {
        // If we hit wall or a drop, then stop
        return Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer) || !Physics2D.OverlapCircle(dropCheck.position, checkRadius, groundLayer);
    }

    private void RandomFlip()
    {
        if (Random.Range(0, 2) == 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
    }

    private bool IsGrounded()
    {
        var position = groundCollider.bounds.center - new Vector3(0f, groundCollider.bounds.extents.y, 0f);
        var size = new Vector2(0.9f * groundCollider.bounds.size.x, 0.1f);

        return Physics2D.OverlapBox(position, size, 0, groundLayer) && rigidbody2d.velocity.y < -0.1f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(dropCheck.position, checkRadius);
        Gizmos.DrawWireSphere(wallCheck.position, checkRadius);

        var position = groundCollider.bounds.center - new Vector3(0f, groundCollider.bounds.extents.y, 0f);
        var size = new Vector2(0.9f * groundCollider.bounds.size.x, 0.1f);
        Gizmos.DrawWireCube(position, size);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.left * aggroRange);
        Gizmos.DrawRay(transform.position, Vector3.right * aggroRange);
    }
}
