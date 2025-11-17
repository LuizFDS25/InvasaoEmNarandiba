using UnityEngine;

public class InvisWall : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(true);
    }

    public void DesativarParede()
    {
        gameObject.SetActive(false);
    }
}
