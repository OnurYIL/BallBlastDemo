using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    public static int totalPowerups = 0;

    private PlayerController player;
    private GameManager gManager;

    public float powerupSpeed = 5f;

    public PowerUpType type;


    // Use this for initialization
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        totalPowerups++;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Drop()
    {
        RandomizePosition();
        StartCoroutine(MoveToPlayer());
    }
    void RandomizePosition()
    {
        transform.position += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
    }
    public IEnumerator MoveToPlayer()
    {

        while (Vector2.Distance(transform.position, player.transform.position) > 0.01f)
        {
            float step = Time.deltaTime * powerupSpeed;
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, step);
            yield return null;
        }
        player.PowerPickedUp(type);
        totalPowerups--;
        Destroy(gameObject);
    }


}
