using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using NaughtyAttributes;
using Redcode.Pools;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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


[System.Serializable]
public class ComboCelebText 
{
    public int comboAmount = 3;
    public string[] phrases;
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
    [ReadOnly] public bool overUI = false;
    [Header("Combo Phrases")]
    public ComboCelebText[] comboPhrases;
    public string[] bombComboString;
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
    private void Update()
    {
        overUI = EventSystem.current.IsPointerOverGameObject();
        foreach (Touch touch in Input.touches)
        {
            int id = touch.fingerId;
            if (EventSystem.current.IsPointerOverGameObject(id))
            {
                overUI = true;
                break;
            }
        }
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
    public void MergeSequence(Collider2D collider, Transform t, int currentUrge)
    {
        t.gameObject.layer = LayerMask.NameToLayer("ToBeDestroyed");
        collider.gameObject.layer = LayerMask.NameToLayer("ToBeDestroyed");
        t.GetChild(1).GetComponent<MMBlink>().StartBlinking();
        collider.transform.GetChild(1).GetComponent<MMBlink>().StartBlinking();
        Sequence MergeSeq = DOTween.Sequence();
        Vector3 middleDistance = Vector3.Lerp(t.position, collider.transform.position, 0.5f);
        MergeSeq.Append(t.transform.GetChild(1).DOMove(middleDistance, 0.45f).SetEase(Ease.InBack));
        MergeSeq.Insert(0, collider.transform.GetChild(1).DOMove(middleDistance, 0.45f).SetEase(Ease.InBack));
        MergeSeq.AppendCallback(() => 
        { 
            GameObject newBomb = _poolManager.GetFromPool<Transform>(bombComboString[Mathf.Abs(currentUrge)]).gameObject;
            newBomb.transform.position = middleDistance;
            newBomb.transform.DOScale(0, 0);
            newBomb.transform.DOScale(1, 0.2f);
            Collider2D[] hitResults = Physics2D.OverlapCircleAll(middleDistance, newBomb.GetComponent<UrgeBody>().bombMergeRadius);
            for (int j = 0; j < hitResults.Length; j++)
            {
                Collider2D collider = hitResults[j];
                if (collider.CompareTag("Ball") && collider.gameObject != gameObject)
                {
                    AddExplosionForce(collider.attachedRigidbody, 100, middleDistance, newBomb.GetComponent<UrgeBody>().bombMergeRadius, mode: ForceMode2D.Impulse);
                }
            }
        });
        MergeSeq.AppendInterval(0.1f);
        MergeSeq.AppendCallback(() =>
        {
            t.transform.GetChild(1).DOLocalMove(Vector3.zero, 0);
            collider.gameObject.transform.GetChild(1).DOLocalMove(Vector3.zero, 0);
            _poolManager.TakeToPool<Transform>(bombComboString[Mathf.Abs(currentUrge) - 1], collider.transform);
            _poolManager.TakeToPool<Transform>(bombComboString[Mathf.Abs(currentUrge) - 1], t);
            t.gameObject.layer = 0;
            collider.gameObject.layer = 0;
        });
    }
    public void AddExplosionForce(Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier = 0.0F, ForceMode2D mode = ForceMode2D.Force)
    {
        var explosionDir = rb.position - explosionPosition;
        var explosionDistance = (explosionDir.magnitude / explosionRadius);

        // Normalize without computing magnitude again
        if (upwardsModifier == 0)
        {
            explosionDir /= explosionDistance;
        }
        else
        {
            // If you pass a non-zero value for the upwardsModifier parameter, the direction
            // will be modified by subtracting that value from the Y component of the centre point.
            explosionDir.y += upwardsModifier;
            explosionDir.Normalize();
        }

        rb.AddForce(Mathf.Lerp(0, explosionForce, (1 - explosionDistance)) * explosionDir, mode);
    }
    IEnumerator SpawnBalls(int _i) 
    {
        spawningDone = false;
        for (int i = 0; i < _i; i++)
        {
            yield return new WaitForSeconds(0.01f);
            SpawnRandomBall();
        }
        spawningDone = true;
    }
    public void DestroyBalls(List<List<Transform>> _t, int urgeIndex, int count)
    {
        movesLeft--;
        if (movesLeft <= 0)
        {
            Timer.Register(1f, () =>
            {
                movesLeft = defaultMovesToNextSpawn;
                movesTxt.text = movesLeft.ToString();
            });
            StartCoroutine(SpawnBalls(nextSpawnAmount));
        }
        movesTxt.text = movesLeft.ToString();
        StartCoroutine(DestroyDelay(_t, count));
         int urgeScore = 0;
        if (urgeIndex > 0)
        {
            urgeScore = UrgeManager.Instance.Urges[urgeIndex].UrgeScore;
        }
        else
        {
            //Fix this properly later
            urgeScore = UrgeManager.Instance.Urges[0].UrgeScore;
        }
        int newScore = urgeScore * count;
        ScoreManager.Instance.PublicScore += newScore;

    }
    IEnumerator DestroyDelay(List<List<Transform>> _t, int c) 
    {
        for (int i = 0; i < _t.Count; i++)
        {
            for (int l = 0; l < _t[i].Count; l++)
            {
                _t[i][l].GetComponent<UrgeBody>().DestroyFeedback?.PlayFeedbacks();
                if (c >= 5 && i == 0 && l == 0 && !_t[i][l].GetComponent<UrgeBody>().isBomb)
                {
                    Vector3 pos = _t[i][l].position;
                  Timer.Register(0.7f,()=> _poolManager.GetFromPool<Transform>("Bomb").transform.position =pos);
                }
            }
            yield return new WaitForSeconds(0.1f);

        }
        for (int l = 0; l < comboPhrases.Length; l++)
        {
            if (comboPhrases[l].comboAmount == c) 
            {
                _poolManager.GetFromPool<TextMeshProUGUI>("ComboText").text = comboPhrases[l].phrases.MMRandomValue();
                break;
            }
            else if(l == comboPhrases.Length-1 && c > comboPhrases[l].comboAmount) 
            {
                _poolManager.GetFromPool<TextMeshProUGUI>("ComboText").text = comboPhrases[l].phrases.MMRandomValue();
                break;
            }
        } 

    }
    int ballIndex = 1;
    public void TakeComboTextPool(TextMeshProUGUI _t)
    {
        _poolManager.TakeToPool<TextMeshProUGUI>("ComboText", _t);
    }
    public void SpawnBall(float xPosition, int urgeIndex)
    {
        float screenSizeY = Camera.main.orthographicSize * 2;
        float screenSizeX = Camera.main.aspect * screenSizeY;

        xPosition = screenSizeX * xPosition;

        GameObject ball = _poolManager.GetFromPool<Transform>("Ball").gameObject;
        ball.name = "ball" + ballIndex;
        ballIndex++;
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