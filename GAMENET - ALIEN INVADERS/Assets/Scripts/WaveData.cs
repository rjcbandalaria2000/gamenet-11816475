using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWaveData", menuName = "WaveData")]
public class WaveData : ScriptableObject
{
    public List<GameObject> EnemiesToSpawn = new List<GameObject>();
    public List<int> NumberOfEnemies = new List<int>();
    public float PrepTime;
}
