using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

//Class used for obtaining player inputs and calling then calling the required scripts / function calls. Some things could be refactored out of this script depending on how large scope gets
public class PlayerInput : MonoBehaviour
{
    private Rigidbody2D playerRb;

    private WeaponController playerWepController;

    private Vector2 playerPos;
    [SerializeField] private float speed;

    [SerializeField] GameObject playerBulletPrefab;
    [SerializeField] GameObject playerLazerPrefab;

    [SerializeField] private List<GameObject> firePoints;
    [SerializeField] private Transform lazerFirePoint;


    [SerializeField] VisualEffect laserVFX;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>(); //Find RB on gameObj and set as var 
        playerWepController = GetComponent<WeaponController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        playerPos = context.ReadValue<Vector2>(); //Read action value from input

        playerRb.AddForce(transform.up * speed); //Apply force to Y axis

    }

    public void OnShootPrimary(InputAction.CallbackContext context)
    {
        if (context.performed) //Ensures no multi-triggers
        {
            playerWepController.FirePrimaryWeapon();
        }
    }

    public void OnShootSecondary(InputAction.CallbackContext context)
    {
        if (context.performed) //Ensures no multi-triggers
        {
            playerWepController.FireSecondaryWeapon();
        }

    }

}
