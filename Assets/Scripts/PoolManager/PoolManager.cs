using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{

    public static PoolManager Singleton { get; private set; }

    private Dictionary<int, List<PoolObject>> poolDictionary = new Dictionary<int, List<PoolObject>>();

    private void Awake ()
    {
        //Set the singleton.
        if (Singleton == null)
            Singleton = this;
    }

    /// <summary>
    /// Get the selected pool asteroid
    /// </summary>
    /// <param name="poolObject"></param>
    /// <returns>Pool Asteroid object</returns>
    public PoolObject GetObject(PoolObject poolObject)
    {
        int poolKey = poolObject.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey))
        {
            List<PoolObject> poolObjectList = poolDictionary[poolKey];

            for (int i = 0; i < poolObjectList.Count; i++)
            {
                if (poolObjectList[i].active == false)
                    return poolObjectList[i];
            }

            InstantiateNewPoolObject(poolObject, poolObjectList);

            //return object added
            return poolObjectList[poolObjectList.Count - 1];
        }
        else
        {
            List<PoolObject> newPoolObjectsList = new List<PoolObject>();
            InstantiateNewPoolObject(poolObject, newPoolObjectsList);

            //Add in poolDictionary
            poolDictionary.Add(poolKey, newPoolObjectsList);

            //return object added
            return newPoolObjectsList[newPoolObjectsList.Count - 1];
        }
    }

    /// <summary>
    /// Instantiates a new PoolAsteroid and add to list.
    /// </summary>
    /// <param name="poolAsteroidToSpawn">Asteroid to instnatiate</param>
    /// <param name="poolListToAdd">List to add</param>
    private void InstantiateNewPoolObject(PoolObject poolAsteroidToSpawn, List<PoolObject> poolListToAdd)
    {
        Vector3 pos = Vector3.zero;
        pos.z = -1;

        PoolObject newPoolObject = (PoolObject)Instantiate(poolAsteroidToSpawn, pos, Quaternion.identity);

        newPoolObject.DisablePoolObject();

        //Add to pool list
        poolListToAdd.Add(newPoolObject);
    }
}
