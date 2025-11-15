using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public int health = 30;

    public Transform alvo;
    private PlayerHealth playerHealth;

    public float speed = 2f;
    public float distanciaminima = 1.2f;
    public int damage = 10;
    public float attackCooldown = 3f;

    public float detectionRadius = 5f;
    public bool playerDetected = false;

    private bool canAttack = true;

    private Rigidbody2D rb;
    private EnemyAnimation anim;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public float hitFlashDuration = 0.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;

        anim = GetComponent<EnemyAnimation>();

        spriteRenderer = GetComponent<SpriteRenderer>()
                         ?? GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        // pega vida do player
        if (alvo != null)
            playerHealth = alvo.GetComponent<PlayerHealth>();

        // cria o collider de detecção
        // cria um objeto filho para área de detecção
        GameObject detectObj = new GameObject("DetectionArea");
        detectObj.transform.parent = this.transform;
        detectObj.transform.localPosition = Vector3.zero;

        // adiciona collider de detecção no filho
        CircleCollider2D detection = detectObj.AddComponent<CircleCollider2D>();
        detection.isTrigger = true;
        detection.radius = detectionRadius;

        // adiciona script relay para chamar OnTriggerEnter/Exit no Enemy
        DetectionRelay relay = detectObj.AddComponent<DetectionRelay>();
        relay.enemy = this;

    }

    private void FixedUpdate()
    {
        if (alvo == null)
            return;

        if (!playerDetected)
        {
            // inimigo parado esperando o player entrar na área
            rb.velocity = Vector2.zero;
            anim.SetDirection(Vector2.zero);
            return;
        }

        Vector2 posicaoAlvo = alvo.position;
        Vector2 posicaoAtual = transform.position;

        float distancia = Vector2.Distance(posicaoAlvo, posicaoAtual);

        if (distancia >= distanciaminima)
        {
            Vector2 direction = (alvo.position - transform.position).normalized;

            rb.velocity = direction * speed;

            anim.SetDirection(rb.velocity);
        }
        else
        {
            rb.velocity = Vector2.zero;
            anim.SetDirection(rb.velocity);

            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (!canAttack || playerHealth == null)
            return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false;

        anim.Attack();

        yield return new WaitForSeconds(0.25f);

        playerHealth.TakeDamage(damage);

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        StartCoroutine(FlashRed());

        if (health <= 0 && !anim.IsDead)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
            rb.useFullKinematicContacts = true;

            rb.velocity = Vector2.zero;
            anim.Die();
            enabled = false;
        }
    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerDetected = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerDetected = false;
    }
}
