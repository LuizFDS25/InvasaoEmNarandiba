using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float delay = 1.5f;
    void Start() => Destroy(gameObject, delay);
}
