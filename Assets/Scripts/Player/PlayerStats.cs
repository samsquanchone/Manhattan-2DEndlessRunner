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

    Renderer visual;

    bool InHole = false;
    bool OutHole = false;
    

    private bool isHit;
    public GameObject shield;
    private ShieldPickup shieldPickup;

    private void Start()
    {

       
        isHit = false;
        shieldPickup = shield.GetComponent<ShieldPickup>();
        UIManager.Instance.ChangePlayerHealht(health); //Set Health UI based off inspector set player deafult health
        visual = GetComponent<Renderer>();
        visual.enabled = true;
    }

    private void LateUpdate() {
        if (InHole) {
            
            

            //newScale = this.transform.localScale;
            
            
            Vector3 newScale = Vector3.Lerp(transform.localScale, new Vector3(0.05f, 0.05f, 0), Speed * Time.deltaTime);
            transform.localScale = newScale;
      

            

            if (transform.localScale.x <= 0.2f) {
                Debug.Log("spawn white hole");
                visual.enabled = false;
                StartCoroutine("WhiteholeTime");
                InHole = false;
            }
      
        }

        if (OutHole) {
            Debug.Log("Scaling!!!!!");
            visual.enabled = true;
            transform.position = new Vector3(-7.1f, 0.0f, 0.0f);
            Vector3 newScale = Vector3.Lerp(transform.localScale, new Vector3(5, 5, 0), Speed * Time.deltaTime);
            transform.localScale = newScale;

            if (transform.localScale.x >= 0.5f)
            {
                Debug.Log("your out!");
                OutHole = false;

            }
        }
    }

    public void PlayerHit(int damage)
    {
        if (shieldPickup.shieldIsOn == false)
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

    public void SpawnWhitehole () {
        // have a reference
        Debug.Log("Spawn white hole");
        Instantiate(whiteholePrefab, this.transform, true);
        //whiteholePrefab.transform.SetParent(null);
        //Vector3 spawnPos = new Vector3(-7.0f, this.transform.position.y, -7.7f);
        //whiteholePrefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        //whiteholePrefab.transform.position = spawnPos;
        InHole = false;

    }

    IEnumerator WhiteholeTime () { // this is on a seperate thread (coroutine)

        yield return new WaitForSeconds(2f);
        SpawnWhitehole();


    } 

}
