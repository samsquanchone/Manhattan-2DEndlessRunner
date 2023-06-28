using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemPickUps;

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


    public override void MovePickUp()
    {
        base.MovePickUp();
    }
    public override void PowerUp()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Entered black hole");

            //Power up player, then delete object
            int damage = Random.Range(5, 10);
            collision.gameObject.GetComponent<PlayerStats>().PlayerHit(damage);

            // now need to deduct or add points

        }
    }


}

