using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormHoleGravity : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Your have entered the hole field");

            if (collision.transform.position.y > this.transform.parent.position.y) {
                Debug.Log("player is above");
                Vector3 impulse = new Vector3(0.0f, -5.0f, 0.0f);
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(impulse, ForceMode2D.Impulse);
            }
            else {
                Debug.Log("player is below");
                Vector3 impulse = new Vector3(0.0f, 5.0f, 0.0f);
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(impulse, ForceMode2D.Impulse);
            }

            //being pulled 
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector3.MoveTowards(collision.transform.position, this.transform.parent.position, 1.0f));

            // now need to deduct or add points

        }
    }
}
