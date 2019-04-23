using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    private float bulletSpeed = 10;
    private Rigidbody2D rBody;


    private void Awake()
    {
        rBody = GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    void Start () {
        BulletPooling.bulletCount++;
	}

    private void OnEnable()
    {
        BulletPooling.instance.activeBullets.Add(this);
        BulletPooling.instance.inactiveBullets.Remove(this);
    }
    private void OnDisable()
    {
        BulletPooling.instance.inactiveBullets.Add(this);
        BulletPooling.instance.activeBullets.Remove(this);
    }

    public void Fire()
    {
        rBody.AddForce(transform.up * bulletSpeed, ForceMode2D.Impulse);
    }

    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            collision.GetComponent<Enemy>().Damage();
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
