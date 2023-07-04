using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPickup : SpeedPickUp
{
    private Animator playerAnimator;
    private GameObject player;
    public PlayerStats playerStats;

    public bool shieldIsOn;

    [SerializeField] int shieldRechargeTime;

    
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
            collision.gameObject.GetComponent<PlayerStats>().ActivateShield(shieldRechargeTime);
            Events.Instance.OnTriggerStinger(this.pickUpType);
            /*
            shieldIsOn = true;
            playerAnimator.StopPlayback();
            playerAnimator.SetTrigger("TrShield");
            Invoke(nameof(resetShield), shieldRechargeTime);
            */
            DeletePickUp();
            
        }
    }
   

    private void resetShield()
    {
        
        playerAnimator.StopPlayback();
        playerAnimator.SetTrigger("TrNorm");
        shieldIsOn = false;
    }
}
