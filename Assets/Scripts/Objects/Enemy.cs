using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public enum EnemyType { BasicEnemy }
public enum EnemyState { Moving, Engaging }

interface IEnemy
{
    PoolingObjectType poolType { get; }
    void MoveEnemy();

    void EngageEnemy();
    void Shoot();

    void Damaged(int damage);

    

}


public class Enemy : MonoBehaviour, IEnemy
{

    public PoolingObjectType m_poolType;
    public PoolingObjectType poolType
    

    {
        get
        {
            return m_poolType;
        }
    }

    public Animator anim;

    private bool IsDead = false;

    protected VisualEffect impactVFX;
    [SerializeField] protected int health;
    [SerializeField] protected int initialHealth;

    [SerializeField] protected int points;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float followSpeed;
    [SerializeField] protected List<Transform> engagementPositions;
    protected int engagementPositionIndex;
    protected Transform playerPosition;

    [SerializeField] private float minShotCooldown;
    [SerializeField] private float maxShotCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bullet;

    protected EnemyState enemyState = EnemyState.Moving;


    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("isDead", false);
        
        impactVFX = GetComponentInChildren<VisualEffect>();
        
        playerPosition = GameObject.Find("Player").transform;
   
    }
    
    void OnEnable()
    {
        this.initialHealth = health;
        this.engagementPositionIndex = Random.Range(0, engagementPositions.Count);
        this.Invoke(nameof(Shoot), 1f);
        Events.Instance.OnTriggerStinger(this.poolType);
    }

    void OnDisable()
    {
        health = initialHealth;
    }

    protected virtual void Update() //Override this on inheriting, can use this functionality, as well as extending it 
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

        Vector3 newYPos = new Vector3(0, playerPosition.position.y, 0);   // Disregard the player's x and z position.


        transform.position = Vector3.MoveTowards(transform.position, newYPos, step);
    }
    private void ShotCooldown()
    {
        if(this.gameObject.activeSelf)
        StartCoroutine("EnemyShotTimer");
        
    }

    IEnumerator EnemyShotTimer()
    {
        float delay = Random.Range(minShotCooldown, maxShotCooldown);

        yield return new WaitForSeconds(delay);
        Shoot();

    }

    public void Shoot()
    {
        GameObject _obj = PoolingManager.Instance.GetPoolObject(PoolingObjectType.EnemyBullet);
        _obj.transform.position = firePoint.transform.position;
        _obj.transform.rotation = firePoint.transform.rotation;
        _obj.SetActive(true);

        ShotCooldown();
    }

    public void Damaged(int damage)
    {
        impactVFX.Play();
        if (IsDead == false) { // using this var to prevent further animations 
            anim.Play("Enemy1_hit");
        }
        
        health -= damage;
        if (health <= 0)
        {
            Debug.Log("Enemy Dead");
            if (IsDead == false) {
                anim.Play("Enemy1_dead");
            }
            
            IsDead = true; 
            StartCoroutine("DeathTime");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Bullet"))
        {
            //Damage astroid
            Debug.Log("eNEMY HIUT");
            Damaged(collision.gameObject.GetComponent<Bullet>().Damage());
            
        }
    }


    IEnumerator DeathTime()
    { // this is on a seperate thread (coroutine)

        yield return new WaitForSeconds(0.3f);
        health = initialHealth; // as we are not deleting the objects, just de-activiating them, we cant rely on on Start
        UIManager.Instance.IncrementPoints(points);
        PoolingManager.Instance.CoolObject(this.gameObject, this.poolType);
        Events.Instance.OnStopStinger(this.poolType);
        IsDead = false;

    }
}

   