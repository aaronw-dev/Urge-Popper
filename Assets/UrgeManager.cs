using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NaughtyAttributes;
using Redcode.Pools;
using UnityEngine;

[System.Serializable]
public class Urge
{
    public string UrgeName;
    public Color UrgeColor = new Color(66, 111, 255, 255);
    public float UrgeWeight;
    public Urge(string UrgeName, Color UrgeColor, float UrgeWeight)
    {
        this.UrgeName = UrgeName;
        this.UrgeColor = UrgeColor;
        this.UrgeWeight = UrgeWeight;
    }
}

public class UrgeManager : MonoBehaviour
{
    public static UrgeManager Instance;
    public PoolManager _poolManager;
    public Urge[] Urges;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    public Urge currentUrge(int index)
    {
        return Urges[index];
    }
    [Button]
    public void SpawnRandomBall()
    {
        SpawnBall(Random.Range(0f, 1f) - 0.5f, Random.Range(0, Urges.Length));
    }
    [Button]
    public void Spawn10Balls() 
    {
        for (int i = 0; i < 10; i++)
        {
            SpawnRandomBall();
        }
    }
    public void DestroyBalls(List<Transform> _t)
    {
        for (int i = 0; i < _t.Count; i++)
        {
            _poolManager.TakeToPool<Transform>("Ball", _t[i]);
        }
    }
    public void SpawnBall(float xPosition, int urgeIndex)
    {
        float screenSizeY = Camera.main.orthographicSize * 2;
        float screenSizeX = Camera.main.aspect * screenSizeY;

        xPosition = screenSizeX * xPosition;

        GameObject ball = _poolManager.GetFromPool<Transform>("Ball").gameObject;
        ball.transform.SetParent(transform, false);
        ball.transform.position = new Vector3(xPosition, screenSizeY / 2);
        ball.GetComponent<UrgeBody>().currentUrge = urgeIndex;
        ball.GetComponent<UrgeBody>().UpdateUrge();
    }
    public UrgeBody[] SolveConnectedBalls(UrgeBody InitialBody)
    {
        List<UrgeBody> results = new List<UrgeBody>();
        List<UrgeBody> finished = new List<UrgeBody>();
        int initialBodyUrge = InitialBody.currentUrge;
        foreach (Transform contactedObject in InitialBody.contactedObjects)
        {
            UrgeBody contactedUrgeBody = contactedObject.GetComponent<UrgeBody>();
            if (finished.Contains(contactedUrgeBody))
                continue;
            if (contactedUrgeBody.currentUrge == initialBodyUrge)
            {
                finished.Add(contactedUrgeBody);
                results.Add(contactedUrgeBody);
            }
            else
            {
                finished.Add(contactedUrgeBody);
            }
        }
        return results.ToArray();
    }
}