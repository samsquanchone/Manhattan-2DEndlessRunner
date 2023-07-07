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

    float difficultyTimer = 10;

    // Start is called before the first frame update
    void Start()
    {
        m_instance = this;
        StartSpawnTimer();
    }
    public void StopRoutine()
    {
        CancelInvoke("StartSpawnTimer");
    }
    void StartSpawnTimer()
    {
        StartCoroutine(SpawnTimer(spawnTime));
    }

    private void Update()
    {
        difficultyTimer -= Time.deltaTime;
        if (difficultyTimer <= 0 && spawnTime > 1.1f)
        {
            Debug.Log("Difficulty ramp");
            BackgroundLooper.Instance.ChangePerSpeed(0.025f);
            spawnTime -= 0.1f; 
            difficultyTimer = 10;
        }
    }


    IEnumerator SpawnTimer(float spawnTime)
    {
        
        yield return new WaitForSeconds(Random.Range( spawnTime - 1, spawnTime)); //Should use tunrary operator here to ensure min spawn time cannont be set below 0

        if (spawnState == SpawnState.NORMAL)
        {

            int i = 0; //Object index
            int x = Random.Range(0, spawnPostitions.Count); //Spawn pos index

            ObjectType spawnType = (ObjectType)Random.Range(0, System.Enum.GetNames(typeof(ObjectType)).Length); //Get random index from enum 
            GameObject _obj;


            switch (spawnType)
            {
                case ObjectType.ENVIRONMENTALHAZARD:
                     i = Random.Range(0, environmentalHazardsPrefabs.Count);
                    _obj = PoolingManager.Instance.GetPoolObject(environmentalHazardsPrefabs[i].GetComponent<Astroid>().GetPoolingType());   //Get pooling enum that for respective astroid index
                    _obj.transform.position = spawnPostitions[x].transform.position;
                    _obj.transform.rotation = environmentalHazardsPrefabs[i].transform.rotation;
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
                    _obj = PoolingManager.Instance.GetPoolObject(pickUpPrefabs[i].GetComponent<PickUpBase>().poolType);
                    _obj.transform.position = spawnPostitions[x].transform.position;
                    _obj.transform.rotation = pickUpPrefabs[i].transform.rotation;
                    _obj.SetActive(true);
                    break;

                default: //Default as other cases not defined yet
                    _obj = PoolingManager.Instance.GetPoolObject(environmentalHazardsPrefabs[i].GetComponent<Astroid>().GetPoolingType()); //Get pooling enum that for respective astroid index
                    _obj.transform.position = spawnPostitions[x].transform.position;
                    _obj.transform.rotation = environmentalHazardsPrefabs[i].transform.rotation;
                    _obj.SetActive(true);
                    break;

            }
        }

        StartSpawnTimer();
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

}
