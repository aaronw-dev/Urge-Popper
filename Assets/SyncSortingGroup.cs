using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
public class SyncSortingGroup : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    [ReadOnly]
    public List<int> childrenSortingOrder = new List<int>();

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        childrenSortingOrder.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            childrenSortingOrder.Add(child.GetComponent<SpriteRenderer>().sortingOrder);
        }
    }
    void Update()
    {
        if (Time.frameCount % 10 == 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = childrenSortingOrder[i] + spriteRenderer.sortingOrder;
            }
        }
    }

    void OnDisable()
    {
        if (transform.childCount == childrenSortingOrder.Count)
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = childrenSortingOrder[i];
            }
    }
}
