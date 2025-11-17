using UnityEngine;
using System.Collections;

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
    private BoxCollider2D boxCollider;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public float hitFlashDuration = 0.5f;

    public GameObject hitParticlesPrefab;

    private bool isVisible = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;

        anim = GetComponent<EnemyAnimation>();

        spriteRenderer = GetComponent<SpriteRenderer>()
                         ?? GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (alvo != null)
            playerHealth = alvo.GetComponent<PlayerHealth>();

        // cria o collider de detecção
        GameObject detectObj = new GameObject("DetectionArea");
        detectObj.transform.parent = this.transform;
        detectObj.transform.localPosition = Vector3.zero;

        CircleCollider2D detection = detectObj.AddComponent<CircleCollider2D>();
        detection.isTrigger = true;
        detection.radius = detectionRadius;

        DetectionRelay relay = detectObj.AddComponent<DetectionRelay>();
        relay.enemy = this;
    }

    void Start()
    {
        if (alvo == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                alvo = p.transform;
                playerHealth = p.GetComponent<PlayerHealth>();
            }
        }
        else
        {
            playerHealth = alvo.GetComponent<PlayerHealth>();
        }
    }


    void Update()
    {
        CheckVisibility();
    }

    private void FixedUpdate()
    {
        if (!isVisible)
        {
            rb.velocity = Vector2.zero;
            anim.SetDirection(Vector2.zero);
            return;
        }

        if (alvo == null)
            return;

        if (!playerDetected)
        {
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
    private void CheckVisibility()
    {
        Vector3 p = Camera.main.WorldToViewportPoint(transform.position);

        bool visible =
            p.x > 0 && p.x < 1 &&
            p.y > 0 && p.y < 1 &&
            p.z > 0;

        isVisible = visible;
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
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = Color.red;
        Instantiate(hitParticlesPrefab, transform.position, Quaternion.identity);
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
