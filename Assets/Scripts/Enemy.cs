using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour {

    public static int Count = 0;
    public static int CountDestroyed = 0;
    public static float healtRangeMax = 1;

    private int topHealth = 0, currentHealth = 0;
    private int vDirection = 1;
    private int coinAmount = 0;
    private bool split;

    private float size;
    private float speedFactor = 10;
    private float pushForce = Constants.ENEMY_PUSHFORCE;
    private float[] directionRange = { Constants.ENEMY_DIRECTIONRANGE, -Constants.ENEMY_DIRECTIONRANGE };

    private float[] sizeRange = { Constants.ENEMY_SIZE_SMALL,
                                  Constants.ENEMY_SIZE_MEDIUM,
                                  Constants.ENEMY_SIZE_BIG};
    private float direction;
    private float animDuration = 0.5f;


    private TextMesh healthText;

    private Rigidbody2D rBody;
    private GameManager gManager;
    private PlayerController player;

    [SerializeField]
    private Coin coin;
    [SerializeField]
    private PowerUp powerUp;

    public ConstantForce2D gravity;

    public GravityDirection gravDir;

    // Use this for initialization
    private void Awake()
    {
        init();
    }
    void Start () {
        Push();
    }

    public void init()
    {
        Count++;

        rBody = GetComponent<Rigidbody2D>();

        gManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        player.ePowerPickedUp += PowerPickedUp;

        //Randomizing the enemy properties
        size = sizeRange[Helpers.RandomWeighted(15,50,35)];
        coinAmount = Random.Range(1, 4);
        healtRangeMax = 1 + Mathf.Floor((float)SaveData.LoadFireRate() / 2);
        topHealth = Random.Range(1, Mathf.CeilToInt(healtRangeMax));
        currentHealth = topHealth;
        
        direction = directionRange[Random.Range(0, 2)];
    }
    private void OnDestroy()
    {
        player.ePowerPickedUp -= PowerPickedUp;
    }

    void Push()
    {

        healthText = transform.Find("healthText").GetComponent<TextMesh>();
        healthText.text = currentHealth.ToString();

        //Depending on the gravity direction, balls moves either the top of the screen or the bottom.
        if ((gravDir == GravityDirection.top) && (GetComponent<ConstantForce2D>() == null))
        {
            vDirection = -1;
            gravity = gameObject.AddComponent<ConstantForce2D>();
            gravity.force = new Vector3(0.0f, -Physics2D.gravity.y, 0.0f);
            rBody.gravityScale = 0;

        }
        //Spawning the enemy by scaling it out of zero then adding a small force.
        transform.DOScale(new Vector3(size, size, 1), animDuration).OnComplete(() => {
            rBody.isKinematic = false;
            rBody.AddForce(new Vector2(direction, pushForce*vDirection) * speedFactor, ForceMode2D.Impulse);
        });
    }
	// Update is called once per frame
	void Update () {
		
	}

    private void PowerPickedUp(PowerUpType type)
    {
        if (type == PowerUpType.freeze)
        {
            StartCoroutine(Freeze());
        }
    }
    IEnumerator Freeze()
    {
        //Record the enemies velocity, freeze for 3 seconds, then reset the velocity to previous values 
        Vector2 velocity = rBody.velocity;
        float angularVelocity = rBody.angularVelocity;

        rBody.velocity = Vector2.zero;
        rBody.angularVelocity = 0;

        rBody.isKinematic = true;
        print("freeze");
        yield return new WaitForSeconds(3);

        rBody.isKinematic = false;

        rBody.velocity = velocity;
        rBody.angularVelocity = angularVelocity;
    }
    public void Damage()
    {
        //Scales the size like a punch animation when hit
        if (!DOTween.IsTweening(transform))
            transform.DOPunchScale(new Vector2(0.06f, 0.06f), 0.1f);

        //Reduce the enemy health by 1
        if (currentHealth > 0)
            currentHealth--;

        //If health is zero, kill the enemy
        if (currentHealth == 0)
        {
            //if it's one of the bigger ones, split it to a smaller part 
            if (size > Constants.ENEMY_SIZE_SMALL)
            {
                Split();
                //Destroy(gameObject);
            }
            else
            {
                Die();
                DropPowerup();
                StartCoroutine(DropCoins());
            }

        }

        //Display the current health of the enemy
        healthText.text = currentHealth.ToString();
    }
    IEnumerator DropCoins()
    {
        for(int i = 0; i<coinAmount; i++)
        {
            Coin newCoin = Instantiate(coin, transform.position, transform.rotation);
            //coin.transform.position = transform.position;
            newCoin.Drop();

            yield return null;
        }
    }
    void DropPowerup()
    {
        //Drop powerup every 20 enemy killed
        if(((CountDestroyed % 20) == 0) && (PowerUp.totalPowerups == 0))
        {
            PowerUp newPower = Instantiate(powerUp, transform.position, transform.rotation);
            //newPower.type = (PowerUpType)Random.Range(0, (int)PowerUpType.length);
            newPower.type = PowerUpType.freeze;
            newPower.Drop();
        }

    }
    void Die()
    {
        if (!DOTween.IsTweening(Camera.main))
            Camera.main.DOShakePosition(.15f, 0.1f);

        //Disable collider to avoid unnecessary collision after it has died
        GetComponent<Collider2D>().enabled = false;

        //Scale+fade animations to indicete the enemy has died
        transform.DOScale(transform.localScale * 1.5f, .15f);
        transform.GetComponent<SpriteRenderer>().DOFade(0, .15f).OnComplete(()=> {
            //Kill the enemy when the animation is over
            gameObject.SetActive(false);
            CountDestroyed++;
            Destroy(gameObject);
        });
    }

    //Splits the enemy to 2, each moves to the opposite sides
    void Split()
    {
        //Instantiate 2 enemy object, set their properties
        CreateSplitEnemy(0);
        CreateSplitEnemy(1);

        Die();
    }

    void CreateSplitEnemy(int direction)
    {
        GameObject newEnemy = Instantiate(gameObject);
        Enemy enemyController = newEnemy.GetComponent<Enemy>();

        enemyController.pushForce = pushForce;
        enemyController.vDirection = (gravDir == GravityDirection.top) ? 1 : -1;
        enemyController.direction = directionRange[direction];
        enemyController.size = size - 0.15f;
        enemyController.animDuration = 0;
        enemyController.gravDir = gravDir;

        //New health is half of the top health from the previous enemy
        enemyController.topHealth = Mathf.CeilToInt((float)topHealth / 2);
        enemyController.currentHealth = Mathf.CeilToInt((float)topHealth / 2);


        enemyController.split = true;
    }

    //if the enemy is out of the screen boundaries, move it to the opposite side of the screen.
    void OnBecameInvisible()
    {
        transform.position = new Vector2(-1 * transform.position.x, transform.position.y);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (split)
        {
            //If the enemy is one of the split ones and hits a wall for the first time, adjust the force applied to it
            if(collision.gameObject.tag == "Wall")
            {
                ResetSpeed();
            }
        }
    }

    void ResetSpeed()
    {
        rBody.velocity = Vector2.zero;
        rBody.angularVelocity = 0;

        pushForce = Constants.ENEMY_PUSHFORCE;
        rBody.AddForce(new Vector2(direction, pushForce * vDirection) * speedFactor, ForceMode2D.Impulse);
        split = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            gManager.FinishGame();
        }
    }
}
