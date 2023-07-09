using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WormholePickup : SpeedPickUp
{

    public Rigidbody2D rb;
    public Rigidbody2D playerRb;
    public float rotation = 0.0f;
    public float PlayerScale = 0.0f;
    private bool inside = false;

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
        if (collision.tag == "Player" && !inside)
        {
            playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            Debug.Log("Entered black hole");
            inside = true;

            if (collision.transform.position.y > this.transform.position.y ) {
                //player is above
                Debug.Log("player is above");
                playerRb.AddForce(new Vector3(0, -1.5f, 0), ForceMode2D.Impulse);
            }
            else {
                //player is below
                Debug.Log("player is below");
                playerRb.AddForce(new Vector3(0, 1.5f, 0), ForceMode2D.Impulse);
            }

            collision.gameObject.GetComponent<PlayerStats>().InWormhole();
            Events.Instance.OnTriggerStinger(this.poolType);

            //Power up player, then delete object
            int damage = Random.Range(-15, 15);
            if (damage > 0)
            {
                collision.gameObject.GetComponent<PlayerStats>().PlayerHit(damage);
            }

            else
            {
                //Convert to positive and heal
                collision.gameObject.GetComponent<PlayerStats>().PlayerHealed(Mathf.Abs(damage));
            }

            // now need to deduct or add points

            UIManager.Instance.IncrementPoints(Random.Range(-100, 100));
            StartCoroutine("DeleteTime");
            //PoolingManager.Instance.CoolObject(this.gameObject, this.poolType);

        }
    }

    IEnumerator DeleteTime()
    { // this is on a seperate thread (coroutine)

        yield return new WaitForSeconds(2f);
        PoolingManager.Instance.CoolObject(this.gameObject, this.poolType);
        inside = false;


    }


}

