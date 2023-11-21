using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NaughtyAttributes;
using Redcode.Pools;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class Urge
{
    public string UrgeName;
    public Color UrgeColor = new Color(66, 111, 255, 255);
    public float UrgeWeight;
    public int UrgeScore;
    public Sprite UrgeGraphic;
    public Urge(string UrgeName, Color UrgeColor, float UrgeWeight, Sprite Graphic)
    {
        this.UrgeName = UrgeName;
        this.UrgeColor = UrgeColor;
        this.UrgeWeight = UrgeWeight;
        this.UrgeGraphic = Graphic;
    }
}

public class UrgeManager : MonoBehaviour
{
    public static UrgeManager Instance;
    public PoolManager _poolManager;
    public Urge[] Urges;

    [Header("Gameplay")]
    public int defaultMovesToNextSpawn;
    public int nextSpawnAmount = 10;
    public TextMeshProUGUI movesTxt;
    int movesLeft;
    public bool spawningDone = true;
    private void Start()
    {
        if (Instance == null)
            Instance = this;
        movesLeft = defaultMovesToNextSpawn;
        movesTxt.text = movesLeft.ToString();
        StartCoroutine(SpawnBalls(nextSpawnAmount));
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
    public float maxX = 1;
    public float spawnYOffset = 0;
    private void OnDrawGizmos()
    {
        float screenSizeY = Camera.main.orthographicSize * 2;
        float screenSizeX = Camera.main.aspect * screenSizeY;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(0, (screenSizeY / 2)+ spawnYOffset) , new Vector3(maxX * screenSizeX,0.5f ));
    }
    [Button]
    public void SpawnRandomBall()
    {
        SpawnBall(Random.Range(0f, maxX) - maxX/2f, Random.Range(0, Urges.Length));
    }
    [Button]
    public void Spawn10Balls()
    {
        StartCoroutine( SpawnBalls(nextSpawnAmount));
    }

    IEnumerator SpawnBalls(int _i) 
    {
        spawningDone = false;
        for (int i = 0; i < _i; i++)
        {
            yield return new WaitForSeconds(0.1f);
            SpawnRandomBall();
        }
        spawningDone = true;
    }
    public void DestroyBalls(List<Transform> _t, int urgeIndex)
    {
        if (_t.Count < 3)
            return;

        movesLeft--;
        if (movesLeft <= 0)
        {
            movesLeft = defaultMovesToNextSpawn;
            StartCoroutine(SpawnBalls(nextSpawnAmount));
        }
        movesTxt.text = movesLeft.ToString();

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
        ball.transform.position = new Vector3(xPosition, (screenSizeY / 2)+ spawnYOffset);
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