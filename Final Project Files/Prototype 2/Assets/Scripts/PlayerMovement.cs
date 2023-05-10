using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public GameObject wall;
    private Vector3 lastPos, wallNormal;
    [Header("Movement variables")]
    public float wallTime = 0.1f;
    public float wallCounter;
    public float jumpSpeed = 15;
    public float airSpeed = 15;
    public float defaultSpeed = 1;
    private float currentSpeed = 45;
    public float slideSpeed = 30;
    public bool isDashing = false;
    public bool wallJumping = false;
    public float coolDown = 1;
    public float dashSpeed = 30;
    public float dashTime = 0.5f;
    public Camera camLook;
    public float gravity = -9.81f;
    [Header("Jumping Checks and Collision variables")]
    public int maxJumps = 2;
    public Transform groundCheck, wallCheck;
    public float groundRadius = 0.4f;
    public float jumpHeight = 3.0f;
    public LayerMask groundMask, wallMask;
    public bool isGrounded = true;
    public float wallRadius = 1.35f;
    public bool onWall, keyPressed;
    public bool isAvailable = true;
    public bool isSliding = false;
    private Vector3 slideForward, move, velocity, dashForward, prevVelocity, groundedVelocity;
    float xMove, zMove, originalHeight, sloperForce;
    public float slopeForceLength;
    // Start is called before the first frame update

    // Update is called once per frame

    private void Start()
    {
        sloperForce = 6;
        lastPos = controller.transform.position;
    }

    void Update()
    {
        //Projects an invisible sphere beneath the player that checks whether or not they are on the ground.
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);
        onWall = Physics.CheckSphere(wallCheck.position, wallRadius, wallMask);
        xMove = Input.GetAxisRaw("Horizontal");
        zMove = Input.GetAxisRaw("Vertical");
        move = transform.right * xMove + transform.forward * zMove;
        
        if (isGrounded)
        {
            groundedVelocity = Vector3.ProjectOnPlane(prevVelocity, Vector3.up);
            maxJumps = 2;
            keyPressed = false;
        }

        if ((xMove != 0 || zMove != 0) && Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && !isSliding && !onWall && !keyPressed)
        {
            isSliding = true;
            originalHeight = controller.height;
            controller.height -= 2;
            slideForward = (controller.transform.position - lastPos).normalized;
            camLook.transform.position = new Vector3(camLook.transform.position.x, camLook.transform.position.y - 1, camLook.transform.position.z);
        }

        if (isSliding)
        {
            move = slideForward;
            defaultSpeed = slideSpeed;
        }

        //Constantly updates the player's last position so that we can lock the direction of their slide.
        lastPos = controller.transform.position;

        //Stops sliding.
        if (isSliding && (Input.GetKeyUp(KeyCode.LeftControl) || onWall || !isGrounded || Input.GetKey(KeyCode.Space)))
        {
            isSliding = false;
            keyPressed = false;
            controller.height = originalHeight;
            camLook.transform.position = new Vector3(camLook.transform.position.x, camLook.transform.position.y + 1, camLook.transform.position.z);
            defaultSpeed = currentSpeed;
        }
        //Snaps player to the ground more readily than setting their y velocity to zero would be.
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -12f;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && isAvailable)
        {
            isDashing = true;
            dashForward = (controller.transform.position - lastPos).normalized;
            StartCoroutine(Dash());
        }

        //If statement used to prevent jump+slide tech.
        /*if (!isGrounded)
        {
            keyPressed = false;
        }*/

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // V = square root(h * -2g)
            keyPressed = true;
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            //move.y = velocity.y;
        }

        if (Input.GetButtonDown("Jump") && onWall && !isGrounded && maxJumps > 0)
        {
            maxJumps--;
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            //wallJump = new Vector3(wallNormal.x * defaultSpeed, 0, wallNormal.z * defaultSpeed);
            //groundedVelocity = Vector3.Reflect(groundedVelocity, wallNormal);
            Debug.Log(groundedVelocity);
            controller.Move(wallNormal * defaultSpeed/2);
            controller.Move(velocity * Time.deltaTime);
            // transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(move.x, velocity.y, move.z), Time.deltaTime * defaultSpeed);
            //StartCoroutine(WallJump());
        }

        //Stores the force of gravity into a float and applies it to the character
        velocity.y += gravity * Time.deltaTime;
        //Delta time is multipled by twice due to real life physics. Displacement equation = 1/2(g) * t^2

        //Gets input from wasd and converts it into movement in game. If statement allows users on controller to controller whether they "walk" or "run".
        //if (move.magnitude > 1)
        //{
        move.Normalize();
        //}

        if ((xMove != 0 || zMove != 0) && OnSlope())
        {
            sloperForce = 5;
            Debug.Log("Sloped");
            controller.Move(Vector3.down * controller.height / 2 * sloperForce * Time.deltaTime);
        }
        //controller.Move(move * defaultSpeed * Time.deltaTime);
        if (!isDashing)
        {
            controller.Move(velocity * jumpSpeed * Time.deltaTime);
        }
        controller.Move(move * defaultSpeed * Time.deltaTime);
        prevVelocity = move / Time.deltaTime;
    }

    IEnumerator Dash()
    {
        if (!isAvailable)
        {
            Debug.Log("Unavailable");
            yield return null;
        }
        defaultSpeed = dashSpeed;
        move = dashForward;
        StartCoroutine(CoolDown());
        yield return new WaitForSeconds(0.5f);

        isDashing = false;
        defaultSpeed = currentSpeed;
        
    }

    IEnumerator CoolDown()
    {
        isAvailable = false;
        Debug.Log("On cooldown");
        yield return new WaitForSeconds(coolDown);
        isAvailable = true;
        Debug.Log("Available again");
    }

    IEnumerator WallJump()
    {
        //defaultSpeed = -defaultSpeed;
        /*velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        yield return new WaitForSeconds(0.2f);
        defaultSpeed = 0;
        xMove = 0;
        zMove = 0;
        yield return new WaitForSeconds(0.2f);
        defaultSpeed = currentSpeed;*/
        //velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        //defaultSpeed = (Mathf.Sqrt(move.x * move.x + move.z * move.z));
        //Vector3 resultant = new Vector3(Mathf.Sqrt(move.x * move.x), 0, Mathf.Sqrt(move.z * move.z));
        //controller.Move(move * defaultSpeed * Time.deltaTime);
        //  yield return new WaitForSeconds(1f);
        move = wallNormal * defaultSpeed;
        controller.Move(velocity * jumpHeight * Time.deltaTime);
        controller.Move(move * defaultSpeed * Time.deltaTime);
        yield return new WaitForSeconds(2f);
    }

    private bool OnSlope()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, controller.height/2 * slopeForceLength))
        {
            if (hit.normal != Vector3.up)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    keyPressed = false;
                }
                return true;
            }
        }

        return false;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Wall"))
        {
            if (isGrounded) return;
            else if (Input.GetButton("Jump"))
            {
                //controller.attachedRigidbody.AddForce(hit.normal, ForceMode.Impulse);
                /* move = lastPos;
                 move = hit.normal;*/
                wallNormal = hit.normal;
                //transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, velocity.y, 0), defaultSpeed * Time.deltaTime);
            }
        }
    }
}
