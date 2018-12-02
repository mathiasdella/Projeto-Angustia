using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    public const float MIN_RUN_STAMINA = 25;
    public const float STAMINA_DECAY_RATE = 25;
    public const float STAMINA_WALK_RECOVERY_RATE = 10;
    public const float STAMINA_IDLE_RECOVERY_RATE = 15;
    public const float BREATH_TIME = 1.5f;
    public const float KEY_THRESHOLD = 0.01f;
    public const float VERTICAL_SPEED = 35f;
    public const float MAX_VERTICAL = 45;
    public const float MIN_VERTICAL = -45;


    public float walkSpeed = 1.0f;
    public float runSpeed = 2.0f;

    public float currentStamina = 100;
    public float maxStamina = 100;

    public float currentStress = 0;
    public float maxStress = 100;

    public float currentSanity = 100;
    public float maxSanity = 100;

    public CharacterState charState = CharacterState.Idle;
    public SanityState sanityState = SanityState.High;

    public int orientation = 1;
    public Animator charAnimator;
    public CharacterController charController;
    public SpriteRenderer charSpriteRenderer;

    private float busyTimer = 0;

    private List<SceneObject> nearbyObjects;
    private int selectedObj = 0;

    private Transform handPivot;

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

        //  Get the Animator component.
        if (charAnimator == null)
        {
            charAnimator = GetComponent<Animator>();
            if (charAnimator == null)
            {
                Debug.LogError("Animator componenet not found!");
            }
        }

        //  Get the SpriteRenderer component.
        if (charSpriteRenderer == null)
        {
            charSpriteRenderer = GetComponent<SpriteRenderer>();
            if (charSpriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer componenet not found!");
            }
        }

        //  Get the hand pivot transform
        if (handPivot == null)
        {
            handPivot = transform.Find("Hand Pivot");
            if (handPivot == null)
            {
                Debug.LogError("Hand Pivot not found!");
            }
        }

        //  Set starting stats.
        currentStamina = maxStamina;
        currentStress = 0;
        currentSanity = maxSanity;
        charState = CharacterState.Idle;
        sanityState = SanityState.High;

        //  Initialize collections.
        nearbyObjects = new List<SceneObject>();
    }

    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            if (charController.isGrounded)
            {
                break;
            }
            charController.Move(Physics.gravity);
        }
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        //  Character Controls
        CharControls();

        //  Character Animations
        CharAnimations();
    }

    private void CharControls()
    {
        float dt = Time.fixedDeltaTime;

        //  Ignore controls if character is busy.
        if (busyTimer > 0)
        {
            //  Update busy timer.
            busyTimer -= dt;

            //  Return.
            return;
        }

        //  Update Character state.
        if (IsBusy && busyTimer <= 0)
        {
            busyTimer = 0;
            charState = CharacterState.Idle;
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
                //  Return if the player successfully interacted with an object and became busy.
                if (busyTimer > 0)
                {
                    return;
                }
            }
        }

        //  Check if the player wants to move the flashlight.
        float verticalAxis = Input.GetAxis("Vertical");
        if (Mathf.Abs(verticalAxis) > KEY_THRESHOLD)
        {
            float zRotation = handPivot.transform.rotation.eulerAngles.z;
            while (zRotation < 0)
            {
                zRotation = zRotation + 360;
            }
            while (zRotation > 360)
            {
                zRotation = zRotation - 360;
            }

            zRotation = zRotation + verticalAxis * VERTICAL_SPEED * dt * transform.localScale.x;

            if (zRotation > MAX_VERTICAL && zRotation <= 180)
            {
                zRotation = MAX_VERTICAL;
            }
            else if (zRotation < 360 + MIN_VERTICAL && zRotation > 180)
            {
                zRotation = 360 + MIN_VERTICAL;
            }

            Vector3 targetEulers = new Vector3(0, 0, zRotation);
            handPivot.eulerAngles = targetEulers;
        }

        //  Check if the player wants to move.
        float horizontalAxis = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizontalAxis) > KEY_THRESHOLD)
        {
            //  Quit Hiding before moving.
            if (charState == CharacterState.Hiding)
            {
                EndHiding();
                return;
            }

            bool isRunning = Mathf.Abs(horizontalAxis) > 0.01f && Mathf.Abs(Input.GetAxis("Run")) > 0.01f && (charState == CharacterState.Running || currentStamina >= MIN_RUN_STAMINA);

            float ySpeed = charController.isGrounded ? 0 : Physics.gravity.y * dt;

            if (isRunning)
            {
                charState = CharacterState.Running;
                charController.Move(new Vector3(runSpeed * horizontalAxis * dt, ySpeed, 0));
            }
            else
            {
                charState = CharacterState.Walking;
                charController.Move(new Vector3(walkSpeed * horizontalAxis * dt, ySpeed, 0));
            }

            //  Set the character orientation.
            orientation = horizontalAxis > 0 ? 1 : -1;

            return;
        }

        //  Character is Idle.
        charState = CharacterState.Idle;
        return;
    }

    private void CharAnimations()
    {
        if (IsWalking)
        {
            charAnimator.SetFloat("speed", 1);
        }
        else if (IsRunning)
        {
            charAnimator.SetFloat("speed", 2);
        }
        else
        {
            charAnimator.SetFloat("speed", 0);
        }

        if (orientation > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            //charSpriteRenderer.flipX = false;
        }
        else if (orientation < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            //charSpriteRenderer.flipX = true;
        }
    }

    private bool TryInteract()
    {
        //  Return if list is empty.
        if (nearbyObjects.Count <= 0)
        {
            return false;
        }

        //  Get the currently selected object.
        SceneObject obj = nearbyObjects[selectedObj % nearbyObjects.Count];

        //  Interact with the object if possible. Return false otherwise.
        if (obj.isInteractible)
        {
            return obj.OnInteract(this);
        }
        else
        {
            return false;
        }
    }

    public void BeginHiding(float busytime)
    {
        busyTimer = busytime;
        charState = CharacterState.Hiding;
    }

    public void EndHiding()
    {
        charState = CharacterState.Idle;
    }

    public void BeginInteracting(float busyTime)
    {
        busyTimer = busyTime;
        charState = CharacterState.Idle;
        charAnimator.SetTrigger("interact");
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col == null)
        {
            return;
        }

        SceneObject sobj = col.GetComponent<SceneObject>();
        if (sobj != null && !nearbyObjects.Contains(sobj))
        {
            nearbyObjects.Add(sobj);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col == null)
        {
            return;
        }

        SceneObject sobj = col.GetComponent<SceneObject>();
        if (sobj != null && nearbyObjects.Contains(sobj))
        {
            nearbyObjects.Remove(sobj);
        }
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
