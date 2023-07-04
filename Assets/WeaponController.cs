using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WeaponController : MonoBehaviour
{
    [SerializeField] GameObject playerBulletPrefab;
    [SerializeField] GameObject playerLazerPrefab;

    [SerializeField] private List<GameObject> firePoints;
    [SerializeField] private Transform lazerFirePoint;

    [SerializeField] private float lazerCooldownTime;

    bool isLazerCool = true;
    bool isLazerActive = false;

    [SerializeField] VisualEffect laserVFX;
    // Start is called before the first frame update

    public void FirePrimaryWeapon()
    {
        Debug.Log("PlayerShoot");
        foreach (GameObject obj in firePoints)
        {
            obj.SetActive(true);
            GameObject _obj = PoolingManager.Instance.GetPoolObject(PoolingObjectType.Bullet);
            _obj.transform.position = obj.transform.position;
            _obj.transform.rotation = obj.transform.rotation;
            _obj.SetActive(true);

          
            StartCoroutine("DisableWeaponFlash");
        }
    }

    private void Update()
    {   if (isLazerActive)
        {
            Vector2 laserDir = transform.TransformDirection(Vector2.right) * 100;
            Debug.DrawRay( lazerFirePoint.transform.position, laserDir, Color.white);

            RaycastHit2D hit = Physics2D.Raycast(lazerFirePoint.position, laserDir);

            if(hit.collider != null)

            if (hit.collider.CompareTag("EnvironmentalHazard"))
            {
               
                hit.collider.gameObject.GetComponent<Astroid>().Hit(80);  //Will need to change to diffeent method down line, to avoid null exeptions for different object types that are not astroids
            }

            else if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.gameObject.GetComponent<Enemy>().Damaged(80);
            }
        }
    }

    public void FireSecondaryWeapon()
    {   if(isLazerCool)
        laserVFX.Play();
        StartCoroutine("ChannelLazer");
        Debug.Log("SecondaryLazer Fired");
    }

    IEnumerator DisableWeaponFlash()
    {
        yield return new WaitForSeconds(0.1f);
        foreach (GameObject obj in firePoints)
        {
            obj.SetActive(false);
        }
    }

    IEnumerator ChannelLazer()
    {
        yield return new WaitForSeconds(0.5f);
        isLazerActive = true;
        StartCoroutine("CoolLazer");
        yield return new WaitForSeconds(2.5f);
        isLazerActive = false;
    }

    IEnumerator CoolLazer()
    {

        isLazerCool = false;
        yield return new WaitForSeconds(lazerCooldownTime);
        isLazerCool = true;

    }
}
