using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int health;

    [SerializeField] private VisualEffect impactVFX;

    [SerializeField] public float Speed = 7.0f;

    public GameObject whiteholePrefab;


    private Animator playerAnimator;
    private GameObject player;

    Renderer visual;

    bool InHole = false;
    bool OutHole = false;
    

    private bool isHit;
    public GameObject shield;
    private ShieldPickup shieldPickup;

    bool isShieldActive = false;

    bool canShoot = true;

    Transform originTransform;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerAnimator = player.GetComponent<Animator>();

        originTransform = this.transform;
        isHit = false;
        InHole = false;
       /// shieldPickup = GameObject.Find("Shield").GetComponent<ShieldPickup>();
        UIManager.Instance.ChangePlayerHealht(health); //Set Health UI based off inspector set player deafult health
        visual = GetComponent<Renderer>();
        visual.enabled = true;
        canShoot = true;
    }

    private void LateUpdate() {
        if (InHole) {


            
            Vector3 newScale = Vector3.Lerp(transform.localScale, new Vector3(0.05f, 0.05f, 0), Speed * Time.deltaTime);
            transform.localScale = newScale;

            if (transform.localScale.x <= 0.2f) {
                canShoot = false;
                Debug.Log("spawn white hole");
                visual.enabled = false;
                StartCoroutine("WhiteholeTime");
                InHole = false;
            }
      
        }

        if (OutHole) {
            Debug.Log("Scaling!!!!!");
            visual.enabled = true;
            Vector3 newScale = Vector3.Lerp(transform.localScale, new Vector3(5, 5, 0), Speed * Time.deltaTime);
            transform.localScale = newScale;

            if (transform.localScale.x == 5f)
            {
                Debug.Log("your out!");
                OutHole = false;
                canShoot = true;

            }
        }
    }

    public void PlayerHit(int damage)
    {
        if (!isShieldActive)
        {
       
            impactVFX.Play();
            health -= damage;
            isHit = true;
            UIManager.Instance.ChangePlayerHealht(health);
        }

        if (health <= 0)
        {
            //Die
            PlayerDead();

        }

    }

    public void PlayerHealed(int amount)
    {
        if (health < 100)
            health += amount;
            UIManager.Instance.ChangePlayerHealht(health);

    }

    void PlayerDead()
    {
        impactVFX.Play();
        gameObject.SetActive(false);
        GameManager.Instance.GameOverState();
    }

    public void InWormhole () {
        InHole = true;
    }

    public void OutWormhole () {
        OutHole = true;
    }
    
    public bool CanPlayerShoot()
    {
        bool can = canShoot;

        return can;
    }
    public void SpawnWhitehole () {
        // have a reference
        GameObject _obj = PoolingManager.Instance.GetPoolObject(PoolingObjectType.WhiteHolePickUp);   //Get pooling enum that for respective astroid index
        _obj.transform.position = new Vector3(-7.0f, this.transform.position.y, -7.7f);
        _obj.transform.rotation = _obj.transform.rotation;
        _obj.SetActive(true);
        InHole = false;

    }

    public void ActivateShield(float shieldTime)
    {
        playerAnimator.StopPlayback();
        playerAnimator.SetTrigger("TrShield");
        isShieldActive = true;
        StartCoroutine(ShieldTimer(shieldTime));
    }

    IEnumerator ShieldTimer(float shieldTime)
    {
        yield return new WaitForSeconds(shieldTime);
        playerAnimator.StopPlayback();
        playerAnimator.SetTrigger("TrNorm");
        isShieldActive = false;
    }

    IEnumerator WhiteholeTime () { // this is on a seperate thread (coroutine)

        yield return new WaitForSeconds(2f);
        SpawnWhitehole();


    } 

}
