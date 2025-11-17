using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //UI do jogador
    public Image healthFill;

    public Image RageFill;
    public GameObject RagePanel;

    // Contador de kills
    public int killCount = 0;
    public GameObject killPopupPrefab;
    public Transform popupParent;
    public TextMeshProUGUI killText;

    // Panels de fim de jogo
    public GameObject gameOverPanel;
    public GameObject gameWonPanel;     
    public GameObject storyPanel;

    // Score final
    public TMPro.TextMeshProUGUI gameOverKillsText;
    public TMPro.TextMeshProUGUI gameOverTimeText;

    public TMPro.TextMeshProUGUI gameWonKillsText;
    public TMPro.TextMeshProUGUI gameWonTimeText;


    // Historia
    public TextMeshProUGUI storyText;
    public GameObject nextButton;
    public GameObject finishButton;

    [TextArea(3, 10)]
    public string[] paginasHistoria;

    private int historiaIndex = 0;


    // Win condition
    public Transform hordaParent;
    public bool hordaEliminada = false;
    public bool playerNaSafeZone = false;
    
    public InvisWall safeZoneWall;


    // Timer
    public float tempoMaximo = 300f;
    private float tempoRestante;
    public TextMeshProUGUI timerText;   

    private bool jogoVencido = false;
    private bool jogoAcabou = false;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        tempoRestante = tempoMaximo;

        if (RagePanel != null)
            RagePanel.SetActive(true);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (gameWonPanel != null)
            gameWonPanel.SetActive(false);

        if (storyPanel != null)
            storyPanel.SetActive(false);

        UpdateHealthBar(100, 100);
        UpdateRageBar(0, 100);

        UpdateKillUI();
        UpdateTimerUI();

    }

    private void Update()
    {
        if (jogoVencido || jogoAcabou) return;

        AtualizarTimer();

    }

    void AtualizarTimer()
    {
        tempoRestante -= Time.deltaTime;

        if (tempoRestante < 0)
        {
            tempoRestante = 0;
            GameOver();
        }

        UpdateTimerUI();
    }

    public void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int min = Mathf.FloorToInt(tempoRestante / 60);
            int sec = Mathf.FloorToInt(tempoRestante % 60);

            timerText.text = $"{min:00}:{sec:00}";
        }
    }

    string FormatTime(float seconds)
    {
        int min = Mathf.FloorToInt(seconds / 60);
        int sec = Mathf.FloorToInt(seconds % 60);
        return $"{min:00}:{sec:00}";
    }


    public void ShowKillPopup()
    {
        StartCoroutine(KillPopupRoutine());
    }

    private IEnumerator KillPopupRoutine()
    {
        GameObject popup = Instantiate(killPopupPrefab, popupParent);

        TextMeshProUGUI text = popup.GetComponent<TextMeshProUGUI>();
        Color c = text.color;

        Vector3 startPos = popup.transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0, 80, 0);

        float time = 0f;
        float duration = 0.6f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            popup.transform.localPosition = Vector3.Lerp(startPos, endPos, t);

            c.a = 1f - t;
            text.color = c;

            yield return null;
        }

        Destroy(popup);
    }

    public void AddKill()
    {
        killCount++;
        UpdateKillUI();
    }

    public void UpdateKillUI()
    {
        if (killText != null)
            killText.text = killCount.ToString();
    }

    public void UpdateHealthBar(float current, float max)
    {
        if (healthFill != null)
            healthFill.fillAmount = current / max;
    }

    public void UpdateRageBar(float current, float max)
    {
        if (RageFill != null)
            RageFill.fillAmount = current / max;
    }

    public void HideRageUI()
    {
        if (RagePanel != null)
            RagePanel.SetActive(false);
    }

    public void ShowRageUI()
    {
        if (RagePanel != null)
            RagePanel.SetActive(true);
    }


    public void PlayerEntrouSafeZone()
    {
        if (!hordaEliminada)
            return;

        playerNaSafeZone = true;

        MostrarHistoria();
    }


    void MostrarHistoria()
    {
        if (storyPanel != null)
            storyPanel.SetActive(true);

        Time.timeScale = 0;

        historiaIndex = 0;
        AtualizarPaginaHistoria();
    }

    public void ProximaPagina()
    {
        if (historiaIndex < paginasHistoria.Length - 1)
        {
            historiaIndex++;
            AtualizarPaginaHistoria();
        }
    }

    void AtualizarPaginaHistoria()
    {
        if (storyText != null)
            storyText.text = paginasHistoria[historiaIndex];

        // Botão próximo só aparece se não estiver na última página
        if (nextButton != null)
            nextButton.SetActive(historiaIndex < paginasHistoria.Length - 1);

        // Botão finalizar só aparece na última página
        if (finishButton != null)
            finishButton.SetActive(historiaIndex == paginasHistoria.Length - 1);
    }

    public void ContinuarDepoisHistoria()
    {
        if (storyPanel != null)
            storyPanel.SetActive(false);

        GameWon();
    }

    public void GameOver()
    {
        jogoAcabou = true;
        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (gameOverKillsText != null)
            gameOverKillsText.text = "KillCount: " + killCount;

        if (gameOverTimeText != null)
            gameOverTimeText.text = "Tempo restante: " + FormatTime(tempoRestante);
    }


    void GameWon()
    {
        jogoVencido = true;
        Time.timeScale = 0f;

        if (gameWonPanel != null)
            gameWonPanel.SetActive(true);

        if (gameWonKillsText != null)
            gameWonKillsText.text = "KillCount: " + killCount;

        if (gameWonTimeText != null)
            gameWonTimeText.text = "Tempo restante: " + FormatTime(tempoRestante);
    }

    public void VoltarMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
