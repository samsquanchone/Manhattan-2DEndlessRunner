using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickUpTypes { SPEED, HEALTH, WORMHOLE }


public interface PickUpBase
{
    void MovePickUp();
    void PowerUp();

    void DeletePickUp();

    void SetPoolType(PoolingObjectType type);
}
public class SpeedPickUp : MonoBehaviour, PickUpBase
{
    [SerializeField] protected float speed;

    public PickUpTypes pickUpType;
    PoolingObjectType poolType;

    private void FixedUpdate()
    {
        this.MovePickUp();
    }

    public void SetPoolType(PoolingObjectType type)
    {
        this.poolType = type; //Used to identify what pool this object is associated with, so it can return to pool when done
    }
    public void MovePickUp()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
    }

    public void PowerUp()
    {
        throw new System.NotImplementedException();

    }

    public void DeletePickUp()
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
