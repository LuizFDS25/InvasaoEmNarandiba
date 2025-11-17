using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    private Rigidbody2D rb;
    private PlayerAnimation playerAnim;
    private PlayerMovement playerMove;

    private SpriteRenderer spriteRenderer;
    public float hitFlashDuration = 0.1f;

    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<PlayerAnimation>();
        playerMove = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        GameManager.instance?.UpdateHealthBar(currentHealth, maxHealth);
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;
        currentHealth = Mathf.Max(0, currentHealth);

        StartCoroutine(FlashRed());
        UpdateHUD();

        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = Color.white;
    }

    private void Die()
    {
        isDead = true;

        rb.velocity = Vector2.zero;

        if (playerAnim != null) playerAnim.Die();
        if (playerMove != null) playerMove.enabled = false;
    }
}
