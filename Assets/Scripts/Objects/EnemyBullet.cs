using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyBullet : MonoBehaviour
{
    Rigidbody2D bulletRb;

    [SerializeField] private float speed;
    [SerializeField] private int damage;

    

    [SerializeField] GameObject impactVFX;

    // Start is called before the first frame update
    void Start()
    {
        bulletRb = GetComponent<Rigidbody2D>();

        StartCoroutine("AutoDestructTimer");
    }

    // Update is called once per frame
    void Update()
    {
        bulletRb.AddForce(-transform.right * speed);

        if (this.gameObject.transform.position.x >= 10)
        {
            PoolingManager.Instance.CoolObject(this.gameObject, PoolingObjectType.EnemyBullet); //Return Bullet prefab back to pool 
        }
    }

    

    IEnumerator AutoDestructTimer()
    {
        yield return new WaitForSeconds(3f);
        PoolingManager.Instance.CoolObject(this.gameObject, PoolingObjectType.EnemyBullet); //Return Bullet prefab back to pool 

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.transform.gameObject.GetComponent<PlayerStats>().PlayerHit(damage);
            PoolingManager.Instance.CoolObject(this.gameObject, PoolingObjectType.EnemyBullet); //Return Bullet prefab back to pool 

        }

    }
}
