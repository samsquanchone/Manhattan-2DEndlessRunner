using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int health;

    [SerializeField] private VisualEffect impactVFX;

    private bool isHit;
    public GameObject shield;
    private ShieldPickup shieldPickup;

    private void Start()
    {

       
        isHit = false;
        shieldPickup = shield.GetComponent<ShieldPickup>();
        UIManager.Instance.ChangePlayerHealht(health); //Set Health UI based off inspector set player deafult health
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
}
