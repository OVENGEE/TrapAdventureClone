using UnityEngine;

public class Trap : MonoBehaviour
{
    public float bounceForce = 10f;
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            AudioFeedback.PlayTrapHit();
            HandlePlayerBounce(collision.gameObject);
        }
    }

    private void HandlePlayerBounce(GameObject player)
    {
        Rigidbody2D rb  = player.GetComponent<Rigidbody2D>();

        if(rb)
        {
            //reset player velocity
            rb.linearVelocity = new Vector2(rb.linearVelocity.x , 0f);

            //apply bounce force
            rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
        }
    }

}
