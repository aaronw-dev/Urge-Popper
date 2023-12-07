using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using NaughtyAttributes;
using Redcode.Pools;
using MoreMountains.Feedbacks;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

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
    public bool isBomb;
    [Header("Feedback")]
    public UnityEvent OnBeginDestroy;
    public MMFeedbacks DestroyFeedback;

    [Header("Ray Casting")]
    public float radius = 5f;           
    public float forceMagnitude = 10f;  
    public int rayCount = 36;           
    float initGraphicScale;

    int bombCheckFrameRate = 0;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        initGraphicScale = transform.GetChild(1).localScale.x;
        //UpdateUrge();
    }
        private void OnEnable()
    {
        UpdateUrge();
        if (isBomb)
        {
            bombCheckFrameRate = Random.Range(3, 9);
           
        }
    }
    [Button]
    public void SolveThisBall()
    {
        contactedObjects.Clear();
        contactedObjects.Add(transform);
        _nodes.Clear();
        _hierachyNodes.Clear();
        hierarchyList.Clear();
        List<Transform> newcontactedObjects = new List<Transform>() {transform};
        SearchContacts(newcontactedObjects);
       
    }

    public List<List<Transform>> hierarchyList = new List<List<Transform>>();
    Vector2[] rayDirections;
    RaycastHit2D[] hits;
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
       
        if ( Time.frameCount % 5 == 0)
        {
            if(isHovering)
                OutlineAndSolve();
        }

        if(isBomb && Time.frameCount % bombCheckFrameRate ==0) 
        {
            Collider2D[] hitResults = Physics2D.OverlapCircleAll(transform.position, triggerRadius);
            for (int j = 0; j < hitResults.Length; j++)
            {
                Collider2D collider = hitResults[j];

                if (collider.CompareTag("Ball") && collider.gameObject != gameObject && collider.GetComponent<UrgeBody>().currentUrge == currentUrge && currentUrge >= -2)
                {
                    UrgeManager.Instance.MergeSequence(collider, transform,currentUrge);
                }
            }
        }
#if UNITY_EDITOR
        rayDirections = CalculateRayDirections();
        hits = ShootRays(rayDirections);

        VisualizeRays(rayDirections, hits);
