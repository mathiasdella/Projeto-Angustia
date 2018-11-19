using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    public const float MIN_RUN_STAMINA = 25;
    public const float STAMINA_DECAY_RATE = 20;
    public const float STAMINA_WALK_RECOVERY_RATE = 10;
    public const float STAMINA_IDLE_RECOVERY_RATE = 15;
    public const float BREATH_TIME = 1.5f;
    public const float KEY_THRESHOLD = 0.01f;


    public float walkSpeed = 100;
    public float runSpeed = 200;

    public float currentStamina = 100;
    public float maxStamina = 100;

    public float currentStress = 0;
    public float maxStress = 100;

    public float currentSanity = 100;
    public float maxSanity = 100;

    CharacterState charState = CharacterState.Idle;
    SanityState sanityState = SanityState.High;


    public CharacterController charController;

    private float busyTimer = 0;

    public bool IsIdle { get { return charState == CharacterState.Idle; } }
    public bool IsWalking { get { return charState == CharacterState.Walking; } }
    public bool IsRunning { get { return charState == CharacterState.Running; } }
    public bool IsHiding { get { return charState == CharacterState.Hiding; } }
    public bool IsBusy { get { return charState == CharacterState.Busy; } }

    private void Awake()
    {
        //  Get the CharacterController component.
        if (charController == null)
        {
            charController = GetComponent<CharacterController>();
            if (charController == null)
            {
                Debug.LogError("CharacterController component not found!");
            }
        }

        //  Set starting stats.
        currentStamina = maxStamina;
        currentStress = 0;
        currentSanity = maxSanity;
        charState = CharacterState.Idle;
        sanityState = SanityState.High;
    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        //  Character Movement
        CharControls();

        //  Character Interactions
        CharacterInteractions();
    }

    private void CharControls()
    {
        float dt = Time.fixedDeltaTime;

        //  Character cant be controlled if busy.
        if (IsBusy)
        {
            //  Update busy timer.
            busyTimer -= dt;

            //  Update state if needed.
            if (busyTimer <= 0)
            {
                busyTimer = 0;
                charState = CharacterState.Idle;
            }

            //  Return.
            return;
        }

        //  Apply Stamina changes.
        if (IsRunning)
        {
            currentStamina = Mathf.Max(0, currentStamina - STAMINA_DECAY_RATE * dt);
        }
        else if (IsWalking)
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + STAMINA_WALK_RECOVERY_RATE * dt);
        }
        else
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + STAMINA_IDLE_RECOVERY_RATE * dt);
        }

        //  Character cant be controlled if breathless.
        if (IsRunning && currentStamina <= 0)
        {
            //  Set state to busy with breathless timer and animation.
            charState = CharacterState.Busy;
            busyTimer = BREATH_TIME;

            //  Return.
            return;
        }

        //  Check if the player wants to interact with an object.
        if (Mathf.Abs(Input.GetAxis("Interact")) > KEY_THRESHOLD)
        {
            //  Attempt to interact with an object.
            if (TryInteract())
            {
                //  Return if the player successfully interacted with an object.
                return;
            }
        }

        //  Check if the player wants to move.
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > KEY_THRESHOLD)
        {
            //  Quit Hiding before moving.
            if (charState == CharacterState.Hiding)
            {
                EndHiding();
                return;
            }

            float horizontalAxis = Input.GetAxis("Horizontal");
            bool isRunning = Mathf.Abs(horizontalAxis) > 0.01f && Mathf.Abs(Input.GetAxis("Run")) > 0.01f && (charState == CharacterState.Running || currentStamina >= MIN_RUN_STAMINA);

            float ySpeed = charController.isGrounded ? 0 : Physics.gravity.y * dt;

            if (isRunning)
            {
                charState = CharacterState.Running;
                charController.Move(new Vector3(runSpeed * horizontalAxis * dt * GameMaster.PixelToWorld, ySpeed, 0));
            }
            else
            {
                charState = CharacterState.Walking;
                charController.Move(new Vector3(walkSpeed * horizontalAxis * dt * GameMaster.PixelToWorld, ySpeed, 0));
            }

            return;
        }

        charState = CharacterState.Idle;
        return;
    }

    private bool TryInteract()
    {
        return false;
    }

    public void CharacterInteractions()
    {

    }

    public void BeginHiding()
    {

    }

    public void EndHiding()
    {

    }

    public enum CharacterState
    {
        Idle,
        Walking,
        Running,
        Hiding,
        Busy
    }

    public enum SanityState
    {
        Low,
        Medium,
        High
    }
}
