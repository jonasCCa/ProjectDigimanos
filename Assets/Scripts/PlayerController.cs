using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    [Header("References")]
    public StatsController stats;
    public InventoryController inventory;
    public CharacterController controller;
    public PlayerInput playerInput;
    Collider selfCollider;
    public List<Collider> itemsOnGround;

    [Header("Movement Control")]
    public float xSpeed = 10;
    public float ySpeed = 10;
    public float breakXSpeed;
    public float breakYSpeed;
    public float breakTime = 0.2f;
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

    [Header("Attack & Defense")]
    public WeaponController weapon;
    public Animator animator;
    [SerializeField] private bool isAttacking;
    public bool isBlocking;
    
    //   Place-holder for blocking effect
        [SerializeField] private GameObject block;
        [SerializeField] private Material blockBase;
        [SerializeField] private Material blockEffect;

    [Header("Knockback")]
    [SerializeField] private bool isKnockbacked;
    //[SerializeField] private float lastKnockbacked;
    [SerializeField] private float knockbackValue;
    [SerializeField] private Vector3 knockbackDirection;
    // Place-holder for knockback animation
    [SerializeField] private float lasEulerX;

    [Header("Info")]
    [SerializeField] private Vector3 movement;
    [SerializeField] private float vertSpeed;
    [SerializeField] private float inputX;
    [SerializeField] private float inputY;
    [SerializeField] private bool inputJ;
    [SerializeField] private float timeOnGround;
    [SerializeField] private float timeSinceGrounded;

    [Header("UI")]
    public GameObject indicator;
    public GameObject playerMenu;

    //Debugging purposes
    //[SerializeField] private Vector3 movement;

    void Start() {
        itemsOnGround = new List<Collider>();

        // Place-holder for equipment system
        weapon = GetComponentInChildren<WeaponController>();

        stats = GetComponent<StatsController>();
        inventory = GetComponent<InventoryController>();
        
        selfCollider = GetComponent<Collider>();
        playerInput = GetComponent<PlayerInput>();
        //playerInput.SwitchCurrentActionMap("Gameplay");

        GetComponent<UIPlayerController>().scrollTransform = playerMenu.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        GetComponent<UIPlayerController>().invSubMenuTransform = playerMenu.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<RectTransform>();
        inventory.listParentUI = GetComponent<UIPlayerController>().scrollTransform.gameObject;

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
            else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Block"))
                isBlocking = true;
            else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
                isAttacking = false;
            }
        } else {
            isBlocking = false;
        }


        if(isBlocking) {
            block.SetActive(true);
            if(isKnockbacked)
                block.GetComponent<MeshRenderer>().material = blockEffect;
        } else {
            block.SetActive(false);
            block.GetComponent<MeshRenderer>().material = blockBase;
        }
    }

    void FixedUpdate() {
        if(stats.isAlive) {
            // Checks ground + player
            onGround = GroundCheck(-transform.up);
            // Starts downwards calculation
            DoGravity();

            // If player is not recieving knockback, continue as normal
            if(!isKnockbacked) {
                
                // Can't move while attacking nor blocking
                if(!isAttacking && !isBlocking) {
                    // Initializes movement calculation
                    movement = new Vector3(inputX, 0, inputY);
                    breakXSpeed = inputX;
                    breakYSpeed = inputY;
                } else { // Slows down movement when attacking
                    //movement = new Vector3(0,0,0);
                    breakXSpeed = Mathf.Lerp(breakXSpeed,0,breakTime);
                    breakYSpeed = Mathf.Lerp(breakYSpeed,0,breakTime);
                    movement = new Vector3(breakXSpeed,0,breakYSpeed);
                }

                // Stops diagonals from being dumb
                movement = Vector3.ClampMagnitude(movement, 1f);
                // Applies speed
                movement.x *= xSpeed;
                movement.z *= ySpeed;

                // Checks PoT
                hasPoT = PoTCheck(transform.up);

                // Can't jump while attacking
                if(!isAttacking) {
                    // Starts jumping calculation
                    if(inputJ && !hasPoT) {
                        DoJumping();
                    }
                }

                // Rotate to where it's moving
                if(movement.magnitude != 0) {
                    float step = turningSpeed * Time.deltaTime;
                    transform.rotation = Quaternion.RotateTowards(transform.rotation,  Quaternion.LookRotation(movement), step);
                }

                // Applies Vertical speed (gravity + jumping)
                movement.y += vertSpeed;
            } else { // If player is recieving knockback
                // Apply knockback
                movement = knockbackDirection * 5*knockbackValue;
                // Apply gravity
                movement.y += vertSpeed;

                // Decreases knockback by time
                knockbackValue -= 20*Time.deltaTime;
                // If knockback is less than 0, stop knockbacking
                if(knockbackValue <= 0) {
                    transform.eulerAngles = new Vector3(lasEulerX,transform.eulerAngles.y,transform.eulerAngles.z);
                    isKnockbacked = false;
                }
            }       

            // Applies movement by time
            controller.Move(movement * Time.deltaTime);
        }
    }

    private void DoGravity() {
        // If on ground, coyote time is now valid and plz don't fall anymore
        if(onGround) {
            // Resets coyote time
            timeOnGround = coyoteTime;

            // Tracks if player's been a minimum time on ground before jumping again
            if(timeSinceGrounded<minGroundTime)
                timeSinceGrounded += Time.deltaTime;
            else
                isJumping = false;

            // Makes sure you don't accelerate donwards while on ground
            if(vertSpeed < 0)
                vertSpeed = 0f;
        } else {
            // If player is going up and hits something
            if(vertSpeed > 0 && CeilingCheck(transform.up))
                vertSpeed = 0;
        }
        // Coyote time is not forever, y'know?
        if (timeOnGround > 0)
            timeOnGround -= Time.deltaTime;
        
        // Gravity 😎
        vertSpeed -= gravity * Time.deltaTime;
    }

    private void DoJumping() {
        if(!isJumping) {
            isJumping = true;
            timeSinceGrounded = 0;
            // If coyote time is valid
            if (timeOnGround > 0) {
                //Makes sure coyote time isn't valid anymore
                timeOnGround = 0;
                
                // Stop falling (in order to do the coyote time magic)
                vertSpeed = 0;
                // Physics guy said so
                vertSpeed += Mathf.Sqrt(jumpForce * 2 * gravity);
            }
        }
    }

    // Recieves knockback from Stats Controller
    public void Knockback(int kbValue, Vector3 attacker) {
        isKnockbacked = true;

        // Face attacker
        transform.LookAt(attacker);

        if(!isBlocking) {
            // Place-holder for knockback animation
            lasEulerX = transform.eulerAngles.x;
            transform.eulerAngles = new Vector3(-20,transform.eulerAngles.y,transform.eulerAngles.z);

            // To avoid flying
            isJumping = true;
            
            knockbackValue = kbValue;
        } else {
            // If it's blocking, reset blocking animation and recieve half of knockback force
            animator.Play("Block", 0, 0.0526315f);
            knockbackValue = kbValue / 2f;
        }
        
        // Calculate direction that the knockback was recieved
        knockbackDirection = transform.position - attacker;
        // Negate Y-axis knockback
        knockbackDirection.y = 0;
        // Normalize direction vector
        knockbackDirection /= knockbackDirection.magnitude;
    }

    //Debugging purposes
    //[Header("Collision Debugging")]
    List<Collider> hitColliders;

    // Returns true if is on ground or in top of another player
    private bool GroundCheck(Vector3 direction)
    {
        // Initializes point to check ground 
        Vector3 pos = transform.position + (direction * bottomOffset);
        // Check objects with groundLayer within a certain raidus from that point
        hitColliders = Physics.OverlapSphere(pos, groundCheckRadius, groundLayer).ToList();

        // Removes self from list
        hitColliders.Remove(selfCollider);

        // If anything was found, then it's grounded
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

    // Returns true if hits ceiling
    private bool CeilingCheck(Vector3 direction)
    {
        // Initializes point to check ceiling 
        Vector3 pos = transform.position + (direction * bottomOffset);
        // Check objects within a certain raidus from that point
        hitColliders = Physics.OverlapSphere(pos, groundCheckRadius, groundLayer).ToList();


        hitColliders.Remove(selfCollider);

        // If anything (besides self) was found, then player's head goes bonk
        if (hitColliders.Count > 0)
            return true;

        return false;
    }

    // Returns true if there's a player on top
    private bool PoTCheck(Vector3 direction) {
        // Initializes point to check PoT 
        Vector3 pos = transform.position + (direction * topOffset);
        //Check objects with playerLayer within a certain raidus from that point
        //List<Collider> hitColliders = Physics.OverlapSphere(pos, potCheckBoxSize, playerLayer).ToList();
        //List<Collider> hitColliders = Physics.OverlapBox(pos, potCheckBoxSize, Quaternion.identity, playerLayer);

        //Debugging purposes
        hitColliders = Physics.OverlapBox(pos, potCheckBoxSize, Quaternion.identity, playerLayer).ToList();
        //hitColliders = Physics.OverlapSphere(pos, potCheckBoxSize, playerLayer).ToList();

        // Delete Self from hitColliders
        //Collider selfCollider = GetComponent<Collider>();
        //if(hitColliders.Contains(selfCollider)) {
            hitColliders.Remove(selfCollider);
        //}

        // If anything was found, then it has PoT
        if (hitColliders.Count > 0)
            return true;

        return false;
    }

    // Stores reference to Item On Ground
    void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == 20) {
            if(!itemsOnGround.Contains(other)) {
                itemsOnGround.Add(other);
            }
            //itemOnGround = other;
        }
    }

    // Removes reference to Item on Ground, if it's on the list
    void OnTriggerExit(Collider other) {
        if(other.gameObject.layer == 20) {
            if(itemsOnGround.Contains(other)) {
                itemsOnGround.Remove(other);
                //itemOnGround = null;
            }
        }
    }


    ////////////////////////
    //  New Input System  //
    //          -         //
    // Event Callbacks on //
    //   <Player Input>   //
    ////////////////////////
    public void OnMove(InputAction.CallbackContext value) {
        // Can't move while attacking
        inputX = value.ReadValue<Vector2>().x;
        inputY = value.ReadValue<Vector2>().y;
    }

    public void OnJump(InputAction.CallbackContext value) {
        // CallbackContext are Finite-State Machines:
        // started -> performed -> canceled

        if(value.performed) {
            inputJ = value.ReadValueAsButton();
        }
    }

    public void OnPickup(InputAction.CallbackContext value) {
        if(value.performed && value.ReadValueAsButton()==true) {
            if(itemsOnGround.Count > 0 && stats.isAlive) {
                // Mitigate nulls
                while(itemsOnGround.Count > 0) {
                    if(itemsOnGround[0] != null)
                        break;
                    
                    itemsOnGround.RemoveAt(0);
                }

                if(itemsOnGround.Count > 0) {
                    ItemContainer auxContainer = itemsOnGround[0].gameObject.GetComponent<ItemContainer>();

                    switch(auxContainer.itemType) {
                        case ItemContainer.ItemType.Usable:
                            // Adds item to inventory and gets leftovers
                            int leftovers = inventory.AddItem(auxContainer.uItem);
                            if(leftovers != 0) {
                                auxContainer.uItem.RemoveQuantity(auxContainer.uItem.quantity - leftovers);
                            } else {
                                itemsOnGround.Remove(itemsOnGround[0]);
                                Destroy(auxContainer.gameObject);
                            }
                            break;
                        case ItemContainer.ItemType.Equipable:
                            if(inventory.AddItem(auxContainer.eItem) == 0) {
                                itemsOnGround.Remove(itemsOnGround[0]);
                                Destroy(auxContainer.gameObject);
                            }
                            break;
                    }
                }
            }
        }
    }

    public void OnLightAttack(InputAction.CallbackContext value) {
        // Can only attack when isn't recieving knockback and isn't dead
        if(!isKnockbacked && stats != null && stats.isAlive) {
            // Can only attack when is not attacking already nor blocking (+button checks)
            if(value.performed && value.ReadValueAsButton()==true && !isAttacking && !isBlocking) {
                // Send attack information to weapon
                weapon.SetAttackType("Light_Attack");
                // Play attacking animation
                if(animator.isActiveAndEnabled)
                    animator.Play("Light_Attack");
            }
        }
    }

    public void OnHeavyAttack(InputAction.CallbackContext value) {
        // Can only attack when isn't recieving knockback
        if(!isKnockbacked && stats != null && stats.isAlive) {
            // Can only attack when is not attacking already nor blocking (+button checks)
            if(value.performed && value.ReadValueAsButton()==true && !isAttacking && !isBlocking) {
                // Send attack information to weapon
                weapon.SetAttackType("Heavy_Attack");
                // Play attacking animation
                if(animator.isActiveAndEnabled)
                    animator.Play("Heavy_Attack");
            }
        }
    }

    public void OnDenfend(InputAction.CallbackContext value) {
        // Can only block when isn't recieving knockback
        if(!isKnockbacked && stats != null && stats.isAlive) {
            // Can only block when is not attacking nor blocking (+button checks)
            if(value.performed && value.ReadValueAsButton()==true && !isAttacking && !isBlocking) {
                // Play blocking animation
                if(animator.isActiveAndEnabled)
                    animator.Play("Block");
            }
        }
    }

    public void OnShortcut1(InputAction.CallbackContext value) {
        if(value.performed && value.ReadValueAsButton()==true && inventory != null) {
            // PLACE-HOLDER
            inventory.UseItemByIndex(inventory.shortcut1);
        }
    }

    public void OnShortcut2(InputAction.CallbackContext value) {
        if(value.performed && value.ReadValueAsButton()==true && inventory != null) {
            // PLACE-HOLDER
            inventory.UseItemByIndex(inventory.shortcut2);
        }
    }





    public void OnOpenMenu(InputAction.CallbackContext value) {
        if(playerInput != null) {
            if(value.ReadValueAsButton()==true && value.performed) {
                playerMenu.SetActive(true);
                playerInput.SwitchCurrentActionMap("UI");
            }
        }
    }


    // Draws fancy stuff on editor scene viewer
    void OnDrawGizmosSelected()
    {
        // Ground check sphere
        Gizmos.color = Color.yellow;
        Vector3 pos = transform.position + (-transform.up * bottomOffset);
        Gizmos.DrawWireSphere(pos, groundCheckRadius);

        // Ground player doublecheck box
        Gizmos.color = new Color(1, 0.647f, 0, 1); //orange
        pos = transform.position + (-transform.up * topOffset);
        Gizmos.DrawWireCube(pos, potCheckBoxSize*2);

        // Ceiling check sphere
        Gizmos.color = Color.yellow;
        pos = transform.position + (transform.up * bottomOffset);
        Gizmos.DrawWireSphere(pos, groundCheckRadius);
        
        // PoT check sphere
        Gizmos.color = new Color(1, 0.647f, 0, 1); //orange
        pos = transform.position + (transform.up * topOffset);
        Gizmos.DrawWireCube(pos, potCheckBoxSize*2);
    }
}
