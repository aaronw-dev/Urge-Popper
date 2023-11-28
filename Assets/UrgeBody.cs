using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using NaughtyAttributes;
using Unity.VisualScripting;
using Redcode.Pools;
using MoreMountains.Feedbacks;
using System.Linq;

public class UrgeBody : MonoBehaviour,IPoolObject
{
    public int currentUrge;
    public List<Transform> contactedObjects = new List<Transform>();
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Collider2D coll;
    public float triggerRadius = 1.5f;
    public List<int> contactGroups = new List<int>();

    [Header("Feedback")]
    public MMFeedbacks DestroyFeedback;
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
        _nodes.Clear();
        _hierachyNodes.Clear();
        hierarchyList.Clear();
        SearchContacts(contactedObjects);
       
    }

    public List<List<Transform>> hierarchyList = new List<List<Transform>>();
    void BuildHierarchy()
    {
        foreach (Node node in _nodes)
        {
            int index = -1;
            List<Transform> myList = hierarchyList.Find(x => x.Contains(node.parentNode));
            if ( myList != null  && myList.Count>0) 
            {
                index = hierarchyList.IndexOf(myList);
            }
            else
            {
                List<Transform> newList = new List<Transform>
                {
                    node.parentNode
                };
                hierarchyList.Add(newList);
                index = hierarchyList.Count - 1;
            }

            if(index+1 >= hierarchyList.Count || hierarchyList.Count == 0) 
            {
                List<Transform> newList = new List<Transform>();
                newList.AddRange(node.childrenNode);
                hierarchyList.Add(newList);
            }
            else 
            {
                for (int i = 0; i < node.childrenNode.Count; i++)
                {
                    if (!hierarchyList[index+1].Contains(node.childrenNode[i]))
                    {
                        hierarchyList[index+1].Add(node.childrenNode[i]);
                    }
                }
            }
        }
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
        if (contactedObjects.Count < 3)
            return;
        BuildHierarchy();
        for (int i = 0; i < hierarchyList.Count; i++)
        {
            _hierachyNodes.Add(new Node(null, hierarchyList[i]));
        }

        UrgeManager.Instance.DestroyBalls(hierarchyList, currentUrge);
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

    [System.Serializable]
    public class Node 
    {
        public Transform parentNode;
        public List<Transform> childrenNode = new List<Transform>();

        public Node(Transform _t, List<Transform> _c)
        {
            parentNode = _t;
            childrenNode = _c;
        }
    }
   public List<Node> _nodes = new List<Node>();
   public  List<Node> _hierachyNodes = new List<Node>();
    void SearchContacts(List<Transform> contacts)
    {
        for (int i = 0; i < contacts.Count; i++)
        {
            List<Transform> newContacts = new List<Transform>();
            Transform contact = contacts[i];
            Collider2D[] hitResults = Physics2D.OverlapCircleAll(contact.position, triggerRadius);

            for (int j = 0; j < hitResults.Length; j++)
            {
                Collider2D collider = hitResults[j];
                if (collider.CompareTag("Ball") && collider.GetComponent<UrgeBody>().currentUrge == currentUrge && collider.gameObject!= contact.gameObject)
                {
                    if (!contactedObjects.Contains(collider.transform) && !newContacts.Contains(collider.transform) && !contacts.Contains(collider.transform) )
                    {
                        contactedObjects.Add(collider.transform);
                        newContacts.Add(collider.transform);
                    }
                }
            }
            if (newContacts.Count > 0)
            {
                Node n = new Node(contact, newContacts);
                _nodes.Add(n);
                SearchContacts(newContacts);
            }
        }
      
    }
    public void TakePool() 
    {
        transform.GetChild(1).localScale = Vector3.one * 0.2640481f;
        UrgeManager.Instance._poolManager.TakeToPool<Transform>("Ball", transform);
    }
    public void UpdateUrge()
    {
        if (!UrgeManager.Instance)
            return;
        transform.GetChild(1).gameObject.SetActive(true);
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
