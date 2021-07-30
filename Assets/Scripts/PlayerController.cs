using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    [Header("References")]
    public CharacterController controller;

    [Header("Movement Control")]
    public float xSpeed = 10;
    public float ySpeed = 10;
    
    [Header("Jump Control")]
    public bool isJumping;
    public float jumpForce = 5;
    public float gravity = 90;
    
    [Header("Ground-Checking")]
    public bool onGround;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.5f;
    public float bottomOffset = 0.7f;
    public float coyoteTime = 0.2f;

    [Header("Info")]
    [SerializeField] private float vertSpeed;
    [SerializeField] private float inputX;
    [SerializeField] private float inputY;
    [SerializeField] private float inputJ;
    [SerializeField] private float timeOnGround;

    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
    }

    void Update()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");
        inputJ = Input.GetAxis("Jump");

        if (inputJ > 0) {
            isJumping = true;   
        }        
    }

    void FixedUpdate() {
        //Check ground
        onGround = GroundCheck(-transform.up);
        //Starts downwards calculation
        DoGravity();

        //Initializes movement calculation
        Vector3 movement = new Vector3(inputX, 0, inputY);
        //Stops diagonals from being dumb
        movement.Normalize();
        //Applies speed
        movement.x *= xSpeed;
        movement.z *= ySpeed;

        //Starts jumping calculation
        if(isJumping)
            DoJumping();

        //Applies Vertical speed (gravity + jumping)
        movement.y += vertSpeed;

        //Applies movement by time
        controller.Move(movement * Time.deltaTime);
    }

    private void DoGravity() {
        //If on ground, coyote time is now valid and plz don't fall anymore
        if(onGround) {
            timeOnGround = coyoteTime;
            if(vertSpeed < 0)
                vertSpeed = 0f;
        }
        //Coyote time is not forever, y'know?
        if (timeOnGround > 0)
            timeOnGround -= Time.deltaTime;
        
        //Gravity 😎
        vertSpeed -= gravity * Time.deltaTime;
    }

    private void DoJumping() {
        isJumping = false;
        //If coyote time is valid
        if (timeOnGround > 0) {
            //Makes sure coyote time isn't valid anymore
            timeOnGround = 0;
            
            //Physics guy said so
            vertSpeed += Mathf.Sqrt(jumpForce * 2 * gravity);
        }
    }

    //Returns true if is on ground
    private bool GroundCheck(Vector3 direction)
    {
        //Initializes point to check ground 
        Vector3 pos = transform.position + (direction * bottomOffset);
        //Check objects with groundLayer within a certain raidus from that point
        Collider[] hitColliders = Physics.OverlapSphere(pos, groundCheckRadius, groundLayer);
        //If anything was found, then it's grounded
        if (hitColliders.Length > 0)
            return true;

        return false;
    }

    //Draws fancy stuff on scene viewer
    void OnDrawGizmosSelected()
    {
        //ground check sphere
        Gizmos.color = Color.yellow;
        Vector3 pos = transform.position + (-transform.up * bottomOffset);
        Gizmos.DrawWireSphere(pos, groundCheckRadius);
    }
}
