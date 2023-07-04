using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnState {NORMAL, LIGHTSPEEDPOWERUP }
enum ObjectType { ENEMY, ENVIRONMENTALHAZARD, PICKUP }; //Used to define base types for getting random type to spawn 
public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance => m_instance;
    private static SpawnManager m_instance;

    [SerializeField] private List<Transform> spawnPostitions;
    [SerializeField] List<GameObject> environmentalHazardsPrefabs;
    [SerializeField] List<GameObject> pickUpPrefabs;
    [SerializeField] List<GameObject> ENEMYPrefabs;
    [SerializeField] private SpawnState spawnState = SpawnState.NORMAL;

    [SerializeField] float spawnTime;

    // Start is called before the first frame update
    void Start()
    {
        m_instance = this;
        InvokeRepeating("StartSpawnTimer", 0.1f, 5f);
    }
    public void StopRoutine()
    {
        CancelInvoke("StartSpawnTimer");
    }
    void StartSpawnTimer()
    {
        StartCoroutine(SpawnTimer(environmentalHazardsPrefabs.Count));
    }

    IEnumerator SpawnTimer(int poolSize)
    {
        yield return new WaitForSeconds(0.1f);

        if (spawnState == SpawnState.NORMAL)
        {

            int i = Random.Range(0, poolSize); //Object index
            int x = Random.Range(0, spawnPostitions.Count); //Spawn pos index

            ObjectType spawnType = (ObjectType)Random.Range(0, System.Enum.GetNames(typeof(ObjectType)).Length); //Get random index from enum 
            GameObject _obj;


            switch (spawnType)
            {
                case ObjectType.ENVIRONMENTALHAZARD:
                    _obj = PoolingManager.Instance.GetPoolObject(GetEnvironmentalHazardToSpawn(i));   //Get pooling enum that for respective astroid index
                    _obj.transform.position = spawnPostitions[x].transform.position;
                    _obj.transform.rotation = environmentalHazardsPrefabs[i].transform.rotation;
                    _obj.GetComponent<Astroid>().SetPoolingType(GetEnvironmentalHazardToSpawn(i));
                    _obj.SetActive(true);
                    break;

                case ObjectType.ENEMY:
                    i = Random.Range(0, ENEMYPrefabs.Count);
                    _obj = PoolingManager.Instance.GetPoolObject(ENEMYPrefabs[i].GetComponent<Enemy>().poolType);   //Get pooling enum that for respective astroid index
                    _obj.transform.position = spawnPostitions[x].transform.position;
                    _obj.transform.rotation = ENEMYPrefabs[i].transform.rotation;
                    _obj.SetActive(true);
                    break;

                case ObjectType.PICKUP:
                    i = Random.Range(0, pickUpPrefabs.Count); //Get random index of prefab list
                    _obj = PoolingManager.Instance.GetPoolObject(GetPickUpToSpawn(pickUpPrefabs[i].GetComponent<PickUpBase>().pickUpType));
                    _obj.transform.position = spawnPostitions[x].transform.position;
                    _obj.transform.rotation = pickUpPrefabs[i].transform.rotation;
                    _obj.SetActive(true);
                    break;

                default: //Default as other cases not defined yet
                    _obj = PoolingManager.Instance.GetPoolObject(GetEnvironmentalHazardToSpawn(i)); //Get pooling enum that for respective astroid index
                    _obj.transform.position = spawnPostitions[x].transform.position;
                    _obj.transform.rotation = environmentalHazardsPrefabs[i].transform.rotation;
                    _obj.SetActive(true);
                    break;

            }
        }
    }


    public void ChangeState(SpawnState state)
    {
        spawnState = state;
        switch (state)
        {
            case SpawnState.NORMAL:
                Debug.Log("normal speed back");
                Events.Instance.ResetTempo();
                BackgroundLooper.Instance.ResetSpeed();
                break;
            case SpawnState.LIGHTSPEEDPOWERUP:
                var pickups = GameObject.FindGameObjectsWithTag("PickUp");
                var enemies = GameObject.FindGameObjectsWithTag("Enemy");
                var EnvironmentalHazards = GameObject.FindGameObjectsWithTag("EnvironmentalHazard");
               
                foreach (var pickUp in pickups)
                {
                    pickUp.SetActive(false);
                }

                foreach (var enemy in enemies)
                {
                    enemy.SetActive(false);
                }

                foreach (var hazard in EnvironmentalHazards)
                {
                    hazard.SetActive(false);
                }

                BackgroundLooper.Instance.ChangeSpeed(1);

                StartCoroutine("LightSpeedStateTimer");
                break;
                
        }
    }

    private void LateUpdate()
    {
        if (spawnState == SpawnState.LIGHTSPEEDPOWERUP)
        {
            UIManager.Instance.IncrementPoints(1);
        }
    }
    IEnumerator LightSpeedStateTimer()
    {
        yield return new WaitForSeconds(5f);
        ChangeState(SpawnState.NORMAL);
    }

    void OnDestroy()
    {
        StopCoroutine("StartSpawnTimer");
    }


    PoolingObjectType GetEnvironmentalHazardToSpawn(int spawnIndex)
    {
        PoolingObjectType type;

        switch (spawnIndex)
        {
            case 0:
                type = PoolingObjectType.Astroid1;
                break;

            case 1:
                type = PoolingObjectType.Astroid2;
                break;

            case 2:
                type = PoolingObjectType.Astroid3;
                break;

            default:
                type = PoolingObjectType.Astroid1;
                break;

        }

        return type;
    }

    PoolingObjectType GetPickUpToSpawn(PickUpTypes pickUpType)
    {
        PoolingObjectType type;

        switch (pickUpType)
        {
            case PickUpTypes.HEALTH:
                type = PoolingObjectType.HealthPickUp;
                break;

            case PickUpTypes.SPEED:
                type = PoolingObjectType.SpeedPickUp;
                break;

            case PickUpTypes.WORMHOLE:
                type = PoolingObjectType.WormHolePickUp;
                break;

            case PickUpTypes.SHIELD:
                type = PoolingObjectType.ShieldPickup;
                break;

            default:
                type = PoolingObjectType.HealthPickUp;
                break;

        }

        return type;

    }

}
