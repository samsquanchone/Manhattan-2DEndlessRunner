using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum ObjectType { ENEMY, ENVIRONMENTALHAZARD, PICKUP }; //Used to define base types for getting random type to spawn 
public class SpawnManager : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPostitions;
    [SerializeField] List<GameObject> environmentalHazardsPrefabs;
    [SerializeField] List<GameObject> pickUpPrefabs;


    [SerializeField] float spawnTime;

    // Start is called before the first frame update
    void Start()
    {
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

           /*case ObjectType.ENEMY:

                break;
           */
              case ObjectType.PICKUP:
                 i = Random.Range(0, pickUpPrefabs.Count); //Get random index of prefab list
                _obj = PoolingManager.Instance.GetPoolObject(GetPickUpToSpawn(pickUpPrefabs[i].GetComponent<SpeedPickUp>().pickUpType));
                _obj.transform.position = spawnPostitions[x].transform.position;
                _obj.transform.rotation = pickUpPrefabs[i].transform.rotation;
                _obj.GetComponent<PickUpBase>().SetPoolType(GetPickUpToSpawn(pickUpPrefabs[i].GetComponent<SpeedPickUp>().pickUpType));
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

            default:
                type = PoolingObjectType.HealthPickUp;
                break;

        }

        return type;

    }

}
