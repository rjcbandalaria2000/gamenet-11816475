using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spawner : MonoBehaviour
{
    public List<GameObject> EnemyPrefabs = new List<GameObject>();
    public List<int> EnemyCountToSpawn = new List<int>();
    public float SpawnDelay;
    public float SpawnTimer; 

    private Coroutine SpawnCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        //if (PhotonNetwork.IsMasterClient)
        //{ 
        //    SpawnEnemies();

        //}
       
    }

    public void SpawnEnemies()
    {
        SpawnCoroutine = StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(SpawnTimer);
        for(int i = 0; i < EnemyPrefabs.Count; i++)
        {
            for (int j = 0; j < EnemyCountToSpawn[i]; j++)
            {
                GameObject enemy = PhotonNetwork.Instantiate(EnemyPrefabs[i].name, this.transform.position, this.transform.rotation);
                GameManager.Instance.PhotonAddEnemies();
                yield return new WaitForSeconds(SpawnDelay);
            }
        }
       
    }
}
