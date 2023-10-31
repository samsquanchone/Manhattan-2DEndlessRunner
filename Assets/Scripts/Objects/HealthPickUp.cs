using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : SpeedPickUp
{
  
    [SerializeField] int healAmount;

    private void FixedUpdate()
    {
        MovePickUp();
    }
    new void MovePickUp()
    {
        base.MovePickUp();
    }
    new void PowerUp()
    {
        
    }

    void DeletePickUp()
    {

        PoolingManager.Instance.CoolObject(this.gameObject, this.poolType);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Events.Instance.OnTriggerStinger(this.poolType);
            collision.transform.gameObject.GetComponent<PlayerStats>().PlayerHealed(healAmount);
            DeletePickUp();
        }
    }

}
