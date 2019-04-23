using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour {

    private int smoothScale = 20;

    private float bulletOffset = 0f;

    [SerializeField]
    private Transform weaponTop, weaponBottom;

    [SerializeField]
    private GameObject bulletPrefab;

    //[SerializeField]
    public static float rateOfFire = 5;

    private float timeToFire = 0;
    private bool movable = true;

    private BulletPooling bulletPooling;
    private GameManager gManager;


    public delegate void OnCoinPickedUp();
    public delegate void OnPowerPickedUp(PowerUpType type);

    public event OnCoinPickedUp eCoinPickedUp;
    public event OnPowerPickedUp ePowerPickedUp;

    // Use this for initialization
    private void Awake()
    {
        bulletPooling = GetComponent<BulletPooling>();
        gManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        rateOfFire = Constants.DEFAULT_GUN_RATE + SaveData.LoadFireRate();
    }
    void Start () {
        gManager.eGameStarted += GameStarted;
        gManager.eGameFinished += GameFinished;
	}

    private void GameStarted()
    {
        //bulletOffset = ((int)rateOfFire % 50)*0.02f;
    }

    private void GameFinished()
    {
        movable = false;
    }

    // Update is called once per frame
    void FixedUpdate () {
        Move();
    }

    void Move()
    {
        if (EventSystem.current.currentSelectedGameObject != null) return;

#if UNITY_EDITOR
        if (Input.GetMouseButton(0) && movable)
        {
            //Convert mouseInput to world point
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var targetPosition = new Vector2(mousePos.x, transform.position.y);

            //Move the player transform smoothly
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothScale);
            if (Time.time > timeToFire)
            {

                Shoot();
                timeToFire = Time.time + 1 / rateOfFire;
            }
        }

#elif UNITY_ANDROID
        if ((Input.touchCount > 0) && movable){
            //Convert mouseInput to world point
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var targetPosition = new Vector2(mousePos.x, transform.position.y);

            //Move the player transform smoothly
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothScale);
            if (Time.time > timeToFire)
            {
                Shoot();
                timeToFire = Time.time + 1 / rateOfFire;
            }
        }
#endif
    }
    void Shoot()
    {
        bulletOffset *= -1;
        FireWeapon(weaponTop, BulletPooling.instance.SpawnBullet());
        FireWeapon(weaponBottom, BulletPooling.instance.SpawnBullet());
    }

    void FireWeapon(Transform weapon, Bullet bullet)
    {
        bullet.transform.position = weapon.position + new Vector3(bulletOffset, 0);
        bullet.transform.rotation = weapon.rotation;

        bullet.Fire();
    }
    public void CoinPickedUp()
    {
        SaveData.SaveCoins(SaveData.LoadCoins() + 1);

        if (eCoinPickedUp != null)
            eCoinPickedUp();
    }
    public void PowerPickedUp(PowerUpType type)
    {
        if (ePowerPickedUp != null)
            ePowerPickedUp(type);
    }
}


