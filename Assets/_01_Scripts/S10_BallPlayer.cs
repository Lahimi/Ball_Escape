using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class S10_BallPlayer : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2     moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Top-down — no gravity, no rotation
        rb.gravityScale = 0f;
        rb.constraints  = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        // Read input every frame
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Normalize so diagonal movement isn't faster
        moveInput = moveInput.normalized;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
