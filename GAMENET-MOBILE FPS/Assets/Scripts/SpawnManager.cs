using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    public List<Transform> SpawnPoints = new List<Transform>();

    public void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public Transform GetRandomSpawnPoint()
    {
        if (SpawnPoints.Count > 0)
        {
            int randomSpawnIndex = Random.Range(0, SpawnPoints.Count);
            return SpawnPoints[randomSpawnIndex];
            Debug.Log("Spawned in a set location" + SpawnPoints[randomSpawnIndex].position);
        }
        else
        {
            return null; 
        }
        
    }


}
