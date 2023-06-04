using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

//Class used for obtaining player inputs and calling then calling the required scripts / function calls. Some things could be refactored out of this script depending on how large scope gets
public class PlayerInput : MonoBehaviour
{
    private Rigidbody2D playerRb;

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
            Debug.Log("PlayerShoot");
            foreach (GameObject obj in firePoints)
            {
                obj.SetActive(true);
                GameObject _obj = PoolingManager.Instance.GetPoolObject(PoolingObjectType.Bullet); 
                _obj.transform.position = obj.transform.position;
                _obj.transform.rotation = obj.transform.rotation;
                _obj.SetActive(true);

               // Instantiate(playerBulletPrefab, obj.transform.position, obj.transform.rotation); //Instantiate bullets at all fire points, should change to pooling
                StartCoroutine("DisableWeaponFlash");
            }
        }
    }

    public void OnShootSecondary(InputAction.CallbackContext context)
    {
        if (context.performed) //Ensures no multi-triggers
        {
            laserVFX.Play();
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 1000);
            if (hit.collider != null)
            {
                Debug.Log("Boom!" + hit.collider.gameObject.name);
               if (hit.collider.gameObject.GetComponent<Astroid>() != null)
                hit.collider.gameObject.GetComponent<Astroid>().Hit(80); //Will need to change to diffeent method down line, to avoid null exeptions for different object types that are not astroids
            }
        }

    }

    //Should abstact some shoot functonality to another script if this script gets too large
    IEnumerator DisableWeaponFlash()
    {
        yield return new WaitForSeconds(0.1f);
        foreach (GameObject obj in firePoints)
        {
            obj.SetActive(false);
        }
    }
}
