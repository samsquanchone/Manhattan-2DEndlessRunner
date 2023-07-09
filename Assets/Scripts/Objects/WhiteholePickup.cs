using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteholePickup : SpeedPickUp
{

    private PlayerStats PlayerStats;

    // Start is called before the first frame update
    void Start()
    {

        PlayerStats = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();

        this.transform.SetParent(null);
        Vector3 spawnPos = new Vector3(11f, 0.0f, 0.0f);
        this.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        this.transform.position = spawnPos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.MovePickUp();
        if (this.transform.position.x <= -7.5) {
            PlayerStats.OutWormhole();
            Events.Instance.OnStopStinger(this.poolType);
            StartCoroutine("DeleteTime");
        }
    }

    public void MovePickUp()
    {
        base.MovePickUp();
    }

    IEnumerator DeleteTime()
    { // this is on a seperate thread (coroutine)

        yield return new WaitForSeconds(1f);
        PoolingManager.Instance.CoolObject(this.gameObject, this.poolType);


    }
}
