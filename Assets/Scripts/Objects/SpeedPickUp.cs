using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;




public interface PickUpBase
{
   
    PoolingObjectType poolType { get; }

    void MovePickUp();
    void PowerUp();

    void DeletePickUp();

  
}
public class SpeedPickUp : MonoBehaviour, PickUpBase
{
    [SerializeField] protected float speed;


    public PoolingObjectType m_poolType;
    public PoolingObjectType poolType
    {
        get
        {
            return m_poolType;
        }
    }

    protected virtual void Start()
    {
      
    }
    private void FixedUpdate()
    {
        this.MovePickUp();
    }

    
    public void MovePickUp()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x <= -17)
        {
            DeletePickUp();
        }
    }

    public void PowerUp()
    {
        StartCoroutine("PowerUpDurationTimer");
        Events.Instance.OnTempoChanged(175);

    }

    public void DeletePickUp()
    {
        PoolingManager.Instance.CoolObject(this.gameObject, this.poolType); //Return instance to pool to be reused
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Events.Instance.OnTriggerStinger(this.poolType);
            PowerUp();
        }
    }

    IEnumerator PowerUpDurationTimer()
    {
        SpawnManager.Instance.ChangeState(SpawnState.LIGHTSPEEDPOWERUP);

        yield return new WaitForSeconds(0.1f);
        DeletePickUp();
    }

}
