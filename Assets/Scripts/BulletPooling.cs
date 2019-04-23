using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPooling : MonoBehaviour {


    public static BulletPooling instance;
    public GameObject bulletPrefab;


    public static int bulletCount = 0;
    public List<Bullet> activeBullets;
    public List<Bullet> inactiveBullets;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        activeBullets = new List<Bullet>();
        inactiveBullets = new List<Bullet>();
    }
    public Bullet SpawnBullet()
    {
        Bullet bullet;

        if (bulletCount >= 50)
        {
            bullet = inactiveBullets[0];
            bullet.gameObject.SetActive(true);
        }
        else
        {
            GameObject bulletClone = Instantiate(bulletPrefab);
            bullet = bulletClone.GetComponent<Bullet>();
        }
        return bullet;
    }
    private void OnDestroy()
    {
        bulletCount = 0;
    }
}
