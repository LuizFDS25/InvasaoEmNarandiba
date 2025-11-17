using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Image healthFill;

    public Image RageFill;
    public GameObject RagePanel;

    public int killCount = 0;
    public GameObject killPopupPrefab;
    public Transform popupParent;


    public TMPro.TextMeshProUGUI killText;


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

        if (RagePanel != null)
            RagePanel.SetActive(true);
        
        UpdateHealthBar(100, 100);
        UpdateRageBar(0, 100);

    }

    public void ShowKillPopup()
    {
        StartCoroutine(KillPopupRoutine());
    }

    private IEnumerator KillPopupRoutine()
    {
        GameObject popup = Instantiate(killPopupPrefab, popupParent);

        TMPro.TextMeshProUGUI text = popup.GetComponent<TMPro.TextMeshProUGUI>();
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
    public void AddKill()
    {
        killCount++;
        UpdateKillUI();
    }


    public void UpdateKillUI()
    {
        killText.text = killCount.ToString();
    }

}
