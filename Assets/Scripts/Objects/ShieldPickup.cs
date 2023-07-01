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
            shieldIsOn = true;
            playerAnimator.StopPlayback();
            playerAnimator.SetTrigger("TrShield");
            Invoke(nameof(resetShield), shieldRechargeTime);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerAnimator = player.GetComponent<Animator>();
        playerStats = player.GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void resetShield()
    {
        shieldIsOn = false;
        playerAnimator.StopPlayback();
        playerAnimator.SetTrigger("TrNorm");
        DeletePickUp();
    }
}
