using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D bulletRb;

    [SerializeField] private float speed;
    [SerializeField] private int damage;

    [SerializeField] GameObject impactVFX;

    // Start is called before the first frame update
    void Start()
    {
        bulletRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        bulletRb.AddForce(transform.right * speed);

        if (this.gameObject.transform.position.x >= 10)
        {
            PoolingManager.Instance.CoolObject(this.gameObject, PoolingObjectType.Bullet); //Return Bullet prefab back to pool 
        }
    }

    public int Damage()
    {
        int _damage = damage;

        PoolingManager.Instance.CoolObject(this.gameObject, PoolingObjectType.Bullet); //Return Bullet prefab back to pool 

        return _damage;
    }

    
}
