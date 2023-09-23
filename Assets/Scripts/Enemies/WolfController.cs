using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : MonoBehaviour
{
    private enum WolfState { Idle, Running, Aware, Preparing, Attacking, Dead };

    [Header("Components")]
    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private DamageHandler damageHandler;
    [SerializeField] private Collider2D groundCollider;
    [SerializeField] private Collider2D hitboxCollider;
    [SerializeField] private Rigidbody2D rigidbody2d;
    [SerializeField] private SpriteRenderer indicatorRenderer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform dropCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float checkRadius = 0.25f;

    [Header("Idle Settings")]
    [SerializeField] private float idleDuation = 1f;

    [Header("Run Settings")]
    [SerializeField] private float runDuration = 1f;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Aware Settings")]
    [SerializeField] private float aggroRange = 4f;
    [SerializeField] private float prepareDuration = 0.5f;
    [SerializeField] private Sprite awareSprite;

    [Header("Attack Settings")]
    [SerializeField] private Vector2 attackForce;
    [SerializeField] private Sprite attackSprite;

    [Header("Debugging")]
    [SerializeField] private WolfState wolfState;
    [SerializeField] private float idleTimer = 0f;
    [SerializeField] private float runTimer = 0f;
    [SerializeField] private float prepareTimer = 0f;

    private void Awake()
    {
        animationHandler = GetComponent<AnimationHandler>();
        damageHandler = GetComponent<DamageHandler>();
        rigidbody2d = GetComponentInChildren<Rigidbody2D>();
    }

    private void Start()
    {
        // Get targets
        playerTransform = PlayerController.instance.transform;

        // Set start state
        rigidbody2d.velocity = Vector2.zero;
        animationHandler.ChangeAnimation("Idle");
        runTimer = runDuration;
        wolfState = WolfState.Idle;
    }

    private void Update()
    {
        if (damageHandler.IsDead() && wolfState != WolfState.Dead)
        {
            // Stop movement
            rigidbody2d.velocity = Vector2.zero;

            // Disable hitbox
            hitboxCollider.enabled = false;

            // Change animation
            animationHandler.ChangeAnimation("Die");

            // Change state
            wolfState = WolfState.Dead;
        }

        switch (wolfState)
        {
            case WolfState.Idle:

                if (idleTimer > 0)
                {
                    idleTimer -= Time.deltaTime;
                }
                else
                {
                    // Randomly change facing direction
                    RandomFlip();

                    // Move in facing direction
                    rigidbody2d.velocity = transform.right * moveSpeed;

                    runTimer = runDuration;
                    animationHandler.ChangeAnimation("Run");
                    wolfState = WolfState.Running;
                }

                // Check for player
                if (PlayerInTerritory())
                {
                    indicatorRenderer.sprite = awareSprite;
                    indicatorRenderer.color = Color.white;

                    // Face player
                    var direction = playerTransform.position - transform.position;
                    if (direction.x > 0)
                        transform.rotation = Quaternion.identity;
                    else if (direction.x < 0)
                        transform.rotation = Quaternion.Euler(0, 180, 0);

                    rigidbody2d.velocity = Vector2.zero;
                    animationHandler.ChangeAnimation("Sit");
                    wolfState = WolfState.Aware;
                }

                break;
            case WolfState.Running:

                if (runTimer > 0 && !EndOfPlatform())
                {
                    runTimer -= Time.deltaTime;
                }
                else
                {
                    // Move in direction of target
                    rigidbody2d.velocity = Vector2.zero;

                    idleTimer = idleDuation;
                    animationHandler.ChangeAnimation("Idle");
                    wolfState = WolfState.Idle;
                }

                // Check for player
                if (PlayerInTerritory())
                {
                    indicatorRenderer.sprite = awareSprite;
                    indicatorRenderer.color = Color.white;

                    // Face player
                    var direction = playerTransform.position - transform.position;
                    if (direction.x > 0)
                        transform.rotation = Quaternion.identity;
                    else if (direction.x < 0)
                        transform.rotation = Quaternion.Euler(0, 180, 0);

                    rigidbody2d.velocity = Vector2.zero;
                    animationHandler.ChangeAnimation("Sit");
                    wolfState = WolfState.Aware;
                }

                break;
            case WolfState.Aware:

                // Do nothing.

                // If player leaves territory
                if (!PlayerInTerritory())
                {
                    indicatorRenderer.color = Color.clear;

                    idleTimer = idleDuation;
                    animationHandler.ChangeAnimation("Idle");
                    wolfState = WolfState.Idle;
                }

                // If player comes too close
                if (Vector3.Distance(transform.position, playerTransform.position) <= aggroRange)
                {
                    indicatorRenderer.sprite = attackSprite;
                    indicatorRenderer.color = Color.red;
                    prepareTimer = prepareDuration;
                    animationHandler.ChangeAnimation("Prepare");
                    wolfState = WolfState.Preparing;
                }

                break;
            case WolfState.Preparing:

                prepareTimer -= Time.deltaTime;

                if (prepareTimer <= 0)
                {
                    // Play sound
                    AudioManager.instance.PlaySFX("Wolf Attack");

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
                    idleTimer = idleDuation;
                    animationHandler.ChangeAnimation("Idle");
                    wolfState = WolfState.Idle;
                }

                break;
            case WolfState.Dead:

                // Do nothing.

                break;
        }
    }

    private bool PlayerInTerritory()
    {
        var hit = Physics2D.Raycast(hitboxCollider.bounds.center, Vector2.left, float.MaxValue, playerLayer | groundLayer);
        if (hit && hit.transform == playerTransform)
        {
            return true;
        }

        hit = Physics2D.Raycast(hitboxCollider.bounds.center, Vector2.right, float.MaxValue, playerLayer | groundLayer);
        if (hit && hit.transform == playerTransform)
        {
            return true;
        }

        return false;
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
        Gizmos.DrawWireSphere(dropCheck.position, checkRadius);
        Gizmos.DrawWireSphere(wallCheck.position, checkRadius);

        var position = groundCollider.bounds.center - new Vector3(0f, groundCollider.bounds.extents.y, 0f);
        var size = new Vector2(0.9f * groundCollider.bounds.size.x, 0.1f);
        Gizmos.DrawWireCube(position, size);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(hitboxCollider.bounds.center, Vector3.left * 100f);
        Gizmos.DrawRay(hitboxCollider.bounds.center, Vector3.right * 100f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
