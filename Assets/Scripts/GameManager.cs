using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {

    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private Transform enemySpawnPoints_top, enemySpawnPoints_bot;

    [SerializeField]
    private UIController uiController;
    private ConstantForce2D gravity;


    private bool gameStarted = false;
    private bool gameFinished = false;

    public delegate void OnGameStarted();
    public delegate void OnGameFinished();

    public event OnGameStarted eGameStarted;
    public event OnGameFinished eGameFinished;

    void Start()
    {
        GenerateEdgeColliders();

        //StartGame();
    }

    // Update is called once per frame
    void Update()
    {

        if (EventSystem.current.currentSelectedGameObject != null) return;

        if (!gameStarted)
            CheckForStartTouch();
    }

    void GenerateEdgeColliders()
    {
        Vector2 lDCorner = Camera.main.ViewportToWorldPoint(new Vector3(0, 0f, Camera.main.nearClipPlane));
        Vector2 rUCorner = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.main.nearClipPlane));

        Vector2[] colliderpoints;

        EdgeCollider2D upperEdge = new GameObject("upperEdge").AddComponent<EdgeCollider2D>();
        colliderpoints = upperEdge.points;
        colliderpoints[0] = new Vector2(lDCorner.x-2, rUCorner.y);
        colliderpoints[1] = new Vector2(rUCorner.x+2, rUCorner.y);
        upperEdge.points = colliderpoints;
        upperEdge.tag = "Wall";
/*
        EdgeCollider2D lowerEdge = new GameObject("lowerEdge").AddComponent<EdgeCollider2D>();
        colliderpoints = lowerEdge.points;
        colliderpoints[0] = new Vector2(lDCorner.x, lDCorner.y);
        colliderpoints[1] = new Vector2(rUCorner.x, lDCorner.y);
        lowerEdge.points = colliderpoints;

        EdgeCollider2D leftEdge = new GameObject("leftEdge").AddComponent<EdgeCollider2D>();
        colliderpoints = leftEdge.points;
        colliderpoints[0] = new Vector2(lDCorner.x, lDCorner.y);
        colliderpoints[1] = new Vector2(lDCorner.x, rUCorner.y);
        leftEdge.points = colliderpoints;

        EdgeCollider2D rightEdge = new GameObject("rightEdge").AddComponent<EdgeCollider2D>();

        colliderpoints = rightEdge.points;
        colliderpoints[0] = new Vector2(rUCorner.x, rUCorner.y);
        colliderpoints[1] = new Vector2(rUCorner.x, lDCorner.y);
        rightEdge.points = colliderpoints;*/
    }
    // Use this for initialization

    void CheckForStartTouch()
    {
        if (Input.GetMouseButton(0))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        Enemy.Count = 0;
        Enemy.CountDestroyed = 0;
        PowerUp.totalPowerups = 0;
        //Hide the ui, start spawning enemies
        uiController.ShowStartUI(false);
        StartCoroutine("EnemySpawner");

        if (eGameStarted != null)
            eGameStarted();

    }
    public void FinishGame()
    {
        gameFinished = true;
        //Show n
        uiController.ShowEndUI(true);

        if (eGameFinished != null)
            eGameFinished();
    }
    IEnumerator EnemySpawner()
    {
        if (!gameFinished)
        {
            Vector3 spawnPos = enemySpawnPoints_top.GetChild(Random.Range(0, enemySpawnPoints_top.childCount)).position;

            Spawn(spawnPos, GravityDirection.top);
            yield return new WaitForSeconds(3);

            spawnPos = enemySpawnPoints_bot.GetChild(Random.Range(0, enemySpawnPoints_bot.childCount)).position;

            Spawn(spawnPos, GravityDirection.bottom);
            yield return new WaitForSeconds(3);

            StartCoroutine("EnemySpawner");
        }
    }
    void Spawn(Vector2 spawnPos, GravityDirection gravDir)
    {
        GameObject enemyClone = Instantiate(enemyPrefab);
        enemyClone.transform.position = spawnPos;
        enemyClone.GetComponent<Enemy>().gravDir = gravDir;
    }




}
