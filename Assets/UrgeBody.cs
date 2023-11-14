using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using NaughtyAttributes;
using Unity.VisualScripting;

public class UrgeBody : MonoBehaviour
{
    public int currentUrge;
    public List<Transform> contactedObjects;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Collider2D coll;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        UpdateUrge();
    }
    [Button]
    public void SolveThisBall()
    {
        UrgeBody[] results = UrgeManager.Instance.SolveConnectedBalls(this);
        foreach (UrgeBody body in results)
        {
            Destroy(body.gameObject);
            Destroy(gameObject);
        }
    }
    [Button]
    public void UpdateUrge()
    {
        spriteRenderer.color = UrgeManager.Instance.Urges[currentUrge].UrgeColor;
        rb.mass = UrgeManager.Instance.Urges[currentUrge].UrgeWeight;
    }
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Ball"))
            contactedObjects.Add(collider.transform);
    }
    public void OnTriggerExit2D(Collider2D collider)
    {
        if (contactedObjects.Contains(collider.transform))
            contactedObjects.Remove(collider.transform);
    }
}
