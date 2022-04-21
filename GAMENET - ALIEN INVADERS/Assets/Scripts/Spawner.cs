using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spawner : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public int EnemyCountToSpawn;
    public float SpawnDelay;
    public float SpawnTimer; 

    private Coroutine SpawnCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        { 
            SpawnEnemies();

        }
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemies()
    {
        SpawnCoroutine = StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(SpawnTimer);
        for(int i = 0; i < EnemyCountToSpawn; i++)
        {
            GameObject enemy = PhotonNetwork.Instantiate(EnemyPrefab.name, this.transform.position, this.transform.rotation);
            yield return new WaitForSeconds(SpawnDelay);
        }
       
    }
}
