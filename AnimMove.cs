using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;// to use input system

public class AnimMove : MonoBehaviour
{  //declare refrence variables
    PlayerControls playerCont;// name of the input sys
    CharacterController controller;
    Animator animator;

    // variables to store optimized setter
    int isRunningHash;
    int isJumpinghash;


    //variables to store player control values
    Vector2 CurrentMovementInput;
    Vector3 CurrentMovement;
    Vector3 velocity;
    bool isMovementPressed;
    bool isJumpPressed; // bool because it set as buttons in input action
    bool isGround;
    float rotationperframe = 5f;
    public float Jumpheight = 3f;
    public float gravity = -9.81f;

    public Transform GroundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public   void Awake()
    {
        playerCont = new PlayerControls();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isRunningHash = Animator.StringToHash("isRunning");
        isJumpinghash = Animator.StringToHash("isJumping");




        playerCont.Character.move.started += OnMovementInput;
        playerCont.Character.move.canceled += OnMovementInput;
        playerCont.Character.move.performed += OnMovementInput; // performed will continously update the 
        playerCont.Character.Jump.started += onJump;
        playerCont.Character.Jump.canceled += onJump;
        //Charater = ActionMaps,move = actions,context =inputSys data, 
    }

    void handleGravity()
    {
        if (controller.isGrounded)// isGrounded is unitys built im
        {
            float groundGravity = -0.5f;
            CurrentMovement.y = groundGravity;
            velocity.y = groundGravity;
        }
        else
        {
            CurrentMovement.y += gravity;
            velocity.y += gravity;
        }
    }

    void OnMovementInput(InputAction.CallbackContext  context )
    {   
        // This Code has data of PlayerControls
        CurrentMovementInput = context.ReadValue<Vector2>();
        CurrentMovement.x = CurrentMovementInput.x *5.0f;
        CurrentMovement.z = CurrentMovementInput.y * 5.0f;
        isMovementPressed = CurrentMovementInput.x != 0 || CurrentMovementInput.y != 0; // cause x & y values are grtr or less than 0 when WASD

    }
    void onJump(InputAction.CallbackContext context)
    {

        if ( isGround)
        {
            isGround = false;
            isJumpPressed = context.ReadValueAsButton();
            velocity.y = Mathf.Sqrt(Jumpheight * -2f * gravity);
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);// has to multiply by Time Sqr
        }
        else if (!isGround)
        {
            isGround = true;
        }
        
    }

    void handleRotation()
    {
       
        Vector3 positionToLookAt;
        //the change in postion our character should point to
        positionToLookAt.x = CurrentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = CurrentMovement.z;
        // the current rotation of our character
        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed )
        {
            Quaternion target = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, target, rotationperframe * Time.deltaTime);
        }
    }


    void handleAnimtion()
    {   
        // get parameter values from animator
        bool isRunning = animator.GetBool(isRunningHash);
        bool isJumping = animator.GetBool(isJumpinghash);

        if(isMovementPressed && !isRunning)
        {
            animator.SetBool("isRunning" ,true);
        }
        else if (!isMovementPressed && isRunning)
        {
            animator.SetBool("isRunning", false);
        }
        if (!isJumping && isJumpPressed)
        {
            animator.SetBool("isJumping", true);
        }
        else if (isJumping && !isJumpPressed) 
        {
            animator.SetBool("isJumping",false);
        }
        
    }

    private void Update()
    {

        isGround = Physics.CheckSphere(GroundCheck.position, groundDistance, groundMask);
        if (isGround && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        handleRotation();
        handleAnimtion();
        controller.Move(velocity  * Time.deltaTime);// imp
        controller.Move(CurrentMovement*Time.deltaTime); 
    }

    private void OnEnable()
    {   
        // Enables the Character action  maps
        playerCont.Character.Enable();
    }
    private void OnDisable()
    {
        playerCont.Character.Disable();

    }


}

