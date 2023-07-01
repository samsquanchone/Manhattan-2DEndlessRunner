using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum PoolingObjectType { Astroid1, Astroid2, Astroid3, Wormhole, Bullet, SpeedPickUp, HealthPickUp, WormHolePickUp, EnemyBullet, Enemy, ShieldPickup};


public class PoolingManager : MonoBehaviour
{
    public static PoolingManager Instance => m_instance;
    private static PoolingManager m_instance;

    public List<PoolInfo> listOfPool;

    private Vector3 defaultPos = new Vector3(-100, -100, -100);

  

    // Start is called before the first frame update
    void Start()
    {
        m_instance = this;

        for (int i = 0; i < listOfPool.Count; i++)
        {
            FillPool(listOfPool[i]);
        }

    }

    void FillPool(PoolInfo info)
    {
        for (int i = 0; i < info.amount; i++)
        {
            GameObject objInstance = null;
            objInstance = Instantiate(info.prefab, info.container.transform);
            objInstance.gameObject.SetActive(false);
            objInstance.transform.position = defaultPos;
            info.pool.Add(objInstance);
        }
    }

    public GameObject GetPoolObject(PoolingObjectType type)
    {
        PoolInfo selected = GetPoolByType(type);
        List<GameObject> pool = selected.pool;

        GameObject objInstance = null;
        if (pool.Count > 0)
        {
            objInstance = pool[pool.Count - 1];
            pool.Remove(objInstance);
        }
        else
        {
            objInstance = Instantiate(selected.prefab, selected.container.transform);
        }

        return objInstance;
    }

    public void CoolObject(GameObject obj, PoolingObjectType type)
    {
        obj.SetActive(false);
        obj.transform.position = new Vector3(0, 0, 0);
        obj.transform.rotation = new Quaternion(0, 0, 0, 0);

        PoolInfo selected = GetPoolByType(type);
        List<GameObject> pool = selected.pool;


        pool.Add(obj);


    }


    private PoolInfo GetPoolByType(PoolingObjectType type)
    {
        for (int i = 0; i < listOfPool.Count; i++)
        {
            if (type == listOfPool[i].type)
            {
                return listOfPool[i];
            }
        }

        return null;
    }
}

[System.Serializable]
public class PoolInfo
{
    public PoolingObjectType type; //Enum type
    public int amount; //Amount to pool
    public GameObject prefab;
    public GameObject container;
    public List<GameObject> pool = new List<GameObject>();
}
