using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float maxGroundStamina = 100;
    public float curGroundStamina;
    public float grdStaRegen = .2f;

    Animator anim;

    public Vector3 direction;
    public float speed;

    public float minBothKeysSpeed;
    public float bothKeysDecelerationSpeed;
    SpriteRenderer sr;
    Rigidbody2D rb2d;
    float initSpeed;
    public float jumpForce;
    public float gravity;
    float initJumpForce;
    public float jumpCD;
    float initJumpCD;

    public bool isGrounded;

    [SerializeField]
    Transform groundCheck;
    [SerializeField]
    Transform groundCheckL;
    [SerializeField]
    Transform groundCheckR;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        
        initSpeed = speed;
        initJumpForce = jumpForce;
        curGroundStamina = maxGroundStamina;
        initJumpCD = jumpCD;
    }

    private void FixedUpdate()
    {
        Raycasting();
        PlayerMovement();
        Jump();
        StaminaUpdate();
        
    }

    void StaminaUpdate()
    {
        if (curGroundStamina < 0)
        {
            curGroundStamina = 0;
        }
        if (curGroundStamina < maxGroundStamina)
        {
            curGroundStamina += 1f*grdStaRegen;
        }
        else
        {
            curGroundStamina = maxGroundStamina;
        }

    }

    void PlayerMovement()
    {
        this.direction.x = Input.GetAxis("Horizontal");
        rb2d.velocity = new Vector2(speed*direction.x, rb2d.velocity.y);

        if (this.direction.x < 0) { sr.flipX = enabled; }
        if (this.direction.x > 0) { sr.flipX = !enabled; }
       
        if (direction.x < 0)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                if (speed > minBothKeysSpeed)
                {
                    speed -= bothKeysDecelerationSpeed;
                }
            }
            else
            {
                speed = initSpeed;
            }
        }
        if (direction.x > 0)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (speed > minBothKeysSpeed)
                {
                    speed -= bothKeysDecelerationSpeed;
                }
            }
            else
            {
                speed = initSpeed;
            }
        }

    }
    void Jump()
    {
        float tempJump;
        tempJump = initJumpForce * 1.1f;
        if (Input.GetButton("Jump") && isGrounded && jumpCD == 0)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);

            curGroundStamina -= 10;
            jumpCD = initJumpCD;
        }
        if (curGroundStamina == maxGroundStamina)
        {
            jumpForce = tempJump;
        }
        else
        {
            jumpForce = initJumpForce;
        }

        //Cooldown
        if (jumpCD > 0)
        {
            jumpCD -= Time.deltaTime;
        }
        if (jumpCD <= 0)
        {
            jumpCD = 0;
        }


        // Better Gravity for Jumping
        if (rb2d.velocity.y < 0)
        {
            rb2d.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb2d.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb2d.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void Raycasting()
    {
        if (
            Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Platforms")) ||
            Physics2D.Linecast(transform.position, groundCheckL.position, 1 << LayerMask.NameToLayer("Platforms")) ||
            Physics2D.Linecast(transform.position, groundCheckR.position, 1 << LayerMask.NameToLayer("Platforms"))
            )
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
