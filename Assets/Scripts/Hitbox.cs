using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public int damage = 10;
    public float activeTime = 0.8f;
    public float distance = 0.3f;

    private void Awake()
    {
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            enemy.TakeDamage(damage);
        }
    }

    public void Activate(Vector2 facingDir)
    {
        gameObject.SetActive(true);

        
        float scaleFactor = Mathf.Max(transform.parent.localScale.x, transform.parent.localScale.y);
        transform.localPosition = facingDir.normalized * distance * scaleFactor;


        // Gira para a direção do ataque
        float angle = Mathf.Atan2(facingDir.y, facingDir.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, angle - 90);

        // Desativa após o tempo
        CancelInvoke();
        Invoke(nameof(Deactivate), activeTime);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
