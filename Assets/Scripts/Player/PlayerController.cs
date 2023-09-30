using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private enum PlayerState { Idle, Run, Rise, Fall, Crouch, Crouchwalk, Exit, Enter, Dead };

    [Header("Components")]
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private MovementHandler movementHandler;
    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private DamageHandler damageHandler;
    [SerializeField] private JuiceHandler juiceHandler;

    [Header("Data")]
    [SerializeField] private PlayerState playerState;

    public static PlayerController instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        // Get refs
        inputHandler = GetComponent<InputHandler>();
        movementHandler = GetComponent<MovementHandler>();
        animationHandler = GetComponent<AnimationHandler>();
        damageHandler = GetComponent<DamageHandler>();
        juiceHandler = GetComponent<JuiceHandler>();
    }

    private void Start()
    {
        // Get current checkpoint
        var checkpoint = GameManager.instance.GetCurrentCheckpoint();

        // If a checkpoint exists
        if (checkpoint != Vector3.back)
        {
            // Simple relocate there, and idle
            transform.position = checkpoint;

            // Set state
            playerState = PlayerState.Idle;

            // Set animation
            animationHandler.ChangeAnimation(playerState.ToString());
        }
        else
        {
            // Do walk in animations from where you are

            // Set state
            playerState = PlayerState.Enter;

            // Set animation
            animationHandler.ChangeAnimation(playerState.ToString());
        }

        // Open scene
        TransitionManager.instance.OpenScene();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for death
        if (playerState != PlayerState.Dead && damageHandler.IsDead())
        {
            // Stop moving
            movementHandler.Stop();

            // Stop sound
            AudioManager.instance.StopSFX("Run");

            // Change animation
            animationHandler.ChangeAnimation("Dead");

            // Change states
            playerState = PlayerState.Dead;

            // Restart in a bit
            StartCoroutine(DelayedRestart());
        }

        switch (playerState)
        {
            case PlayerState.Idle:

                // Handle running
                HandleRunning();

                // Handle jumping
                HandleJumping();

                // Handle crouching
                HandleCrouching();

                // Check for running
                if (movementHandler.IsRunning())
                {
                    // Play particles
                    juiceHandler.StartRun();

                    // Play sound
                    InvokeRepeating("FootstepsSFX", 0f, 0.33f);

                    // Change states
                    playerState = PlayerState.Run;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());

                    return;
                }

                // Check for crouching
                if (movementHandler.IsCrouching())
                {
                    // Enable crouch
                    movementHandler.StartCrouch();

                    // Change states
                    playerState = PlayerState.Crouch;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());

                    return;
                }

                // Check for jumping
                if (!movementHandler.IsGrounded())
                {
                    // Play effect
                    juiceHandler.PlayJump();

                    // Play sound
                    AudioManager.instance.PlaySFX("Jump");

                    // Change states
                    playerState = PlayerState.Rise;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());

                    return;
                }

                // Check for falling
                if (movementHandler.IsFalling())
                {
                    // Change states
                    playerState = PlayerState.Fall;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());

                    return;
                }

                break;
            case PlayerState.Run:

                // Handle running
                HandleRunning();

                // Handle jumping
                HandleJumping();

                // Handle crouching
                HandleCrouching();

                // Check for idling
                if (!movementHandler.IsRunning())
                {
                    // Stop particles
                    juiceHandler.StopRun();

                    // Stop sound
                    CancelInvoke("FootstepsSFX");

                    // Change states
                    playerState = PlayerState.Idle;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for crouch walk
                if (movementHandler.IsCrouching())
                {
                    // Stop particles
                    juiceHandler.StopRun();

                    // Stop sound
                    CancelInvoke("FootstepsSFX");

                    // Play sound
                    InvokeRepeating("FootstepsSFX", 0f, 0.66f);

                    // Change states
                    playerState = PlayerState.Crouchwalk;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for jump
                if (movementHandler.IsRising())
                {
                    // Stop particles
                    juiceHandler.StopRun();

                    // Stop sound
                    CancelInvoke("FootstepsSFX");

                    // Play effect
                    juiceHandler.PlayJump();

                    // Play sound
                    AudioManager.instance.PlaySFX("Jump");

                    // Change states
                    playerState = PlayerState.Rise;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for falling
                if (movementHandler.IsFalling())
                {
                    // Stop particles
                    juiceHandler.StopRun();

                    // Stop sound
                    CancelInvoke("FootstepsSFX");

                    // Change states
                    playerState = PlayerState.Fall;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                break;
            case PlayerState.Rise:

                // Handle running
                HandleRunning();

                // Handle jumping
                HandleJumping();

                // Handle jump cancel
                if (inputHandler.GetJumpInputUp()) movementHandler.EndJumpEarly();

                // Check for landing
                if (movementHandler.IsGrounded())
                {
                    // Change states
                    playerState = PlayerState.Idle;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for falling
                if (movementHandler.IsFalling())
                {
                    // Change states
                    playerState = PlayerState.Fall;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                break;
            case PlayerState.Fall:

                // Handle running
                HandleRunning();

                // Handle jumping
                HandleJumping();

                // Check for landing
                if (movementHandler.IsGrounded())
                {
                    // Play particles
                    juiceHandler.PlayLand();

                    // Play sound
                    AudioManager.instance.PlaySFX("Land");

                    // Change states
                    playerState = PlayerState.Idle;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for coyote jumps
                if (movementHandler.IsRising())
                {
                    // Play effect
                    juiceHandler.PlayJump();

                    // Play sound
                    AudioManager.instance.PlaySFX("Jump");

                    // Change states
                    playerState = PlayerState.Rise;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                break;
            case PlayerState.Crouch:

                // Handle running
                HandleRunning();

                // Handle crouching
                HandleCrouching();

                // Handle dropping
                HandleDropping();

                // Check for idling
                if (!movementHandler.IsCrouching())
                {
                    // Disable crouch
                    movementHandler.EndCrouch();

                    // Change states
                    playerState = PlayerState.Idle;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for crouch walk
                if (movementHandler.IsRunning())
                {
                    // Play sound
                    InvokeRepeating("FootstepsSFX", 0f, 0.66f);

                    // Change states
                    playerState = PlayerState.Crouchwalk;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for dropping
                if (movementHandler.IsFalling())
                {
                    // Disable crouch
                    movementHandler.EndCrouch();

                    // Change states
                    playerState = PlayerState.Fall;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                break;
            case PlayerState.Crouchwalk:

                // Handle running
                HandleRunning();

                // Handle crouching
                HandleCrouching();

                // Handle dropping
                HandleDropping();

                // Check for crouch
                if (!movementHandler.IsRunning())
                {
                    // Stop sound
                    CancelInvoke("FootstepsSFX");

                    // Change states
                    playerState = PlayerState.Crouch;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());

                    return;
                }

                // Check for running
                if (!movementHandler.IsCrouching())
                {
                    // Disable crouch
                    movementHandler.EndCrouch();

                    // Play particles
                    juiceHandler.StartRun();

                    // Stop sound
                    CancelInvoke("FootstepsSFX");

                    // Play sound
                    InvokeRepeating("FootstepsSFX", 0f, 0.33f);

                    // Change states
                    playerState = PlayerState.Run;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());

                    return;
                }

                // Check for falling
                if (movementHandler.IsFalling())
                {
                    // Disable crouch
                    movementHandler.EndCrouch();

                    // Change states
                    playerState = PlayerState.Fall;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());

                    return;
                }

                break;
            case PlayerState.Exit:

                // Walk out of the scene
                movementHandler.MoveRight();

                break;
            case PlayerState.Enter:

                // Walk into the scene
                movementHandler.MoveRight();

                // When animation is over...
                if (animationHandler.IsFinished())
                {
                    // Change states
                    playerState = PlayerState.Idle;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                break;
            case PlayerState.Dead:

                // Do nothing...

                // If we are revived
                if (!damageHandler.IsDead())
                {
                    // Change states
                    playerState = PlayerState.Idle;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                break;
            default:
                // Throw error
                throw new System.Exception("STATE NOT IMPLEMENTED.");
        }
    }

    private void FootstepsSFX()
    {
        AudioManager.instance.PlaySFX("Run");
    }

    private IEnumerator DelayedRestart()
    {
        yield return new WaitForSeconds(1f);

        GameManager.instance.RestartLevel();
    }

    private void HandleRunning()
    {
        if (inputHandler.GetLeftInput()) movementHandler.MoveLeft();
        else if (inputHandler.GetRightInput()) movementHandler.MoveRight();
        else movementHandler.Stop();
    }

    private void HandleJumping()
    {
        if (inputHandler.GetJumpInputDown()) movementHandler.Jump();
    }

    private void HandleCrouching()
    {
        if (inputHandler.GetCrouchKey()) movementHandler.StartCrouch();
        else movementHandler.EndCrouch();
    }

    private void HandleDropping()
    {
        if (inputHandler.GetCrouchKey() && inputHandler.GetJumpInputDown()) movementHandler.Drop();
    }
}
