using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


/// <summary>
/// Interface for environmental hazard, could share pick up, but pick up does not use physics to move, instead transform.
/// So unique implementation used for this type of object
/// </summary>
public interface EnvironmentalHazard
{
    void MoveHazard();
    void SetPoolingType(PoolingObjectType poolType);

    void Hit(int damage);

    void Destroyed();
}

/// <summary>
/// Example implementation of a base enviro type, inherit from this to be able to access functionality 
/// </summary>
public class Astroid : MonoBehaviour, EnvironmentalHazard
{
    private Rigidbody2D rb;

    [SerializeField] private float speed;
    [SerializeField] int health;
    [SerializeField] int damage;
    [SerializeField] int points;

    //VFX refs
    [SerializeField] VisualEffect impactVFX;

    PoolingObjectType astroidType;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        MoveHazard();
    }

    public void MoveHazard()
    {
        rb.AddForce(-transform.right * speed);
        transform.Rotate(new Vector3(0, 0, 20 * Time.deltaTime));

        if (transform.position.x <= -18f)
        {
            PoolingManager.Instance.CoolObject(this.gameObject, astroidType);
        }
    }
    public void SetPoolingType(PoolingObjectType poolType)
    {
        astroidType = poolType;
    }
    public void Hit(int damage)
    {
        impactVFX.Play();
        health -= damage;
    }

    public void Destroyed()
    {
        //Destroy object here
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //DamagePlayer
            Debug.Log("PlayerHit");
            collision.gameObject.GetComponent<PlayerStats>().PlayerHit(damage); //Damage player
            StartCoroutine(DestructionTimer(false));

        }
        else if (collision.tag == "Bullet")
        {
            //Damage astroid
            Debug.Log("AstroidHit");
            health -= collision.gameObject.GetComponent<Bullet>().Damage();

            impactVFX.Play(); //Trigger impact vfx

            if (health <= 0)
            {
                StartCoroutine(DestructionTimer(true));
                  
            }
        }
    }

    IEnumerator DestructionTimer(bool AddPoints) //To ensure VFX plays
    {
        yield return new WaitForSeconds(0.1f);
        if (AddPoints) { UIManager.Instance.IncrementPoints(points); } //Add astroids points value to UI var for points
        PoolingManager.Instance.CoolObject(this.gameObject, astroidType);
    }
}
