using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NaughtyAttributes;
using Redcode.Pools;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Urge
{
    public string UrgeName;
    public Color UrgeColor = new Color(66, 111, 255, 255);
    public float UrgeWeight;
    public int UrgeScore;
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

    private void Start()
    {
        if (Instance == null)
            Instance = this;
    }
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
    public void DestroyBalls(List<Transform> _t, int urgeIndex)
    {
        if (_t.Count < 3)
            return;
        for (int i = 0; i < _t.Count; i++)
        {
            _poolManager.TakeToPool<Transform>("Ball", _t[i]);
        }
        int urgeScore = UrgeManager.Instance.Urges[urgeIndex].UrgeScore;
        int newScore = urgeScore * _t.Count;
        ScoreManager.Instance.PublicScore += newScore;
        Debug.Log("Added " + newScore + " score");
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