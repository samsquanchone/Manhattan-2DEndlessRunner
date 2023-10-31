using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Astroid : MonoBehaviour
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
    private void Update()
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
