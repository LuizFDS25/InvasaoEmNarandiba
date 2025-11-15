using System.Collections;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private Animator animator;

    public string[] idleDirections = { "N idle", "NW idle", "W idle", "SW idle", "S idle", "SE idle", "E idle", "NE idle" };
    public string[] walkDirections = { "N walk", "NW walk", "W walk", "SW walk", "S walk", "SE walk", "E walk", "NE walk" };
    public string[] attackDirections = { "N attack", "NW attack", "W attack", "SW attack", "S attack", "SE attack", "E attack", "NE attack" };
    public string[] deathDirections = { "N death", "NW death", "W death", "SW death", "S death", "SE death", "E death", "NE death" };

    private int lastDirection = 4;
    private bool isAttacking = false;

    private bool isDead = false;
    public bool IsDead => isDead;

    public bool IsAttacking => isAttacking;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Chamada pelo script Enemy.cs
    public void SetDirection(Vector2 direction)
    {
        if (isAttacking) return;

        string[] array;

        if (direction.sqrMagnitude < 0.01f)
        {
            array = idleDirections;
        }
        else
        {
            lastDirection = DirectionToIndex(direction);
            array = walkDirections;
        }

        animator.Play(array[lastDirection]);
    }

    public void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;

        animator.Play(attackDirections[lastDirection]);

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        yield return null;
        float duration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);

        isAttacking = false;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        animator.Play(deathDirections[lastDirection]);

        StartCoroutine(FreezeAndDestroy());
    }

    private IEnumerator FreezeAndDestroy()
    {
        float duration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);

        animator.speed = 0f;

        yield return new WaitForSeconds(5f);

        Destroy(gameObject);
    }

    private int DirectionToIndex(Vector2 dir)
    {
        Vector2 n = dir.normalized;

        float step = 360f / 8f;
        float offset = step / 2f;

        float angle = Vector2.SignedAngle(Vector2.up, n);
        angle += offset;

        if (angle < 0)
            angle += 360f;

        return Mathf.FloorToInt(angle / step);
    }
}
