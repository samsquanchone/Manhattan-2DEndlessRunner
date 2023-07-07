using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class MeleeEnemy : Enemy
{
    [SerializeField] private int meleDamage;
    // Start is called before the first frame update
    void Start()
    {
        
        playerPosition = GameObject.Find("Player").transform;
        anim = GetComponent<Animator>();
        impactVFX = GetComponentInChildren<VisualEffect>();
        engagementPositionIndex = Random.Range(0, engagementPositions.Count);

       

        enemyState = EnemyState.Moving;

       
    }

    private void OnEnable()
    {
        initialHealth = health;
        Events.Instance.OnTriggerStinger(this.poolType);

    }

    private void OnDisable()
    {
        health = initialHealth;
    }

    protected override void Update() //Override this on inheriting, can use this functionality, as well as extending it 
    {
        switch (enemyState)
        {
            case EnemyState.Moving:
                MoveEnemy();
                break;

            case EnemyState.Engaging:
                EngageEnemy();
                break;
        }
    }

    public void MoveEnemy()
    {
        if (transform.position.x >= engagementPositions[engagementPositionIndex].position.x)
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }
        else
        {
            enemyState = EnemyState.Engaging;
        }
    }

    public void EngageEnemy()
    {
        float step = followSpeed * Time.deltaTime;

        Vector3 newYPos = new Vector3(playerPosition.position.x, playerPosition.position.y, 0);   // Disregard the player's x and z position.


        transform.position = Vector3.MoveTowards(transform.position, newYPos, step);
    }
   
    public void Damaged(int damage)
    {
        impactVFX.Play();
        health -= damage;
        if (health <= 0)
        {
            anim.Play("Enemy1_dead");
            health = initialHealth; // as we are not deleting the objects, just de-activiating them, we cant rely on on Start
            UIManager.Instance.IncrementPoints(points);
            PoolingManager.Instance.CoolObject(this.gameObject, this.poolType);
            Events.Instance.OnStopStinger(this.poolType);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Bullet"))
        {
            //Damage astroid
            
            Damaged(collision.gameObject.GetComponent<Bullet>().Damage());


        }

        else if (collision.CompareTag("Player"))
        {
            //Damage astroid
          
            collision.gameObject.GetComponent<PlayerStats>().PlayerHit(meleDamage);
            Events.Instance.OnStopStinger(this.poolType);
            PoolingManager.Instance.CoolObject(this.gameObject, this.poolType);
        }
    }

}
