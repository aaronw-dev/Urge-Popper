using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailState : MonoBehaviour
{
    public float secondsToFail = 2f;
    public GameEvent FailEvent;

    Coroutine FailRoutine;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball")) 
        {
            if (FailRoutine != null)
                StopCoroutine(FailRoutine);
            FailRoutine = StartCoroutine(FailyCounter());
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            if(FailRoutine == null)
            FailRoutine = StartCoroutine(FailyCounter());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            if (FailRoutine != null) 
            {
                StopCoroutine(FailRoutine);
                FailRoutine = null;
            }

        }
    }
    IEnumerator FailyCounter() 
    {
        yield return new WaitForSeconds(secondsToFail);
        FailEvent.Raise();
    }
}
