using UnityEngine;
using System.Collections;

public class RageSystem : MonoBehaviour
{
    
    public int maxRage = 100;
    public int currentRage = 0;
    public int ragePerHit = 20;

    public float transformationDuration = 10f;

    public RuntimeAnimatorController normalController;
    public RuntimeAnimatorController superController;

    private PlayerAnimation anim;
    private Hitbox hitbox;
    private PlayerMovement movement;

    private bool isTransformed = false;

    void Awake()
    {
        anim = GetComponent<PlayerAnimation>();
        hitbox = GetComponentInChildren<Hitbox>();
        movement = GetComponent<PlayerMovement>();
    }

    public void AddRage(int amount)
    {
        if (isTransformed) return;

        currentRage += amount;
        currentRage = Mathf.Clamp(currentRage, 0, maxRage);

        GameManager.instance?.UpdateRageBar(currentRage, maxRage);

        if (currentRage >= maxRage)
            StartTransformation();
    }

    private void StartTransformation()
    {
        if (isTransformed) return;
        isTransformed = true;

        hitbox.damage *= 3;

        // troca animador
        StartCoroutine(SwapAnimatorNextFrame());

        // aumenta velocidade
        movement.moveSpeed /= 1.3f;

        // aumenta tamanho
        transform.localScale *= 1.3f;

        // barra some
        GameManager.instance?.HideRageUI();

        Invoke(nameof(EndTransformation), transformationDuration);
    }

    private IEnumerator SwapAnimatorNextFrame()
    {
        yield return null;

        anim.SwitchAnimatorController(superController);
    }


    private void EndTransformation()
    {
        // volta ao normal
        anim.SwitchAnimatorController(normalController);

        hitbox.damage /= 3;
        movement.moveSpeed *= 1.3f;
        transform.localScale /= 1.3f;

        currentRage = 0;
        isTransformed = false;

        GameManager.instance?.ShowRageUI();
        GameManager.instance?.UpdateRageBar(0, maxRage);
    }
}
