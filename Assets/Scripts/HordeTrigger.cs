using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HordeZoneTrigger : MonoBehaviour
{
    public string enemyTag = "Enemy";
    public InvisWall parede;
    public TextMeshProUGUI hordeText;
    public string mensagemCompleta = "Horda Aniquilada, Entre na zona segura";

    private HashSet<GameObject> inimigosDentro = new HashSet<GameObject>();
    private bool playerDentro = false;

    void Start()
    {
        if (hordeText != null)
            hordeText.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(enemyTag))
        {
            inimigosDentro.Add(other.gameObject);
            AtualizarTexto();
        }

        if (other.CompareTag("Player"))
        {
            playerDentro = true;
            if (hordeText != null)
                hordeText.gameObject.SetActive(true);

            AtualizarTexto();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(enemyTag))
        {
            inimigosDentro.Remove(other.gameObject);
            AtualizarTexto();
            VerificarSeHordaAcabou();
        }

        if (other.CompareTag("Player"))
        {
            playerDentro = false;
            if (hordeText != null)
                hordeText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        
        inimigosDentro.RemoveWhere(i => i == null);
        AtualizarTexto();

        VerificarSeHordaAcabou();
    }

    void AtualizarTexto()
    {
        if (!playerDentro || hordeText == null)
            return;

        if (inimigosDentro.Count > 0)
        {
            hordeText.text = $"Inimigos restantes: {inimigosDentro.Count}";
        }
        else
        {
            hordeText.text = mensagemCompleta;
        }
    }

    void VerificarSeHordaAcabou()
    {
        if (inimigosDentro.Count == 0)
        {
            if (parede != null)
                parede.DesativarParede();

            AtualizarTexto();
        }
    }
}
