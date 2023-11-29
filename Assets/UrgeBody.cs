using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using NaughtyAttributes;
using Unity.VisualScripting;
using Redcode.Pools;

public class UrgeBody : MonoBehaviour, IPoolObject
{
    public int currentUrge;
    public List<Transform> contactedObjects = new List<Transform>();
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Collider2D coll;
    public float triggerRadius = 1.5f;
    public List<int> contactGroups = new List<int>();
    public bool hasPassedInJars;


    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        //UpdateUrge();
    }
    private void OnEnable()
    {
        UpdateUrge();
    }
    [Button]
    public void SolveThisBall()
    {
        contactedObjects.Clear();
        contactedObjects.Add(transform);
        contactGroups.Add(1);
        SearchContacts(contactedObjects);

        //StartCoroutine(DestroyItems());

    }
    private void Update()
    {
        if (isHovering && Time.frameCount % 5 == 0)
            OutlineAndSolve();
    }
    bool isHovering;
    private void OnMouseEnter()
    {
        isHovering = true;
        OutlineAndSolve();
    }

    private void OutlineAndSolve()
    {
        if (UrgeManager.Instance.spawningDone == false) return;
        for (int i = 0; i < contactedObjects.Count; i++)
        {
            contactedObjects[i].GetChild(0).gameObject.SetActive(false);
            contactedObjects[i].GetChild(0).GetComponent<Renderer>().sortingOrder = 0;
            contactedObjects[i].GetChild(1).GetComponent<Renderer>().sortingOrder = 1;
        }
        SolveThisBall();

        if (contactedObjects.Count < 3)
        {
            return;
        }
        for (int i = 0; i < contactedObjects.Count; i++)
        {
            contactedObjects[i].GetChild(0).gameObject.SetActive(true);
            contactedObjects[i].GetChild(0).GetComponent<Renderer>().sortingOrder = 3;
            contactedObjects[i].GetChild(1).GetComponent<Renderer>().sortingOrder = 4;
        }
    }

    private void OnMouseExit()
    {
        isHovering = false;
        for (int i = 0; i < contactedObjects.Count; i++)
        {
            contactedObjects[i].GetChild(0).gameObject.SetActive(false);
            contactedObjects[i].GetChild(0).GetComponent<Renderer>().sortingOrder = 0;
            contactedObjects[i].GetChild(1).GetComponent<Renderer>().sortingOrder = 1;
        }
    }
    private void OnMouseUpAsButton()
    {
        isHovering = false;
        for (int i = 0; i < contactedObjects.Count; i++)
        {
            contactedObjects[i].GetChild(0).gameObject.SetActive(false);
            contactedObjects[i].GetChild(0).GetComponent<Renderer>().sortingOrder = 0;
            contactedObjects[i].GetChild(1).GetComponent<Renderer>().sortingOrder = 1;
        }
        SolveThisBall();
        UrgeManager.Instance.DestroyBalls(contactedObjects, currentUrge);
    }
    IEnumerator DestroyItems()
    {
        int last = 0;
        for (int i = 0; i < contactGroups.Count; i++)
        {
            for (int ij = 0; ij < contactGroups[i]; ij++)
            {
                contactedObjects[last + ij].localScale = Vector3.zero;

            }
            last += contactGroups[i];
            yield return new WaitForSeconds(.01f);
        }
        yield return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
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
        if (!UrgeManager.Instance)
            return;
        spriteRenderer.color = UrgeManager.Instance.Urges[currentUrge].UrgeColor;
        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = UrgeManager.Instance.Urges[currentUrge].UrgeGraphic;
        rb.mass = UrgeManager.Instance.Urges[currentUrge].UrgeWeight;
    }

    public void OnCreatedInPool()
    {
        UpdateUrge();
    }

    public void OnGettingFromPool()
    {
        UpdateUrge();
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
