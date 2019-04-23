using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

    public static int totalCoins = 0;

    private PlayerController player;
    private GameManager gManager;

    public float coinSpeed = 10f;



    // Use this for initialization
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Drop()
    {
        //Give small margin to coins spawned, for visuals only.
        RandomizePosition();

        //Move the coin to the players current position.
        StartCoroutine(MoveToPlayer());
    }
    void RandomizePosition()
    {
        transform.position += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
    }
    public IEnumerator MoveToPlayer()
    {
        while(Vector2.Distance(transform.position, player.transform.position) > 0.01f)
        {
            float step = Time.deltaTime * coinSpeed;
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, step);
            yield return null;
        }
        player.CoinPickedUp();
        Destroy(gameObject);
    }

}
