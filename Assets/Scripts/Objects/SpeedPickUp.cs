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

    public override void SetPoolType(PoolingObjectType type)
    {
        this.poolType = type; //Used to identify what pool this object is associated with, so it can return to pool when done
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
        PoolingManager.Instance.CoolObject(this.gameObject, this.poolType); //Return instance to pool to be reused
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Pick up registered");
            Events.Instance.OnTempoChanged(175);

            StartCoroutine("PowerUpDurationTimer");
            //Power up player, then delete object

            int damage = Random.Range(-4, 10);
            collision.gameObject.GetComponent<PlayerStats>().PlayerHit(damage);


        }
    }

    IEnumerator PowerUpDurationTimer()
    {
        yield return new WaitForSeconds(5f);
        Events.Instance.ResetTempo();
    }

}
