using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    
    [Header("References")]
    public CharacterController controller;
    public GameObject playerCamera;
    private PhotonView PV;

    [Header("Movement Control")]
    public float xSpeed = 10;
    public float ySpeed = 10;
    
    [Header("Jump Control")]
    public bool isJumping;
    public float jumpForce = 5;
    public float gravity = 90;
    public float coyoteTime = 0.15f;
    public float minGroundTime = 0.05f;
    
    [Header("Ground-Checking")]
    public bool onGround;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.5f;
    public float bottomOffset = 0.7f;

    [Header("Info")]
    [SerializeField] private float vertSpeed;
    [SerializeField] private float inputX;
    [SerializeField] private float inputY;
    [SerializeField] private float inputJ;
    [SerializeField] private float timeOnGround;
    [SerializeField] private float timeSinceGrounded;
    [SerializeField] private Vector3 movement;

    void Start() {
        PV = GetComponent<PhotonView>();

        controller = GetComponent<CharacterController>();
        playerCamera = GameObject.Find("MainCamera");

        playerCamera.transform.rotation = Quaternion.identity;
        playerCamera.transform.Rotate(20,0,0);

        isJumping = true;
    }

    void Update() {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        inputJ = Input.GetAxis("Jump");     
    }

    void FixedUpdate() {
        if(PV.IsMine) {
            //Check ground
            onGround = GroundCheck(-transform.up);
            //Starts downwards calculation
            DoGravity();

            //Initializes movement calculation
            //Vector3 movement = new Vector3(inputX, 0, inputY);
            movement = new Vector3(inputX, 0, inputY);
            //Stops diagonals from being dumb
            //movement.Normalize();
            movement = Vector3.ClampMagnitude(movement, 1f);
            //Applies speed
            movement.x *= xSpeed;
            movement.z *= ySpeed;

            //Starts jumping calculation
            if(inputJ > 0) {
                DoJumping();
            }

            //Applies Vertical speed (gravity + jumping)
            movement.y += vertSpeed;

            //Applies movement by time
            controller.Move(movement * Time.deltaTime);

            // WIP: COMPLETE CHANGE WILL BE DONE LATER
            MoveCamera();
        }
    }

    // WIP: COMPLETE CHANGE WILL BE DONE LATER
    private void MoveCamera() {
        playerCamera.transform.position = new Vector3(transform.position.x,
                                                      transform.position.y + 4.1f,
                                                      transform.position.z - 8.52f);
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
