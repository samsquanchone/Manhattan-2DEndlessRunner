using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WormholePickup : SpeedPickUp
{

    public Rigidbody2D rb;
    public float rotation = 0.0f;

    void start() {

        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.MovePickUp();

        rb.SetRotation(rotation);
        rotation += 1.0f;
    }


    public void MovePickUp()
    {
        base.MovePickUp();
    }
    public void PowerUp()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Entered black hole");

            //Power up player, then delete object
            int damage = Random.Range(-15, 15);
            collision.gameObject.GetComponent<PlayerStats>().PlayerHit(damage);

            // now need to deduct or add points

            UIManager.Instance.IncrementPoints(Random.Range(-100, 100));
            PoolingManager.Instance.CoolObject(this.gameObject, this.poolType);

        }
    }


}