#endif
    }

   

    public void ApplyRandomForce() 
    {
        StartCoroutine(ApplyRandomForceFixedUpdate());
    }
    IEnumerator ApplyRandomForceFixedUpdate() 
    {
        rayDirections = CalculateRayDirections();
        hits = ShootRays(rayDirections);

        VisualizeRays(rayDirections, hits);

        int randomRayIndex = GetRandomNonCollidingRayIndex(hits);
        yield return new WaitForFixedUpdate();
        if (randomRayIndex != -1)
        {
            ApplyForce(rayDirections[randomRayIndex]);
        }
    }
    Vector2[] CalculateRayDirections()
    {
        Vector2[] directions = new Vector2[rayCount];
        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * (360f / rayCount);
            directions[i] = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        }
        return directions;
    }

    RaycastHit2D[] ShootRays(Vector2[] rayDirections)
    {
        RaycastHit2D[] hits = new RaycastHit2D[rayCount];

        for (int i = 0; i < rayCount; i++)
        {
            RaycastHit2D[] rayHits = Physics2D.RaycastAll(transform.position, rayDirections[i], radius);
            for (int j = 0; j < rayHits.Length; j++)
            {
                if (rayHits[j].collider.transform != transform)
                {
                    hits[i] = rayHits[j];
                    break;
                }
            }
        }

        return hits;
    }

    void VisualizeRays(Vector2[] rayDirections, RaycastHit2D[] hits)
    {
        for (int i = 0; i < rayCount; i++)
        {
            Debug.DrawRay(transform.position, rayDirections[i] * radius, hits[i].collider ? Color.red : Color.green);
        }
    }

    int GetRandomNonCollidingRayIndex(RaycastHit2D[] hits)
    {
        List<int> nonCollidingIndices = new List<int>();
        for (int i = 0; i < rayCount; i++)
        {
            if (!hits[i].collider)
            {
                nonCollidingIndices.Add(i);
            }
        }

        if (nonCollidingIndices.Count > 0)
        {
            return nonCollidingIndices[Random.Range(0, nonCollidingIndices.Count)];
        }
        else
        {
            return -1;
        }
    }

    void ApplyForce(Vector2 direction)
    {
        Vector2 forceVector = direction * forceMagnitude;
        if (rb != null)
        {
            rb.AddForce(forceVector,ForceMode2D.Impulse);
        }
    }

    bool isHovering;
    private void OnMouseEnter()
    {
        if (UrgeManager.Instance.overUI) return;
        if (gameObject.layer == LayerMask.NameToLayer("ToBeDestroyed"))
            return;
        OnBeginDestroy?.Invoke();
        isHovering = true;
        OutlineAndSolve();
    }
    public void Hover(bool b) => isHovering = b;
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

        if (gameObject.layer == LayerMask.NameToLayer("ToBeDestroyed"))
            return;

        if (contactedObjects.Count < 3 && !isBomb)
        {
            return;
        }
        for (int i = 0; i < contactedObjects.Count; i++)
        {
            contactedObjects[i].GetChild(0).gameObject.SetActive(true);
            contactedObjects[i].GetChild(0).GetComponent<Renderer>().sortingOrder = 3 + (contactedObjects[i].GetComponent<UrgeBody>().isBomb ? 1 : 0);
            contactedObjects[i].GetChild(1).GetComponent<Renderer>().sortingOrder = 4 + (contactedObjects[i].GetComponent<UrgeBody>().isBomb ? 1 :0);
        }
    }


    private void OnMouseExit()
    {
        if (UrgeManager.Instance.overUI) return;
        if (gameObject.layer == LayerMask.NameToLayer("ToBeDestroyed"))
            return;
        isHovering = false;
        OnBeginDestroy?.Invoke();
        for (int i = 0; i < contactedObjects.Count; i++)
        {
            contactedObjects[i].GetChild(0).gameObject.SetActive(false);
            contactedObjects[i].GetChild(0).GetComponent<Renderer>().sortingOrder = 0;
            contactedObjects[i].GetChild(1).GetComponent<Renderer>().sortingOrder = 1;
        }
    }
    private void OnMouseUpAsButton()
    {
        if (UrgeManager.Instance.overUI) return;
        if (gameObject.layer == LayerMask.NameToLayer("ToBeDestroyed"))
            return;
        isHovering = false;
        for (int i = 0; i < contactedObjects.Count; i++)
        {
            contactedObjects[i].GetChild(0).gameObject.SetActive(false);
            contactedObjects[i].GetChild(0).GetComponent<Renderer>().sortingOrder = 0;
            contactedObjects[i].GetChild(1).GetComponent<Renderer>().sortingOrder = 1;
        }
        SolveThisBall();
        OnBeginDestroy?.Invoke();
        if (contactedObjects.Count < 3 && !isBomb)
            return;
        for (int i = 0; i < contactedObjects.Count; i++)
        {
            contactedObjects[i].gameObject.layer = LayerMask.NameToLayer("ToBeDestroyed");
                
        }
        BuildHierarchy();
        for (int i = 0; i < hierarchyList.Count; i++)
        {
            _hierachyNodes.Add(new Node(null, hierarchyList[i]));
        }

        UrgeManager.Instance.DestroyBalls(hierarchyList, currentUrge, contactedObjects.Count);
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
            List<Transform> BombContacts = new List<Transform>();
            Transform contact = contacts[i];
            Collider2D[] hitResults = Physics2D.OverlapCircleAll(contact.position, triggerRadius);
            for (int j = 0; j < hitResults.Length; j++)
            {
                Collider2D collider = hitResults[j];

                if (collider.CompareTag("Ball") && collider.gameObject != contact.gameObject)
                {
                    if (!contactedObjects.Contains(collider.transform) && !newContacts.Contains(collider.transform) && !contacts.Contains(collider.transform))
                    {

                        if (collider.GetComponent<UrgeBody>().currentUrge == currentUrge || isBomb)
                        {
                            contactedObjects.Add(collider.transform);
                            newContacts.Add(collider.transform);
                            if(isBomb && collider.GetComponent<UrgeBody>().currentUrge == currentUrge) 
                            {
                                BombContacts.Add(collider.transform);
                            }
                        }
                        
                    }
                }
                
            }
            if (newContacts.Count > 0 || isBomb)
            {
                
                Node n = new Node(contact, newContacts);
                _nodes.Add(n);
                if (!isBomb && newContacts.Count > 0)
                    SearchContacts(newContacts);
                else if (isBomb && BombContacts.Count > 0)
                    SearchContacts(BombContacts);

            }
        }
      
    }
    public void TakePool(string b = "Ball") 
    {
        transform.GetChild(1).localScale = Vector3.one * initGraphicScale;
        UrgeManager.Instance._poolManager.TakeToPool<Transform>(b, transform);
    }
    public void UpdateUrge()
    {
        transform.GetChild(1).gameObject.SetActive(true);
        if (!UrgeManager.Instance||isBomb)
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
