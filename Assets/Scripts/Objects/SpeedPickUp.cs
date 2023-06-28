using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemPickUps;

public class SpeedPickUp : PickUpBase
{

    private void FixedUpdate()
    {
        this.MovePickUp();
    }
    public override void MovePickUp()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
    }

    public override void PowerUp()
    {
        throw new System.NotImplementedException();

    }

    public override void DeletePickUp()
    {
        throw new System.NotImplementedException();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Pick up registered");
            Events.Instance.OnTempoChanged(175);

            StartCoroutine("PowerUpDurationTimer");
            //Power up player, then delete object
        }
    }

    IEnumerator PowerUpDurationTimer()
    {
        yield return new WaitForSeconds(5f);
        Events.Instance.ResetTempo();
    }

}
