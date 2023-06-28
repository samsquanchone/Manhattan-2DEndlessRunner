using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int health;

    [SerializeField] private VisualEffect impactVFX;

    private void Start()
    {

        UIManager.Instance.ChangePlayerHealht(health); //Set Health UI based off inspector set player deafult health
    }

    public void PlayerHit(int damage)
    {
        health -= damage;
        impactVFX.Play();
        UIManager.Instance.ChangePlayerHealht(health);

        if (health <= 0)
        {
            //Die
            PlayerDead();

        }


    }

    void PlayerDead()
    {
       gameObject.SetActive(false);
    }
}
