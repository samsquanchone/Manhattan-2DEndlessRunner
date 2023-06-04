using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Boarder { TOP, BOTTOM };
public class BoarderCollision : MonoBehaviour
{
    private Vector3 impulse;
    [SerializeField] private Boarder boarder;

    private void Start()
    {
        switch (boarder)
        {
            case (Boarder.TOP):

                impulse = new Vector3(0.0f, -5f, 0.0f);
                break;

            case (Boarder.BOTTOM):

                impulse = new Vector3(0.0f, 5f, 0.0f);
                break;

        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(impulse, ForceMode2D.Impulse);
        }
    }

}
