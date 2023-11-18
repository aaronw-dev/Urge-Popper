using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using NaughtyAttributes;
using Unity.VisualScripting;

public class UrgeBody : MonoBehaviour
{
    public int currentUrge;
    public List<Transform> contactedObjects = new List<Transform>();
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Collider2D coll;
    public float triggerRadius = 1.5f;
    public List<int> contactGroups = new List<int>();


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
        /*        UrgeBody[] results = UrgeManager.Instance.SolveConnectedBalls(this);
                foreach (UrgeBody body in results)
                {
                    Destroy(body.gameObject);
                    Destroy(gameObject);
                }*/
        contactedObjects.Clear();
        contactedObjects.Add(transform);
        contactGroups.Add(1);
        SearchContacts(contactedObjects);
        
        //StartCoroutine(DestroyItems());

    }

    private void OnMouseEnter()
    {
        SolveThisBall();
        for (int i = 0; i < contactedObjects.Count; i++)
        {
            contactedObjects[i].GetChild(0).gameObject.SetActive(true);
        }
    }
    private void OnMouseExit()
    {
        for (int i = 0; i < contactedObjects.Count; i++)
        {
            contactedObjects[i].GetChild(0).gameObject.SetActive(false);
        }
    }
    private void OnMouseUpAsButton()
    {
        for (int i = 0; i < contactedObjects.Count; i++)
        {
            contactedObjects[i].GetChild(0).gameObject.SetActive(false);
        }
        SolveThisBall();
        UrgeManager.Instance.DestroyBalls(contactedObjects);
    }
    IEnumerator DestroyItems() 
    {
        int last = 0;
        for (int i = 0; i < contactGroups.Count; i++)
        {
            for (int ij = 0; ij < contactGroups[i]; ij++)
            {
                contactedObjects[last+ ij].localScale = Vector3.zero;

            }
            last += contactGroups[i];
            yield return new WaitForSeconds(.01f);
        }
        yield return null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,triggerRadius);
    }

    void SearchContacts(List<Transform> contacts) 
    {
        List<Transform> newContacts = new List<Transform>();
        for (int i = 0; i < contacts.Count; i++)
        {
            Transform contact = contacts[i];
            Collider2D[] hitResults = Physics2D.OverlapCircleAll(contact.position, triggerRadius);

            for (int j = 0; j < hitResults.Length; j++)
            {
                Collider2D collider = hitResults[j];
                if (collider.CompareTag("Ball") && collider.GetComponent<UrgeBody>().currentUrge == currentUrge)
                {
                    if (!contactedObjects.Contains(collider.transform) && !newContacts.Contains(collider.transform) && !contacts.Contains(collider.transform))
                    {
                        contactedObjects.Add(collider.transform);
                        newContacts.Add(collider.transform);
                    }
                }
            }
            if (newContacts.Count > 0)
            {
                SearchContacts(newContacts);
                
            }
        }
            if (newContacts.Count > 0)
                contactGroups.Add(newContacts.Count);
    }
    public void UpdateUrge()
    {
        spriteRenderer.color = UrgeManager.Instance.Urges[currentUrge].UrgeColor;
        rb.mass = UrgeManager.Instance.Urges[currentUrge].UrgeWeight;
    }
  /*  public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Ball"))
            contactedObjects.Add(collider.transform);
    }
    public void OnTriggerExit2D(Collider2D collider)
    {
        if (contactedObjects.Contains(collider.transform))
            contactedObjects.Remove(collider.transform);
    }*/
}
