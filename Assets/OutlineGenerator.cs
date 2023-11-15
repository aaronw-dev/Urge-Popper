using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineGenerator : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    LineRenderer lineRenderer;

    public float thickness = 1;
    public Material material;
    public int smoothing = 0;

    public float extrusion = 0;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponentInChildren<LineRenderer>();
    }
    [Button]
    void GenerateOutline()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponentInChildren<LineRenderer>();

        if (spriteRenderer == null || spriteRenderer.sprite == null)
        {
            Debug.LogError("SpriteRenderer or Sprite is missing.");
            return;
        }

        Vector2[] vertices = spriteRenderer.sprite.vertices;

        
       if(lineRenderer== null)
        {
            lineRenderer = new GameObject("outline").AddComponent<LineRenderer>();
            lineRenderer.transform.SetParent(transform);
            lineRenderer.transform.localPosition = Vector3.zero;

        }
        lineRenderer.positionCount = vertices.Length + smoothing;
        lineRenderer.loop = smoothing == 0;
        lineRenderer.useWorldSpace = false;
        lineRenderer.material = material;
        lineRenderer.startWidth = thickness/100f;
        lineRenderer.endWidth = thickness/100f;
        List<Vector3> calcPos = new List<Vector3>();
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 vertex = vertices[i];
            // Convert local space vertex to world space
            Vector3 worldPosition = new Vector3(vertex.x, vertex.y, 0f);
            Vector3 direction = (Vector3.zero - worldPosition).normalized;
            Vector3 extrudedPos = worldPosition + (direction * -((thickness+extrusion)/200) );
            calcPos.Add(extrudedPos);
        }

        calcPos.Sort(new Vector3Comparer(Vector3.zero));
        for (int j = 0; j < smoothing; j++)
        {
            calcPos.Add(calcPos[j]);
        }

        lineRenderer.SetPositions(calcPos.ToArray());
    }

    double LinearInterpolation(double x)
    {
        // Provided data points
        double input1 = 10;
        double output1 = 1.055;
        double input2 = 20;
        double output2 = 1.105;

        // Linear interpolation formula
        return output1 + (x - input1) * ((output2 - output1) / (input2 - input1));
    }
}


public class Vector3Comparer : IComparer<Vector3>
{

    private Vector3 center;

    public Vector3Comparer(Vector3 center)
    {
        this.center = center;
    }
    public int Compare(Vector3 v1, Vector3 v2)
    {
        float angle1 = Mathf.Atan2(v1.y - center.y, v1.x - center.x);
        float angle2 = Mathf.Atan2(v2.y - center.y, v2.x - center.x);

        if (Mathf.Approximately(angle1, angle2))
        {
            // If two vertices have the same angle, compare their distances from the center
            float distance1 = Vector3.Distance(v1, center);
            float distance2 = Vector3.Distance(v2, center);
            return distance1.CompareTo(distance2);
        }

        return angle1.CompareTo(angle2);
    }
}