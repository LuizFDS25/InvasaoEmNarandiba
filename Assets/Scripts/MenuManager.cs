using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject tutorialPanel;

    public void Start()
    {
        tutorialPanel.SetActive(false);
    }
    public void Jogar()
    {
        SceneManager.LoadScene("Game");
    }


    public void Sair()
    {

        Application.Quit();
    }

    public void Tutorial()
    {
        tutorialPanel.SetActive(true);
    }

    public void FecharTutorial()
    {
        tutorialPanel.SetActive(false);
    }
}
