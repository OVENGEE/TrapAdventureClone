using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public HealthUI healthUI;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector3 spawnPosition;

    private static int savedHealth;
    private static bool hasSavedHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!hasSavedHealth)
        {
            currentHealth = maxHealth;
            savedHealth = currentHealth;
            hasSavedHealth = true;
        }
        else
        {
            currentHealth = savedHealth;
        }

        healthUI.SetMaxHearts(maxHealth);
        healthUI.UpdateHearts(currentHealth);

        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        spawnPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Trap trap = collision.GetComponent<Trap>();
        if(trap && trap.damage > 0)
        {
            TakeDamage(trap.damage);
            ResetToSpawn();
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        savedHealth = currentHealth;
        healthUI.UpdateHearts(currentHealth);

        //flash red
        StartCoroutine(FlashRed());

        if(currentHealth <= 0)
        {
            //player dead! -- call game over, animation, etc
        }
    }

    private void ResetToSpawn()
    {
        if(rb)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        transform.position = spawnPosition;
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }
}
