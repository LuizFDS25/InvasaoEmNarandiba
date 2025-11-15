using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
public class OnTrigger2DUtil : MonoBehaviour
{
    public string targetTag = "Player";
    public UnityEvent onTriggerEnterEvent, OnTriggerExitEvent;

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            onTriggerEnterEvent?.Invoke();
        }
    }

    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            OnTriggerExitEvent?.Invoke();
        }
        
        
    }
}
