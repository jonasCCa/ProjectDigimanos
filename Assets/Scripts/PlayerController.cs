using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    [Header("References")]
    public StatsController stats;
    public CharacterController controller;
    public GameObject playerCamera;
    public PlayerInput playerInput;
    Collider selfCollider;

    [Header("Movement Control")]
    public float xSpeed = 10;
    public float ySpeed = 10;
    public float turningSpeed = 1000;
    
    [Header("Jump Control")]
    public bool isJumping;
    public float jumpForce = 5;
    public float gravity = 90;
    public float coyoteTime = 0.15f;
    public float minGroundTime = 0.04f;
    
    [Header("Ground-Checking")]
    public bool onGround;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.5f;
    public float bottomOffset = 0.7f;

    //PoT -> Player on Top
    [Header("PoT-Checking")]
    public bool hasPoT;
    public LayerMask playerLayer;
    public Vector3 potCheckBoxSize = new Vector3(){x = 0.5f, y = 0.3f, z = 0.5f};
    public float topOffset = 1;

    [Header("Attacking")]
    public GameObject weapon;
    public LayerMask enemyLayer;
    public Animator animator;
    [SerializeField] private bool isAttacking;

    [Header("Info")]
    [SerializeField] private float vertSpeed;
    [SerializeField] private float inputX;
    [SerializeField] private float inputY;
    [SerializeField] private bool inputJ;
    [SerializeField] private float timeOnGround;
    [SerializeField] private float timeSinceGrounded;

    [Header("Tests")]
    public GameObject indicator;
    //Debugging purposes
    //[SerializeField] private Vector3 movement;

    void Start() {
        stats = GetComponent<StatsController>();
        
        selfCollider = GetComponent<Collider>();
        playerInput = GetComponent<PlayerInput>();
        //playerInput.SwitchCurrentActionMap("Gameplay");

        controller = GetComponent<CharacterController>();

        isJumping = true;
    }

    void Update() {
        // Old input system
        //inputX = Input.GetAxisRaw("Horizontal");
        //inputY = Input.GetAxisRaw("Vertical");
        //inputJ = Input.GetAxis("Jump");
        
        if(!animator.IsInTransition(0)) {
            if(animator.GetCurrentAnimatorStateInfo(0).IsName("Light_Attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("Heavy_Attack"))
                isAttacking = true;
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                isAttacking = false;
            
        }
    }

    void FixedUpdate() {
        //Checks ground + player
        onGround = GroundCheck(-transform.up);

        //Starts downwards calculation
        DoGravity();

        //Initializes movement calculation
        Vector3 movement = new Vector3(inputX, 0, inputY);
        //Debugging purposes
        //movement = new Vector3(inputX, 0, inputY);
        
        //Stops diagonals from being dumb
        //movement.Normalize();
        movement = Vector3.ClampMagnitude(movement, 1f);
        //Applies speed
        movement.x *= xSpeed;
        movement.z *= ySpeed;

        //Checks PoT
        hasPoT = PoTCheck(transform.up);

        //Starts jumping calculation
        if(inputJ && !hasPoT) {
            DoJumping();
        }

        //Rotate to where it's moving
        if(movement.magnitude != 0) {
            float step = turningSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation,  Quaternion.LookRotation(movement), step);
        }

        //Applies Vertical speed (gravity + jumping)
        movement.y += vertSpeed;        

        //Applies movement by time
        controller.Move(movement * Time.deltaTime);
    }

    private void DoGravity() {
        //If on ground, coyote time is now valid and plz don't fall anymore
        if(onGround) {
            //Resets coyote time
            timeOnGround = coyoteTime;

            //Tracks if player's been a minimum time on ground before jumping again
            if(timeSinceGrounded<minGroundTime)
                timeSinceGrounded += Time.deltaTime;
            else
                isJumping = false;

            //Makes sure you don't accelerate donwards while on ground
            if(vertSpeed < 0)
                vertSpeed = 0f;
        } else {
            // If player is going up and hits something
            if(vertSpeed > 0 && CeilingCheck(transform.up))
                vertSpeed = 0;
        }
        //Coyote time is not forever, y'know?
        if (timeOnGround > 0)
            timeOnGround -= Time.deltaTime;
        
        //Gravity 😎
        vertSpeed -= gravity * Time.deltaTime;
    }

    private void DoJumping() {
        if(!isJumping) {
            isJumping = true;
            timeSinceGrounded = 0;
            //If coyote time is valid
            if (timeOnGround > 0) {
                //Makes sure coyote time isn't valid anymore
                timeOnGround = 0;
                
                //Stop falling (in order to do the coyote time magic)
                vertSpeed = 0;
                //Physics guy said so
                vertSpeed += Mathf.Sqrt(jumpForce * 2 * gravity);
            }
        }
    }


    //Debugging purposes
    //[Header("Collision Debugging")]
    List<Collider> hitColliders;

    //Returns true if is on ground or in top of another player
    private bool GroundCheck(Vector3 direction)
    {
        // Initializes point to check ground 
        Vector3 pos = transform.position + (direction * bottomOffset);
        // Check objects with groundLayer within a certain raidus from that point
        hitColliders = Physics.OverlapSphere(pos, groundCheckRadius, groundLayer).ToList();

        // Removes self from list
        hitColliders.Remove(selfCollider);

        //If anything was found, then it's grounded
        if (hitColliders.Count > 0)
            return true;
        else {
            // Same offset as PoT check
            pos = transform.position + (direction * topOffset);
            // Double checks downwards for player collision
            hitColliders = Physics.OverlapBox(pos, potCheckBoxSize, Quaternion.identity, playerLayer).ToList();
            // Removes self from list
            hitColliders.Remove(selfCollider);

            if (hitColliders.Count > 0)
            return true;
        }

        return false;
    }

    //Returns true if hits ceiling
    private bool CeilingCheck(Vector3 direction)
    {
        //Initializes point to check ceiling 
        Vector3 pos = transform.position + (direction * bottomOffset);
        //Check objects within a certain raidus from that point
        hitColliders = Physics.OverlapSphere(pos, groundCheckRadius).ToList();


        hitColliders.Remove(selfCollider);

        //If anything (besides self) was found, then player's head goes bonk
        if (hitColliders.Count > 0)
            return true;

        return false;
    }

    //Returns true if there's a player on top
    private bool PoTCheck(Vector3 direction) {
        //Initializes point to check PoT 
        Vector3 pos = transform.position + (direction * topOffset);
        //Check objects with playerLayer within a certain raidus from that point
        //List<Collider> hitColliders = Physics.OverlapSphere(pos, potCheckBoxSize, playerLayer).ToList();
        //List<Collider> hitColliders = Physics.OverlapBox(pos, potCheckBoxSize, Quaternion.identity, playerLayer);

        //Debugging purposes
        hitColliders = Physics.OverlapBox(pos, potCheckBoxSize, Quaternion.identity, playerLayer).ToList();
        //hitColliders = Physics.OverlapSphere(pos, potCheckBoxSize, playerLayer).ToList();

        //Delete Self from hitColliders
        //Collider selfCollider = GetComponent<Collider>();
        //if(hitColliders.Contains(selfCollider)) {
            hitColliders.Remove(selfCollider);
        //}

        //If anything was found, then it has PoT
        if (hitColliders.Count > 0)
            return true;

        return false;
    }


    ////////////////////////
    //  New Input System  //
    //          -         //
    // Event Callbacks on //
    //   <Player Input>   //
    ////////////////////////
    public void OnMove(InputAction.CallbackContext value) {
        inputX = value.ReadValue<Vector2>().x;
        inputY = value.ReadValue<Vector2>().y;
    }

    public void OnJump(InputAction.CallbackContext value) {
        // CallbackContext are Finite-State Machines:
        // started -> performed -> canceled

        //if(value.performed || value.canceled) {
        if(value.performed) {
            inputJ = value.ReadValueAsButton();
            //Debug.Log("Entrou OnJump(): " + inputJ);
        }
    }

    public void OnLightAttack(InputAction.CallbackContext value) {
        if(value.performed && value.ReadValueAsButton()==true && !isAttacking) {
            if(animator.isActiveAndEnabled)
                animator.Play("Light_Attack");
        }
    }

    public void OnHeavyAttack(InputAction.CallbackContext value) {
        if(value.performed && value.ReadValueAsButton()==true && !isAttacking) {
            if(animator.isActiveAndEnabled)
                animator.Play("Heavy_Attack");
        }
    }




    //Draws fancy stuff on editor scene viewer
    void OnDrawGizmosSelected()
    {
        //Ground check sphere
        Gizmos.color = Color.yellow;
        Vector3 pos = transform.position + (-transform.up * bottomOffset);
        Gizmos.DrawWireSphere(pos, groundCheckRadius);

        //Ground player doublecheck box
        Gizmos.color = new Color(1, 0.647f, 0, 1); //orange
        pos = transform.position + (-transform.up * topOffset);
        Gizmos.DrawWireCube(pos, potCheckBoxSize*2);

        //Ceiling check sphere
        Gizmos.color = Color.yellow;
        pos = transform.position + (transform.up * bottomOffset);
        Gizmos.DrawWireSphere(pos, groundCheckRadius);
        
        //PoT check sphere
        Gizmos.color = new Color(1, 0.647f, 0, 1); //orange
        pos = transform.position + (transform.up * topOffset);
        Gizmos.DrawWireCube(pos, potCheckBoxSize*2);
    }
}
