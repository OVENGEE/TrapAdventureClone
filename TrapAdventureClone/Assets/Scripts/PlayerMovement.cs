using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;
    
    [Header("Jumping")]
    public float jumpPower = 10f;
    
    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    void Update()
    {
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y); 
        Gravity();
    }

    private void Gravity()
    {
        if(rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier; //fall increasingly faster
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(isGrounded())
        {
            if(context.performed)
            {
                //hold down jump button full height
                rb.linearVelocity = new Vector2(rb.linearVelocity.x , jumpPower);
                AudioFeedback.PlayJump();
            }
            else if(context.canceled)
            {
                //light tap of jump button = half the height
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
        }
    }

    private bool isGrounded()
    {
        float extraHeight = 0.05f;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(groundCheckPos.position, groundCheckSize, 0f, Vector2.down, extraHeight, groundLayer);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.normal.y > 0.9f)
            {
                return true;
            }
        }

        return false;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position , groundCheckSize);
    }
}
