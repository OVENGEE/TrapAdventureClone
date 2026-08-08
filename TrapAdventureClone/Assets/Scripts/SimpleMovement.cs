using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public float speed;
    private float Move;

    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
       Move = Input.GetAxis("Horizontal");

       rb.linearVelocity = new Vector2(Move * speed, rb.linearVelocity.y);
    }
}


